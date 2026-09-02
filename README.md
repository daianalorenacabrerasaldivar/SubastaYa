# SubastaYa - Plataforma de Subastas en Tiempo Real

![.NET](https://img.shields.io/badge/.NET-8.0-blue) ![License](https://img.shields.io/badge/license-MIT-green)

## Descripción General

**SubastaYa** es una plataforma de subastas en tiempo real diseñada con arquitectura **Clean Architecture** bajo modelo **Monorepo**. El sistema implementa mecanismos avanzados de **fair play** (Escrow, Anti-Sniping, Optimistic Locking) garantizando transacciones seguras, auditoría inmutable y manejo atómico de saldos.

### Características Principales

- 🔄 **Subastas en Tiempo Real**: Actualización instantánea de pujas con WebSocket (roadmap)
- 🛡️ **Sistema de Escrow**: Retención de fondos durante subastas para garantizar solvencia
- ⏱️ **Anti-Sniping**: Extensión automática de fecha de cierre si hay pujas en los últimos 60 segundos
- 🔒 **Optimistic Locking**: Campo `version` previene condiciones de carrera (HTTP 409)
- 📝 **Auditoría Inmutable**: Registro de toda operación en tabla `AuditoriaLog`
- 👥 **Gestión de Usuarios**: Roles (vendedor, comprador), billetes virtuales
- 📊 **Documentación OpenAPI/Swagger**: API completamente documentada con comentarios XML

---

## Arquitectura

### Estructura de Carpetas (Clean Architecture)

```
SubastaYa/
├── backend/                           # Aplicación Backend (.NET 8)
│   ├── src/
│   │   ├── Api/                       # Capa de Presentación (Controllers)
│   │   │   ├── Controllers/
│   │   │   │   ├── AuctionsController.cs
│   │   │   │   └── WeatherForecastController.cs
│   │   │   ├── Program.cs             # Configuración Swagger/DI
│   │   │   └── Api.csproj
│   │   ├── Application/               # Capa de Aplicación (DTOs, Servicios)
│   │   │   ├── Dtos/
│   │   │   │   └── AuctionDtos.cs
│   │   │   ├── Interfaces/
│   │   │   │   └── ISubastaService.cs
│   │   │   └── Application.csproj
│   │   ├── Domain/                    # Capa de Dominio (Entidades, Enums)
│   │   │   ├── Entities/
│   │   │   │   ├── Auction.cs
│   │   │   │   ├── Bid.cs
│   │   │   │   ├── Billetera.cs
│   │   │   │   └── AuditoriaLog.cs
│   │   │   ├── Interfaces/
│   │   │   │   └── IAuctionRepository.cs
│   │   │   └── Domain.csproj
│   │   └── Infrastructure/            # Capa de Infraestructura (BD, Repos)
│   │       ├── Persistence/
│   │       └── Infrastructure.csproj
│   ├── SubastaYa.sln
│   └── stress-test.sh                 # Script de prueba de concurrencia
├── frontend/                          # Aplicación Frontend (React/Vite)
│   ├── src/
│   ├── package.json
│   └── ...
└── README.md

```

### Diagrama de Capas (Clean Architecture)

```
┌─────────────────────────────────────┐
│   Presentation Layer (Api)          │  Controllers, ViewModels
│   - AuctionsController              │  Atributos HTTP, Validación
├─────────────────────────────────────┤
│   Application Layer (Application)   │  DTOs, Servicios, Use Cases
│   - ISubastaService                 │  Lógica de negocio orquestada
│   - CreateAuctionUseCase            │
├─────────────────────────────────────┤
│   Domain Layer (Domain)             │  Entidades, Value Objects, Enums
│   - Auction, Bid, Billetera         │  Interfaces de Repositorio
│   - AuctionStatus, AuditoriaLog     │  Lógica de negocio pura
├─────────────────────────────────────┤
│   Infrastructure Layer (Infra)      │  EF Core, DbContext
│   - AuctionRepository               │  Base de Datos, Migraciones
│   - AppDbContext                    │  Implementación de Interfaces
└─────────────────────────────────────┘
```

**Ventajas del Monorepo**:
- Backend y Frontend comparten configuración de versionado (Git)
- Fácil coordinación de cambios entre capas
- Reutilización de enums/DTOs si es necesario

---

## Requisitos Previos

### Versiones Requeridas

| Componente        | Versión      | Notas                                |
|-------------------|--------------|--------------------------------------|
| .NET SDK          | 8.0+         | Descargar desde dotnet.microsoft.com |
| SQL Server        | 2019+        | LocalDB o Docker                     |
| Node.js           | 18.0+        | NPM incluido                         |
| Docker (opcional) | Latest       | Para ejecutar SQL Server sin instalar |
| Git               | 2.30+        | Control de versiones                 |

### Instalación

1. **Clonar el repositorio**:
   ```bash
   git clone https://github.com/daianalorenacabrerasaldivar/SubastaYa.git
   cd SubastaYa
   ```

2. **Verificar .NET**:
   ```bash
   dotnet --version
   # Debe mostrar: 8.0.xxx
   ```

3. **Verificar Node.js**:
   ```bash
   node --version  # v18.x.x
   npm --version   # 9.x.x
   ```

---

## Puesta en Marcha Paso a Paso

### Paso 1: Levantar Base de Datos

#### Opción A: Docker (Recomendado)

```bash
# Crear y ejecutar contenedor SQL Server
docker run -e "ACCEPT_EULA=Y" \
           -e "SA_PASSWORD=YourPassword123!" \
           -p 1433:1433 \
           --name sqlserver-subasya \
           -d mcr.microsoft.com/mssql/server:2022-latest

# Verificar que el contenedor está corriendo
docker ps | grep sqlserver-subasya
```

#### Opción B: SQL Server Local (Windows)

Si tienes SQL Server instalado localmente, la cadena de conexión ya está configurada en `appsettings.json`.

### Paso 2: Configurar Cadena de Conexión

Edita `backend/src/Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=SubastaYa;User Id=sa;Password=YourPassword123!;TrustServerCertificate=true;"
  }
}
```

Para **Windows Auth** en SQL Server local:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SubastaYa;Integrated Security=true;"
  }
}
```

### Paso 3: Restaurar Dependencias Backend

```bash
cd backend
dotnet restore

# Alternativa: restauración automática
dotnet build
```

### Paso 4: Aplicar Migraciones (Code-First)

```bash
cd backend

# Ver migraciones pendientes
dotnet ef migrations list

# Crear base de datos y aplicar todas las migraciones
dotnet ef database update

# Output esperado:
# Applying migration 'InitialCreate'...
# Applying migration 'AddBilletera'...
# Done.
```

### Paso 5: Ejecutar Backend API

```bash
cd backend/src/Api

dotnet run
# O con hot-reload:
# dotnet watch run

# Output esperado:
# info: Microsoft.Hosting.Lifetime[14]
#      Now listening on: https://localhost:5001
#      Now listening on: http://localhost:5000
```

**Swagger UI disponible en**: `http://localhost:5000`

### Paso 6: Ejecutar Frontend

En otra terminal:

```bash
cd frontend

# Instalar dependencias
npm install

# Desarrollo con hot reload
npm run dev
# O
npm run develop

# Output esperado:
#   Local:   http://localhost:5173
#   press h to show help
```

**Frontend disponible en**: `http://localhost:5173`

### Paso 7: Verificar Conectividad

```bash
# Verificar API
curl -i http://localhost:5000/api/v1/auctions

# Verificar Swagger
open http://localhost:5000/swagger

# Verificar Frontend
open http://localhost:5173
```

---

## Datos de Semilla (Seed Data)

Al ejecutar `dotnet ef database update`, se cargan automáticamente datos iniciales en la base de datos:

### Usuarios Precargados

| Email               | Rol       | Saldo Inicial | Saldo Retenido | Notas                          |
|---------------------|-----------|---------------|----------------|--------------------------------|
| vendedor@test.com   | Vendedor  | $1,000.00     | $0.00          | Crea las 5 subastas de prueba  |
| comprador1@test.com | Comprador | $150,000.00   | $45,000.00     | Pujador activo, con retención  |
| comprador2@test.com | Comprador | $200,000.00   | $0.00          | Solvente                       |
| sinfondos@test.com  | Comprador | $500.00       | $0.00          | Presupuesto limitado (prueba)  |

### Subastas de Prueba

| ID | Título                           | Estado      | Precio Salida | Incremento | Fin Esperado      | Ganador         | Motivo Prueba                |
|----|----------------------------------|-------------|---------------|------------|-------------------|-----------------|------------------------------|
| 1  | Laptop Dell XPS 13 - Prueba      | ACTIVA      | $500.00       | $10.00     | Hoy + 24h         | comprador1      | Estándar, sin anti-sniping   |
| 2  | Teléfono iPhone 15 Pro - Crítico | ACTIVA      | $1,200.00     | $50.00     | Hoy + 2h 59m      | comprador2      | Anti-sniping activo (<60s)   |
| 3  | Tablet Samsung Galaxy Tab S9     | PROGRAMADA  | $300.00       | $5.00      | Mañana + 12h      | (ninguno)       | Inicia mañana                |
| 4  | Monitor LG UltraWide - Ganada    | FINALIZADA  | $400.00       | $20.00     | Ayer 15:30 UTC    | comprador1      | Con ganador y puja retenida  |
| 5  | Teclado Mecánico RGB - Desierta | FINALIZADA  | $150.00       | $5.00      | Ayer 10:00 UTC    | (ninguno)       | Sin pujas, saldo retornado   |

**Importancia de Seed Data**:
- Valida migraciones correctamente
- Proporciona escenarios de prueba realistas
- Simula comportamientos de usuarios reales
- Facilita testing de endpoints

---

## Prueba de Concurrencia (Stress Test - Optimistic Locking)

### ¿Qué es el Optimistic Locking?

El campo `Version`/`RowVersion` en la entidad `Auction` previene **race conditions**:

1. Cliente A obtiene Subasta (version=AQAAAA==)
2. Cliente B obtiene Subasta (version=AQAAAA==)
3. Cliente A hace puja → version→AQAAAQ== ✓
4. Cliente B intenta hacer puja → HTTP 409 Conflict ❌ (su version es vieja)
5. Cliente B reintenta obteniendo la subasta actualizada → ✓

**Resultado**: Imposible perder una puja válida, garantía de atomicidad.

### Script de Stress Test (Bash)

Crea un archivo `stress-test.sh` en `backend/`:

```bash
#!/bin/bash

# Stress Test: Dos clientes pujan simultáneamente en la misma subasta
# Esperado: Una puja exitosa (200), una con conflicto (409)

AUCTION_ID=2
API_URL="http://localhost:5000/api/v1/auctions/${AUCTION_ID}/bids"

echo "===== SubastaYa Stress Test: Optimistic Locking ====="
echo "Escenario: Dos postores pujan en el mismo milisegundo"
echo "Subasta ID: $AUCTION_ID"
echo "API: $API_URL"
echo ""

# Obtener datos actuales de la subasta
echo "[1] Obteniendo datos de subasta..."
AUCTION=$(curl -s "$API_URL/../$AUCTION_ID")
CURRENT_PRICE=$(echo "$AUCTION" | grep -o '"currentPrice":[^,]*' | cut -d: -f2)
MIN_INCREMENT=$(echo "$AUCTION" | grep -o '"minimumBidIncrement":[^,]*' | cut -d: -f2)

NEW_BID=$(echo "$CURRENT_PRICE + $MIN_INCREMENT + 50" | bc)
echo "Precio actual: $CURRENT_PRICE"
echo "Incremento mínimo: $MIN_INCREMENT"
echo "Puja A (comprador1): $NEW_BID"
echo "Puja B (comprador2): $(echo $NEW_BID + 100 | bc)"
echo ""

# Payload para Puja A
PAYLOAD_A='{
  "amount": '"$NEW_BID"',
  "bidderId": 2
}'

# Payload para Puja B
PAYLOAD_B='{
  "amount": '"$(echo $NEW_BID + 100 | bc)"',
  "bidderId": 3
}'

echo "[2] Iniciando pujas simultáneas..."
echo ""

# Enviar ambas solicitudes en paralelo
echo "Enviando Puja A (comprador1, monto: $NEW_BID)..."
curl -X POST "$API_URL" \
  -H "Content-Type: application/json" \
  -d "$PAYLOAD_A" \
  -w "\nHTTP Status: %{http_code}\n" \
  -s &
PID_A=$!

# Pequeño delay (<1ms) para simular simultáneo
sleep 0.001

echo "Enviando Puja B (comprador2, monto: $(echo $NEW_BID + 100 | bc))..."
curl -X POST "$API_URL" \
  -H "Content-Type: application/json" \
  -d "$PAYLOAD_B" \
  -w "\nHTTP Status: %{http_code}\n" \
  -s &
PID_B=$!

# Esperar ambas solicitudes
wait $PID_A $PID_B

echo ""
echo "===== Resultado ====="
echo "✓ Una puja debe haber sido aceptada (200 OK)"
echo "✓ Otra puja debe haber sido rechazada (409 Conflict)"
echo ""
echo "Si ambas retornan 409, reintentar:"
echo "  1. Obtener subasta actualizada: GET /api/v1/auctions/$AUCTION_ID"
echo "  2. Extraer 'version' actual del JSON"
echo "  3. Incluir 'version' en siguiente POST /bids"
echo ""
```

### Ejecución del Stress Test

```bash
# Dar permisos de ejecución
chmod +x backend/stress-test.sh

# Ejecutar (asegúrate de que la API está corriendo)
cd backend
./stress-test.sh

# Output esperado:
# ===== SubastaYa Stress Test: Optimistic Locking =====
# Escenario: Dos postores pujan en el mismo milisegundo
# Subasta ID: 2
# ...
# HTTP Status: 200
# HTTP Status: 409
# 
# ===== Resultado =====
# ✓ Una puja debe haber sido aceptada (200 OK)
# ✓ Otra puja debe haber sido rechazada (409 Conflict)
```

### Interpretación de Resultados

| Status | Significado                                       | Acción                                    |
|--------|---------------------------------------------------|-------------------------------------------|
| 200    | Puja exitosa, saldo retenido, auditoría registrada| ✓ Puja válida procesada                  |
| 409    | Conflicto de versión, otro cliente actualizó     | Obtener subasta actualizada y reintentar |
| 400    | Datos inválidos (monto, subasta no activa)      | Verificar parámetros de solicitud         |
| 404    | Subasta no encontrada                            | Verificar ID de subasta                   |
| 422    | Saldo insuficiente (Escrow)                      | Usuario debe depositar fondos             |

---

## Resumen de Reglas de Negocio

### 1. Manejo Atómico de Saldos (Escrow)

- Al hacer una puja, se **retiene** la cantidad en la billetera del comprador
- No se puede transferir fondos retenidos hasta resolución de subasta
- Si gana: fondos se transfieren al vendedor
- Si pierde: fondos se liberan automáticamente

**Ejemplo**:
```
Comprador1 saldo: $150,000 (disponible)
  ↓ Puja $5,000 en Subasta #1
Comprador1 saldo: $145,000 (disponible) + $5,000 (retenido)
  ↓ Subasta #1 finaliza (Comprador1 gana)
Vendedor recibe: +$5,000
Comprador1 paga: $5,000 de retenido
```

### 2. Anti-Sniping (+2 Minutos)

- Si hay puja en los **últimos 60 segundos** antes de fin de subasta
- La fecha de **EndDate se extiende automáticamente +2 minutos**
- Iguala posibilidades entre compradores en último momento

**Timeline**:
```
18:30 - Subasta Inicia (Fin = 19:00)
19:00 - 60s = 18:59:00 ← Punto crítico
18:59:30 → Puja registrada
        → EndDate se extiende a 19:02:00
19:02:00 - Subasta cierra definitivamente
```

### 3. Background Worker (Roadmap)

- Servicio de Windows/Linux que verifica subastas expiradas cada 5 minutos
- Calcula ganador, transfiere fondos, genera reporte
- Marcado automático como FINALIZADA o DESIERTA

### 4. Auditoría Inmutable

- **Tabla `AuditoriaLog`**: Cada operación genera registro inmutable
- Campos: `UsuarioId, Accion, Entidad, ValorAnterior, ValorNuevo, Timestamp`
- No se puede modificar/eliminar, solo consultar
- Facilita trazabilidad y cumplimiento normativo

**Operaciones Auditadas**:
- ✓ Creación de subasta
- ✓ Registro de puja
- ✓ Extensión anti-sniping
- ✓ Finalización de subasta
- ✓ Transacciones de billetera

---

## Configuración de Swagger/OpenAPI

### Acceso a Swagger UI

```
http://localhost:5000/swagger
```

### Endpoints Documentados

#### **GET /api/v1/auctions**
Listado paginado de subastas con filtros

**Query Parameters**:
- `status`: PROGRAMADA, ACTIVA, FINALIZADA, DESIERTA
- `categoryId`: Filtro por categoría
- `precioMin`, `precioMax`: Rango de precios
- `pageNumber` (default: 1), `pageSize` (default: 10)
- `orderBy` (default: "startDate"), `sortDirection` (default: "asc")

**Respuesta (200)**:
```json
{
  "totalCount": 5,
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 1,
  "items": [
    {
      "id": 1,
      "title": "Laptop Dell XPS 13",
      "description": "Laptop de alta gama",
      "startingPrice": 500.00,
      "currentPrice": 1250.00,
      "status": "ACTIVA",
      "startDate": "2024-01-15T10:00:00Z",
      "endDate": "2024-01-22T10:00:00Z",
      "categoryId": 2,
      "leadingBidderId": 2
    }
  ]
}
```

#### **GET /api/v1/auctions/{id}**
Detalle completo de una subasta

**Response (200)**:
```json
{
  "id": 1,
  "title": "Laptop Dell XPS 13",
  "description": "Laptop de 13 pulgadas, 16GB RAM, SSD 512GB",
  "startingPrice": 500.00,
  "minimumBidIncrement": 10.00,
  "currentPrice": 1250.00,
  "status": "ACTIVA",
  "startDate": "2024-01-15T10:00:00Z",
  "endDate": "2024-01-22T10:00:00Z",
  "timeRemaining": "5 días, 3 horas, 42 minutos",
  "categoryId": 2,
  "leadingBidderId": 2,
  "createdByUserId": 1,
  "createdAt": "2024-01-15T09:55:00Z",
  "bidHistory": [
    {
      "id": 1,
      "bidderId": 2,
      "amount": 600.00,
      "timestamp": "2024-01-15T11:30:00Z"
    },
    {
      "id": 2,
      "bidderId": 3,
      "amount": 1250.00,
      "timestamp": "2024-01-15T14:15:00Z"
    }
  ]
}
```

#### **POST /api/v1/auctions**
Crear nueva subasta

**Request Body**:
```json
{
  "title": "Laptop Dell XPS 13",
  "description": "Laptop de 13 pulgadas, 16GB RAM, SSD 512GB",
  "startingPrice": 500.00,
  "minimumBidIncrement": 10.00,
  "startDate": "2024-01-15T10:00:00Z",
  "endDate": "2024-01-22T10:00:00Z",
  "categoryId": 2,
  "createdByUserId": 1
}
```

**Response (201)**:
```json
{
  "id": 10,
  "title": "Laptop Dell XPS 13",
  "description": "Laptop de 13 pulgadas...",
  "startingPrice": 500.00,
  "currentPrice": 500.00,
  "status": "PROGRAMADA",
  "startDate": "2024-01-15T10:00:00Z",
  "endDate": "2024-01-22T10:00:00Z",
  "categoryId": 2,
  "leadingBidderId": null
}
```

**Error (400)** - Validaciones:
- Título vacío
- Precios <= 0
- EndDate <= StartDate
- CategoryId o CreatedByUserId inválidos

#### **POST /api/v1/auctions/{id}/bids**
Registrar puja (ENDPOINT CRÍTICO - Optimistic Locking)

**Request Body**:
```json
{
  "amount": 1500.00,
  "bidderId": 2
}
```

**Response (200) - Puja Exitosa**:
```json
{
  "bidId": 42,
  "auctionId": 1,
  "bidderId": 2,
  "amount": 1500.00,
  "timestamp": "2024-01-15T16:45:30Z",
  "newEndDate": "2024-01-22T10:02:00Z",
  "antiSnipingApplied": true,
  "newBalance": 148500.00,
  "retainedBalance": 1500.00
}
```

**Response (409) - Conflicto Concurrencia**:
```json
{
  "message": "Conflicto de concurrencia: otro usuario ha modificado la subasta recientemente. Intente nuevamente."
}
```
→ **ACCIÓN**: Obtener subasta actualizada via GET /api/v1/auctions/{id} y reintentar

**Response (422) - Saldo Insuficiente**:
```json
{
  "message": "Saldo insuficiente: requiere $1500 pero disponible es $800"
}
```

**Response (400) - Datos Inválidos**:
```json
{
  "message": "El monto de la puja debe ser mayor al precio actual ($1250) + incremento mínimo ($10)"
}
```

---

## Estructura de Códigos HTTP Posibles

| Método | Endpoint                    | 200 | 201 | 400 | 404 | 409 | 422 | 500 |
|--------|-----------------------------|----|-----|-----|-----|-----|-----|-----|
| GET    | /api/v1/auctions            | ✓  |     | ✓   |     |     |     | ✓   |
| GET    | /api/v1/auctions/{id}       | ✓  |     | ✓   | ✓   |     |     | ✓   |
| POST   | /api/v1/auctions            |    | ✓   | ✓   |     |     |     | ✓   |
| POST   | /api/v1/auctions/{id}/bids  | ✓  |     | ✓   | ✓   | ✓   | ✓   | ✓   |

---

## Configuración del Proyecto para Próximas Etapas

### Autenticación (Roadmap)

```csharp
// Program.cs
services.AddJwtBearer(options => {
    options.Authority = "https://auth-server";
    options.Audience = "subasya-api";
});
```

### Autorización por Roles

```csharp
// AuctionsController
[Authorize(Roles = "Vendedor")]
[HttpPost]
public async Task<ActionResult> CreateAuction(...)
```

### WebSocket para Actualizaciones en Tiempo Real

```csharp
// Roadmap: SignalR Hub
app.MapHub<AuctionHub>("/hubs/auction");
```

---

## Troubleshooting

### Error: "Cannot connect to database"

```bash
# Verificar SQL Server está corriendo
docker ps | grep sqlserver
# O verificar LocalDB:
sqllocaldb i

# Si falta, crear instancia:
sqllocaldb create subasya
```

### Error: "Migrations pending"

```bash
cd backend
dotnet ef database update
```

### Error: "Port 5000/5001 already in use"

```bash
# Windows
netstat -ano | findstr :5000
taskkill /PID <PID> /F

# Linux/Mac
lsof -i :5000
kill -9 <PID>
```

### Swagger UI no muestra documentación XML

- Verificar que `Api.csproj` tiene `<GenerateDocumentationFile>true</GenerateDocumentationFile>`
- Compilar proyecto: `dotnet build`
- Archivo XML debe estar en `bin/Debug/net8.0/Api.xml`

---

## Contribuciones

Las contribuciones son bienvenidas. Para cambios significativos:

1. Fork el repositorio
2. Crea una rama: `git checkout -b feature/MiCaracteristica`
3. Commit: `git commit -m 'Add: Nueva característica'`
4. Push: `git push origin feature/MiCaracteristica`
5. Abre Pull Request

---

## Licencia

Este proyecto está bajo licencia **MIT**. Ver archivo [LICENSE](LICENSE) para más detalles.

---

## Contacto

- **Desarrolladores**: [Daiana Lorena Cabrera Saldivar](https://github.com/daianalorenacabrerasaldivar)
- **Email Soporte**: soporte@subasyta.local
- **Repositorio**: https://github.com/daianalorenacabrerasaldivar/SubastaYa

---

## Referencias y Documentación

- [Clean Architecture - Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [ASP.NET Core Official Docs](https://docs.microsoft.com/en-us/aspnet/core/)
- [EF Core Database Migrations](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- [Swagger/OpenAPI Specification](https://swagger.io/specification/)
- [Optimistic Locking Pattern](https://en.wikipedia.org/wiki/Optimistic_locking)

---

**Última actualización**: 15 de Enero de 2024
**Versión de Documentación**: 1.0
**Aplicable a**: SubastaYa v1.0 (.NET 8.0)
