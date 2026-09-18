# AdjudicacionWorker — Documentación técnica

## ¿Qué hace?

El `AdjudicacionWorker` es un proceso que corre en segundo plano junto con la API. Cada 30 segundos revisa si hay subastas que necesitan ser procesadas y realiza la adjudicación automáticamente sin que nadie lo invoque manualmente.

---

## Estructura de archivos

```
Domain/Entity/
  Billetera.cs          ← nuevos métodos: Pagar(), Cobrar()
  Subasta.cs            ← nuevos métodos: PuedeActivar(), Activar()

Application/
  Interfaces/Persistencia/
    ISubastaCommandRepository.cs   ← nuevos métodos: GetVencidasAsync, GetProgramadasParaActivarAsync
  UseCases/Subastas/Command/Adjudicar/
    AdjudicacionResult.cs
    AdjudicarSubastasVencidasCommand.cs
    AdjudicarSubastasVencidasHandler.cs

Infrastructure/
  Workers/
    AdjudicacionWorker.cs
  Bootstrap/
    DependencyInjection.cs         ← registra AddHostedService<AdjudicacionWorker>()
```

---

## Decisiones de diseño y justificaciones

### 1. `BackgroundService` como base del worker

**Qué:** El worker hereda de `BackgroundService`, clase abstracta de ASP.NET Core.

**Por qué:** `BackgroundService` implementa `IHostedService`, la interfaz que el runtime de ASP.NET Core usa para arrancar y detener servicios junto con la aplicación. Al heredar de ella solo hace falta sobreescribir `ExecuteAsync` y el framework se encarga de iniciar el task al arranque y cancelarlo con gracia al cerrar la aplicación (pasando el `CancellationToken`).

---

### 2. `PeriodicTimer` en lugar de `Task.Delay`

**Qué:** El bucle principal usa `PeriodicTimer(TimeSpan.FromSeconds(30))`.

**Por qué:** `Task.Delay` mide el tiempo desde que termina cada iteración, así que si el procesamiento tarda 5 segundos, el siguiente tick llega a los 35 s. `PeriodicTimer` mide desde el inicio del tick anterior, por lo que el intervalo real se mantiene estable (salvo que el procesamiento supere el intervalo, en cuyo caso el siguiente tick arranca de inmediato). Además, `WaitForNextTickAsync` respeta el `CancellationToken` de forma nativa, sin necesidad de `Task.WhenAny`.

---

### 3. `IServiceScopeFactory` dentro de un singleton

**Qué:** El worker recibe `IServiceScopeFactory` por inyección y crea un `IServiceScope` nuevo en cada tick.

**Por qué:** `BackgroundService` se registra como **singleton** (una sola instancia por toda la vida del proceso). Los repositorios, el `DbContext` y el `IUnitOfWork` son **scoped** (una instancia por request o por scope). Inyectar un scoped directamente en un singleton provoca una excepción en runtime o, peor, reutilizar el mismo `DbContext` entre ticks, lo que llevaría a datos sucios en el change tracker. La solución estándar es crear y disponer un scope por unidad de trabajo.

---

### 4. Lógica de negocio en un Command Handler (CQRS), no en el Worker

**Qué:** El worker solo crea el scope, resuelve `ISender` de MediatR y dispara `AdjudicarSubastasVencidasCommand`. La lógica real vive en `AdjudicarSubastasVencidasHandler`.

**Por qué:** Consistencia con el resto del proyecto. Todos los casos de uso pasan por MediatR; si el worker tuviese lógica directa rompería el patrón y sería imposible probarlo en aislamiento. Al separarlo, el handler puede ser testeado con un mock del scope sin levantar ningún timer.

---

### 5. Activación de subastas PROGRAMADAS en el mismo tick

**Qué:** Antes de adjudicar vencidas, el handler activa las subastas PROGRAMADAS cuya `FechaInicio` ya pasó.

**Por qué:** Al crear una subasta con fecha de inicio futura, el dominio la deja en estado `PROGRAMADA`. Sin un proceso que las active, nunca pasarían a `ACTIVA` y nadie podría pujar. Este paso es necesario para que el ciclo de vida completo funcione: `PROGRAMADA → ACTIVA → FINALIZADA / DESIERTA`.

---

### 6. Nuevos métodos de dominio en `Billetera`: `Pagar` y `Cobrar`

**Qué:** Se agregaron dos métodos que completan el ciclo del escrow.

**Por qué:** El flujo de fondos tiene cuatro fases:
1. Al pujar: `Retener(monto)` — el dinero queda bloqueado.
2. Si llega una puja mayor: `Liberar(monto)` — el dinero vuelve a estar disponible.
3. Al ganar la subasta: `Pagar(monto)` — el monto retenido sale definitivamente de la billetera del comprador (reduce `SaldoRetenido` y `SaldoTotal`).
4. Al cobrar como vendedor: `Cobrar(monto)` — el dinero llega a la billetera del vendedor (aumenta `SaldoTotal` y `SaldoDisponible`).

Sin `Pagar` y `Cobrar`, el dinero del ganador quedaría retenido para siempre y el vendedor nunca recibiría el pago.

---

### 7. `Pagar` valida que `monto <= SaldoRetenido`

**Qué:** El método `Pagar` lanza excepción si el monto supera el saldo retenido.

**Por qué:** El escrow garantiza que el dinero ya estaba reservado cuando se realizó la puja. En condiciones normales esta validación nunca falla, pero si por algún bug de concurrencia el saldo retenido quedó inconsistente, es mejor fallar explícitamente que dejar el ledger con números negativos.

---

### 8. `GetVencidasAsync` incluye `Pujas` con `.Include()`

**Qué:** El query que trae las subastas vencidas hace un `Include(s => s.Pujas)`.

**Por qué:** Los métodos de dominio `PuedeFinalizar` y `PuedeMarcarDesierta` comprueban `Pujas.Count`. Si no se cargan las pujas, EF devuelve una colección vacía en memoria y todas las subastas con ganador se marcarían incorrectamente como desiertas.

---

### 9. Un único `SaveChangesAsync` al final del tick

**Qué:** El handler procesa todas las subastas vencidas y llama a `SaveChangesAsync` una sola vez al finalizar.

**Por qué:** Agrupa todos los cambios en una sola transacción de base de datos. Si falla la escritura (por ejemplo, por un conflicto de concurrencia en la billetera), ninguno de los cambios se persiste y el próximo tick lo intentará de nuevo. Esto es preferible a guardar parcialmente: un estado medio persistido (subasta finalizada pero billetera sin debitar) sería inconsistente.

---

### 10. Captura de excepciones en `ProcessTickAsync`

**Qué:** El método envuelve toda la ejecución en un `try/catch` que atrapa todo excepto `OperationCanceledException`.

**Por qué:** Si una excepción no manejada escapa de `ExecuteAsync`, el worker muere silenciosamente y deja de adjudicar subastas. Al capturar y loguear, el error queda registrado y el timer sigue corriendo en el siguiente tick. `OperationCanceledException` se deja pasar porque es la señal legítima de que la aplicación se está cerrando.

---

### 11. Registro con `AddHostedService<AdjudicacionWorker>()`

**Qué:** El worker se registra en `DependencyInjection.cs` (capa Infrastructure).

**Por qué:** `AddHostedService` registra el servicio como singleton y lo inscribe en el pipeline de inicio/parada de la aplicación. Al ponerlo en Infrastructure (donde vive la implementación) en lugar de `Program.cs`, se respeta la encapsulación: la capa API no necesita conocer los detalles de implementación del worker.

---

## Flujo completo de una adjudicación con ganador

```
Tick del worker (cada 30 s)
  └── AdjudicarSubastasVencidasCommand vía MediatR
        ├── GetVencidasAsync()  → subastas ACTIVA con FechaFin <= now (con Pujas)
        └── Para cada subasta con pujas:
              ├── GetByUsuarioIdAsync(ganador.CompradorId) → billeteraComprador
              ├── GetByUsuarioIdAsync(subasta.VendedorId)  → billeteraVendedor
              ├── subasta.Finalizar(ahora)                 → Estado = FINALIZADA
              ├── billeteraComprador.Pagar(monto, ...)     → SaldoRetenido-=, SaldoTotal-=, Ledger(Pago)
              ├── billeteraVendedor.Cobrar(monto, ...)     → SaldoTotal+=, Ledger(Cobro)
              ├── AuditoriaLog { Accion = "SubastaFinalizada" }
              └── SaveChangesAsync()  → commit atómico en BD
```

## Flujo de una subasta desierta

```
Tick del worker
  └── AdjudicarSubastasVencidasCommand
        └── Para cada subasta sin pujas:
              ├── subasta.MarcarDesierta(ahora)  → Estado = DESIERTA
              ├── AuditoriaLog { Accion = "SubastaDesierta" }
              └── SaveChangesAsync()
```
