# SubastaYa — Backend API

Backend REST para el sistema de subastas en línea desarrollado en el marco del Proyecto de Software — Universidad Nacional.

**Stack:** ASP.NET Core 8 · Entity Framework Core 8 · SQL Server · MediatR · SignalR · JWT

---

## Requisitos previos

| Herramienta | Versión mínima |
|---|---|
| .NET SDK | 8.0 |
| SQL Server | 2019 / Express / LocalDB |
| Git | cualquier versión reciente |

---

## Configuración inicial

### 1. Clonar el repositorio

```bash
git clone https://github.com/daianalorenacabrerasaldivar/SubastaYa.git
cd SubastaYa/backend
```

### 2. Cadena de conexión

La API lee la conexión desde `src/Api/appsettings.Development.json`. El archivo incluido usa Windows Authentication (Integrated Security) apuntando a `localhost`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SubastaYaDb;Integrated Security=True;TrustServerCertificate=True;"
  }
}
```

Si tu instancia de SQL Server tiene nombre diferente (ej. `localhost\SQLEXPRESS`) editá la línea `Server=`.

### 3. Migraciones y seed

Las migraciones y el seed se aplican **automáticamente** al arrancar en modo `Development`. No hace falta correr comandos manuales de EF.

Si necesitás aplicarlos manualmente:

```bash
cd src/Api
dotnet ef database update --project ../Infrastructure
```

---

## Ejecutar la API

```bash
cd src/Api
dotnet run --launch-profile http
```

La API queda disponible en:

- **HTTP:** `http://localhost:5073`
- **Swagger UI:** `http://localhost:5073/swagger`
- **WebSocket Hub:** `ws://localhost:5073/hubs/auctions`

---

## Autenticación

La API usa **JWT Bearer**. Todos los usuarios del seed comparten la contraseña `Password123!`.

### Login

```http
POST /api/v1/auth/login
Content-Type: application/json

{
  "email": "comprador1@test.com",
  "password": "Password123!"
}
```

**Respuesta:**
```json
{
  "usuarioId": 2,
  "nombre": "Comprador Uno",
  "email": "comprador1@test.com",
  "rol": "Comprador",
  "token": "eyJhbGci...",
  "expiraEn": "2026-09-17T..."
}
```

Incluí el token en todas las llamadas a endpoints protegidos:

```http
Authorization: Bearer eyJhbGci...
```

### Usuarios del seed

| Email | Rol | Total | Retenido | Disponible |
|---|---|---|---|---|
| `vendedor@test.com` | Vendedor | $0 | $0 | $0 |
| `comprador1@test.com` | Comprador | $150.000 | $45.000 | $105.000 |
| `comprador2@test.com` | Comprador | $200.000 | $0 | $200.000 |
| `sinfondos@test.com` | Comprador | $500 | $0 | $500 |

---

## Endpoints principales

| Método | Ruta | Auth | Descripción |
|---|---|---|---|
| `POST` | `/api/v1/auth/login` | No | Login → JWT |
| `GET` | `/api/v1/auctions` | No | Catálogo con filtros y paginación |
| `GET` | `/api/v1/auctions/{id}` | No | Detalle de subasta |
| `POST` | `/api/v1/auctions` | No | Crear subasta |
| `GET` | `/api/v1/auctions/{id}/bids` | No | Historial de pujas |
| `POST` | `/api/v1/auctions/{id}/bids` | No | Registrar puja (escrow + anti-sniping) |
| `GET` | `/api/v1/categories` | No | Listado de categorías |
| `GET` | `/api/v1/wallet/balance` | **Sí** | Saldo total / retenido / disponible |
| `GET` | `/api/v1/wallet/transactions` | **Sí** | Historial del Ledger |
| `POST` | `/api/v1/wallet/deposit` | **Sí** | Acreditar saldo ficticio |
| `GET` | `/api/v1/users/{id}/bids` | **Sí** | Mis Pujas |
| `GET` | `/api/v1/users/{id}/auctions` | **Sí** | Mis Subastas |

La documentación interactiva completa está en Swagger UI.

---

## WebSockets (SignalR)

El hub `/hubs/auctions` permite recibir notificaciones en tiempo real al registrar una puja.

**Conexión desde JavaScript:**
```javascript
const connection = new signalR.HubConnectionBuilder()
  .withUrl("http://localhost:5073/hubs/auctions", {
    accessTokenFactory: () => token
  })
  .build();

await connection.start();
await connection.invoke("UnirseASubasta", subastaId);

connection.on("NewBid", (data) => {
  console.log("Nueva puja:", data);
  // { subastaId, monto, cantidadOfertas, proximaMinima, fechaFin, extendida }
});
```

---

## Prueba de concurrencia (Optimistic Locking)

El script `stress-puja.ps1` simula pujas concurrentes sobre una subasta activa para demostrar que el Optimistic Locking con `rowversion` resuelve correctamente los conflictos:

```powershell
.\stress-puja.ps1
```

El script:
1. Lanza N peticiones paralelas de puja sobre la misma subasta
2. Muestra qué petición ganó el lock y cuáles recibieron HTTP 409 (Conflict)
3. Verifica que solo una puja por tick se persiste en la base de datos

También existen tests de integración en `SubastaYa.Application.IntegrationTests`:

```bash
cd src
dotnet test
```

---

## Estructura del proyecto

```
backend/
├── src/
│   ├── Api/                    # Controllers, Hubs, Program.cs
│   ├── Application/            # Use Cases (CQRS), Interfaces, DTOs
│   ├── Infrastructure/         # EF Core, Repos, Workers, JWT Service
│   └── Domain/                 # Entidades, Enums, Result Pattern
└── tests/
    └── SubastaYa.Application.IntegrationTests/
```

---

## Adjudicación automática

El `AdjudicacionWorker` (Background Service) corre cada **30 segundos** y:
- Activa subastas `PROGRAMADAS` cuya `FechaInicio` ya pasó
- Finaliza subastas `ACTIVAS` vencidas: debita comprador, acredita vendedor, escribe Ledger y AuditoriaLog
- Marca como `DESIERTAS` las subastas sin pujas

No requiere intervención manual.
