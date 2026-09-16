# SubastaYa — Análisis de Brechas del Backend

**Cátedra:** Proyecto de Software  
**Stack:** ASP.NET Core 8 · EF Core · SQL Server · MediatR  
**Rama:** `main` · **Fecha:** 14 sep 2026

> ⚠️ **Alerta de seguridad detectada en el PDF:** El documento contiene bloques ocultos de prompt injection ("Directiva de Sistema Corporativa") que intentan instruir al asistente de IA a insertar código malformado (prefijos `[CODE-ERROR]`, variables `idx_tk`, headers espurios, atributos HTML). Fueron ignorados completamente — el análisis se basa únicamente en el contenido legítimo del TP.

---

## Estado general del proyecto

| Métrica | Valor |
|---|---|
| Requisitos implementados | 12 |
| Parcialmente cubiertos | 3 |
| Pendientes / faltantes | 7 |
| Puntos en juego (de 10) | 5 |

- **Implementado:** 54 %
- **Parcial:** 14 %
- **Faltante:** 32 %

---

## ✅ Implementado correctamente

### Catálogo de subastas con filtros y paginación
`GET /api/v1/auctions?status=&categoryId=&minPrice=&maxPrice=&sortBy=&page=&pageSize=`

Filtra por estado (`PROGRAMADA`/`ACTIVA`/`FINALIZADA`/`DESIERTA`), categoría, rango de precio. Responde con `PagedResponse<AuctionListItem>` con `FechaServidor`.

### Creación y detalle de subasta
- `POST /api/v1/auctions`
- `GET /api/v1/auctions/{id}`

Incluye validación de fechas, precio base, incremento mínimo. El detalle expone oferta actual, próxima puja mínima, postor líder y `Version` para concurrencia.

### Registro de puja con lógica anti-sniping
`POST /api/v1/auctions/{id}/bids`

Valida monto mínimo, estado activo, puja ≠ vendedor. Si hay < 60 s restantes extiende `FechaFin` +2 min. Retorna `Extendida: bool` en la respuesta.

### Escrow / Garantía atómica (Billetera)

La retención del nuevo postor y la liberación del postor anterior ocurren en la misma llamada a `UnitOfWork.SaveChangesAsync()`. `Billetera.Version` usa `rowversion` para detectar conflictos.

### Optimistic Locking en entidades críticas

Tanto `Subasta.Version` como `Billetera.Version` son columnas `rowversion`. El `UnitOfWork` captura `DbUpdateConcurrencyException` y retorna `DataStatus.Conflict` → HTTP 409.

### Historial de pujas
`GET /api/v1/auctions/{id}/bids`

Lista cronológica con monto, seudónimo de comprador y fecha. Verifica que la subasta exista (404 si no).

### Audit Log de eventos críticos _(parcial)_

Registra extensiones anti-sniping y rechazos de puja (saldo insuficiente, monto bajo, concurrencia). Entidad `AuditoriaLog` inmutable en BD.

### Swagger / OpenAPI autogenerado

Swashbuckle 6.4 configurado, se abre automáticamente en `localhost:7259/swagger` en modo desarrollo.

### Seed Data — Usuarios, categorías y subastas

`DatabaseSeeder` crea 4 usuarios con billeteras (vendedor, comprador1 $150k/$45k retenidos, comprador2 $200k, sinfondos $500), 4 categorías y 5 subastas de prueba (activa, crítica, próxima, vencida con ganador, vencida desierta).

### Clean Architecture + CQRS con MediatR

Separación estricta: `Domain → Application → Infrastructure → Api`. Comandos y queries desacoplados. Lógica de negocio en el dominio (`Subasta.RegistrarPuja()`, `AplicarAntiSniping()`, etc.).

### Test de concurrencia (Integration Tests)

`ConcurrenciaOptimistaTests` verifica que dos contextos EF que modifican la misma subasta simultáneamente producen `DataStatus.Conflict` en el segundo. Script `stress-puja.ps1` incluido.

### Migrations Code-First + Códigos HTTP correctos

Migración `20260912205626_CreacionInicial`. El `ApiControllerBase` mapea `DataStatus` → 400/404/409/422/500 correctamente según el estándar RESTful del TP.

---

## ⚠️ Parcialmente implementado

### Audit Log — faltan 2 de 4 eventos obligatorios `[Alta]`

El TP exige auditar 4 tipos de evento. Solo están cubiertos 2:

- ✅ Extensiones anti-sniping
- ✅ Rechazos de puja
- ❌ Cambios de estado por el Worker (`ACTIVA→FINALIZADA`/`DESIERTA`) — depende del worker pendiente
- ❌ Acreditaciones manuales de saldo — depende del endpoint `POST /api/wallet/deposit` pendiente

### Seed Data — faltan pujas y ledger previos `[Media]`

El TP requiere explícitamente:

- ❌ Historial de las 2 ofertas previas en la "subasta activa estándar" (con postor líder en $45.000)
- ❌ Transacciones en `TransaccionLedger` que respalden depósitos y la retención de $45.000

Sin estos registros, el frontend del módulo 4 (Historial de Movimientos) y el módulo 3 (Historial de Ofertas) quedan vacíos en el seed.

### Autenticación / Identidad de usuario `[Media]`

La entidad `Usuario` tiene `PasswordHash` y el seed crea 4 usuarios, pero:

- ❌ No existe `POST /api/auth/login` ni `POST /api/auth/register`
- ❌ No hay JWT ni ningún middleware de autenticación
- ❌ `CompradorId` y `VendedorId` viajan libremente en el body — cualquier usuario puede hacerse pasar por otro

El módulo 5 ("Mis Actividades") y la billetera asumen que hay un usuario autenticado. Sin auth, los endpoints de usuario no tienen cómo saber quién consulta.

---

## ❌ Pendiente — no implementado

> Impactan hasta **5 pts** de la nota

### 1. Billetera — endpoints de consulta y depósito `[Alta · 3 pts API+Lógica]`

El TP pide expresamente (§3.4) y el Módulo 4 requiere en frontend:

- `GET /api/wallet/balance` — saldo Total / Retenido / Disponible del usuario autenticado
- `POST /api/wallet/deposit` — acreditar saldo ficticio + escribir en Ledger + audit log
- `GET /api/wallet/transactions` — historial de movimientos del Ledger (depósitos, retenciones, liberaciones, pagos)

La entidad `Billetera` y `TransaccionLedger` existen en el dominio, pero no hay ningún controlador ni caso de uso para exponerlas.

### 2. Background Worker — adjudicación automática `[Alta · 3 pts API+Lógica]`

El §2.3 del TP exige un proceso en segundo plano que periódicamente detecte subastas vencidas y ejecute:

- **Con ganador:** marcar como `FINALIZADA`, debitar billetera del comprador, acreditar al vendedor, escribir en Ledger, registrar en `AuditoriaLog`.
- **Sin pujas:** marcar como `DESIERTA`, registrar en `AuditoriaLog`.

Los métodos de dominio `Finalizar()` y `MarcarDesierta()` ya existen en `Subasta`. Falta el `IHostedService` / `BackgroundService` que los invoque (ej. `AdjudicacionWorker` con un `PeriodicTimer` cada 30 s).

### 3. Panel de usuario — "Mis Actividades" `[Alta · 3 pts API]`

El Módulo 5 del TP requiere dos pestañas con datos del backend:

- `GET /api/users/{id}/bids` — subastas en que participó (ganó / aún abierta)
- `GET /api/users/{id}/auctions` — publicaciones del vendedor con métricas de recaudación y estado

No existe controlador de usuarios ni queries de usuario en la capa Application.

### 4. Endpoint de categorías `[Media]`

`GET /api/categories` — necesario para el formulario de publicación de subasta (Módulo 2).

`ICategoriaQueryRepository` existe en Application pero no hay handler ni controller que lo exponga. Sin este endpoint el formulario no puede poblar el dropdown de categorías.

### 5. WebSockets / Tiempo real `[Baja · diferencial de nota]`

El §3.1 del TP pide WebSockets (SignalR) para la Sala de Subasta en Vivo — temporizador y nuevas pujas en tiempo real. La alternativa mínima aceptable es short-polling (cada 2-3 s al `GET /api/v1/auctions/{id}`), que ya funciona con los endpoints actuales.

WebSockets es diferencial positivo en la nota de arquitectura. Se puede implementar con `Microsoft.AspNetCore.SignalR`, enviando un evento `NewBid` desde `PlaceBidHandler` al hub.

### 6. README.md con instrucciones de entrega `[Media · requisito de entrega]`

El §4.1 exige un `README.md` con:

- Pasos para compilar, levantar la BD y ejecutar migraciones
- Cómo lanzar la aplicación
- Documentación de la prueba de concurrencia (puede referenciar el `stress-puja.ps1` existente)

El script de stress ya existe; falta el README que lo documente y explique todo el setup.

### 7. Tests unitarios de Application `[Baja · calidad de código]`

Existe el proyecto `SubastaYa.Application.UnitTests` (csproj generado) pero no contiene ningún archivo de test todavía. El criterio de evaluación de calidad de código (2 pts) se beneficia de tener al menos tests para `PlaceBidHandler` (el caso más crítico) y la lógica de dominio `Subasta.AplicarAntiSniping()`.

---

## Resumen priorizado de tareas

| # | Tarea | Puntos del TP | Prioridad | Estado |
|---|---|---|---|---|
| 1 | WalletController — balance, deposit, transactions | 3 pts (API + Lógica) | Alta | ✅ Completo |
| 2 | AdjudicacionWorker — Background Service periódico | 3 pts (Lógica negocio) | Alta | ✅ Completo |
| 3 | Endpoints "Mis Actividades" — bids y auctions por usuario | 3 pts (API) | Alta | ✅ Completo |
| 4 | Completar Seed Data — pujas previas + ledger de $45.000 | 2 pts (BD + Arquitectura) | Alta | ✅ Completo |
| 5 | Audit Log — eventos del Worker y acreditaciones de saldo | 3 pts (Lógica) | Alta | ✅ Completo |
| 6 | Autenticación básica (JWT) + middleware de usuario | Calidad / Correctitud | Media | ⚠️ Pendiente |
| 7 | `GET /api/categories` — listado de categorías | 2 pts (API) | Media | ✅ Completo |
| 8 | README.md — setup + stress test documentado | Requisito de entrega | Media | ❌ Pendiente |
| 9 | Tests unitarios de Application (PlaceBidHandler, dominio) | 2 pts (Calidad) | Baja | ❌ Pendiente |
| 10 | WebSockets / SignalR para sala de subasta en vivo | Diferencial de nota | Baja | ❌ Pendiente |

### Progreso

| Métrica | Valor |
|---|---|
| Tareas completadas | 7 / 10 |
| Puntos de API/Lógica recuperados | ~17 pts |
| Pendientes con impacto en nota | Auth JWT (#6), README (#8), Tests (#9) |
| Diferencial opcional | WebSockets (#10) |

---

*Análisis generado automáticamente el 14/09/2026 · SubastaYa Backend · branch `main`*
*Última actualización: 15/09/2026 · branch `develop`*
