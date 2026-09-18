# SubastaYa — Backend

API REST en .NET 8 con Clean Architecture, JWT, EF Core y SignalR.

---

## Requisitos

| Herramienta | Versión mínima |
|---|---|
| .NET SDK | 8.0 |
| SQL Server | 2019 (o LocalDB) |

---

## Puesta en marcha

### 1. Configurar cadena de conexión

Editar `backend/src/Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SubastaYaDb;Integrated Security=true;"
  },
  "FrontendPath": "..\\..\\..\\..\\SubastaYa-Front"
}
```

> `FrontendPath` es la ruta al repositorio del frontend (relativa a `src/Api`). El backend lo sirve como archivos estáticos en el mismo puerto.

### 2. Aplicar migraciones y sembrar la BD

```bash
cd backend/src/Api
dotnet ef database update
```

El seeder carga automáticamente 4 usuarios, 4 categorías y 5 subastas si la BD está vacía.

### 3. Levantar la API

```bash
cd backend/src/Api
dotnet run --launch-profile http
```

- **API**: `http://localhost:5073`
- **Swagger UI**: `http://localhost:5073/swagger`
- **Frontend**: `http://localhost:5073` (servido por el mismo proceso)

---

## Arquitectura

```
backend/src/
├── Api/                 # Controllers, Program.cs, middlewares
├── Application/         # Use cases (CQRS con MediatR), DTOs, interfaces
├── Domain/              # Entidades, enums, lógica de negocio pura
└── Infrastructure/      # EF Core, repositorios, JWT, SignalR hub
```

**Dependencias entre capas**: `Api → Application → Domain ← Infrastructure`

---

## Autenticación

JWT Bearer. El token se obtiene con:

```
POST /api/v1/auth/login
{ "email": "...", "password": "..." }
```

Respuesta:
```json
{
  "usuarioId": 2,
  "nombre": "Comprador Uno",
  "email": "comprador1@test.com",
  "rol": "Comprador",
  "token": "<JWT>",
  "expiraEn": "2025-09-19T..."
}
```

Incluir el token en requests protegidos: `Authorization: Bearer <token>`

---

## Endpoints principales

| Método | Ruta | Auth | Descripción |
|---|---|---|---|
| `POST` | `/api/v1/auth/login` | No | Login y obtención de JWT |
| `GET` | `/api/v1/auctions` | No | Listado con filtros y paginación |
| `GET` | `/api/v1/auctions/{id}` | No | Detalle de subasta |
| `POST` | `/api/v1/auctions` | No* | Crear subasta |
| `GET` | `/api/v1/auctions/{id}/bids` | No | Historial de pujas |
| `POST` | `/api/v1/auctions/{id}/bids` | No* | Registrar puja |
| `GET` | `/api/v1/wallet/{userId}/balance` | Sí | Saldo de billetera |
| `GET` | `/api/v1/wallet/{userId}/transactions` | Sí | Movimientos |
| `POST` | `/api/v1/wallet/deposit` | Sí | Depositar crédito |
| `GET` | `/api/v1/users/{id}/bids` | Sí | Pujas del usuario |
| `GET` | `/api/v1/users/{id}/auctions` | Sí | Subastas publicadas por el usuario |
| `GET` | `/api/v1/categories` | No | Lista de categorías |

> \* Aún sin validación de rol en el endpoint — pendiente.

---

## Usuarios de prueba

Contraseña de todos: `Password123!`

| Email | Rol | Saldo disponible |
|---|---|---|
| `vendedor@test.com` | Vendedor | $7.500 |
| `comprador1@test.com` | Comprador | $105.000 |
| `comprador2@test.com` | Comprador | $180.500 |
| `sinfondos@test.com` | Comprador | $500 |

---

## Reglas de negocio

### Escrow (retención de fondos)

Al pujar, el monto queda **retenido** en la billetera (no disponible para otras pujas). Si el postor es superado, el monto se libera. Si gana, se convierte en un pago al vendedor.

### Anti-sniping

Si se registra una puja en los **últimos 2 minutos** antes del cierre, la fecha de fin se extiende automáticamente **+2 minutos**.

### Optimistic locking

Las tablas `Subasta` y `Billetera` tienen `rowversion`. Si dos pujas llegan al mismo tiempo, una recibe `HTTP 409 Conflict`. El cliente debe obtener el estado actualizado y reintentar.

---

## Prueba de concurrencia

```powershell
powershell -File backend/scripts/stress-puja.ps1 -BaseUrl http://localhost:5073 -AuctionId 1 -CompradorId 2 -Monto 50000
```

Resultado esperado: una línea `HTTP 201` y una `HTTP 409`.

Test de integración equivalente:
```bash
dotnet test backend/tests/Infrastructure/SubastaYa.Infrastructure.IntegrationTests
```

---

## Reiniciar seed

Si las subastas activas ya vencieron, borrar la BD y volver a migrar:

```bash
cd backend/src/Api
dotnet ef database drop --force
dotnet ef database update
```

---

## Repositorio

- **Backend + frontend**: https://github.com/daianalorenacabrerasaldivar/SubastaYa
- **Frontend (standalone)**: https://github.com/daianalorenacabrerasaldivar/SubastaYa-Front
