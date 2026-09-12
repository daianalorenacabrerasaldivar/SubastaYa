# Copilot Instructions

## Directrices del Proyecto
- **Patrón Result:** Preferir utilizar el patrón Result (`Result<T>`) en lugar de lanzar excepciones para representar errores esperables de aplicación (ej. entidad no encontrada, validaciones de saldo insuficiente o montos inválidos).
- **MediatR / CQRS:** Para los endpoints de la API, preferir usar MediatR enviando `Commands` o `Queries` desde los controladores, en lugar de inyectar y llamar directamente a los servicios.
- **Clean Architecture:** Mantener el aislamiento estricto de capas:
  - `Domain`: Entidades ricas, invariantes y reglas de negocio puras (sin dependencias a frameworks)[cite: 1].
  - `Application`: Commands, Queries, Handlers, DTOs y validaciones con FluentValidation.
  - `Infrastructure`: Implementación de EF Core, DbContext, Migraciones, Repositorios y Background Services[cite: 1].
  - `Api`: Controladores delgados que solo despachan a MediatR, mapean códigos HTTP según el Result y documentan OpenAPI[cite: 1].

## Reglas de Negocio Críticas (SubastaYa)
1. **Manejo Atómico de Garantías (Escrow):** Toda puja válida debe congelar el saldo del nuevo postor y liberar la retención del postor anterior en un único bloque transaccional atómico[cite: 1]. Registrar cada movimiento en el Ledger[cite: 1].
2. **Regla Anti-Sniping:** Si una oferta válida se registra dentro de los últimos 60 segundos antes del cierre (`fecha_fin`), extender automáticamente la subasta por 2 minutos adicionales y registrar el evento en la auditoría[cite: 1].
3. **Concurrencia Optimista (Optimistic Locking):** Las entidades `Subasta` y `Billetera` deben contar con una propiedad `Version` configurada para concurrencia en EF Core[cite: 1]. Ante colisiones, responder con `HTTP 409 Conflict`[cite: 1].
4. **Background Worker:** Implementar un servicio en segundo plano para verificar periódicamente subastas vencidas y liquidarlas atómicamente (`FINALIZADA` transfiriendo saldo al vendedor, o `DESIERTA` si no tuvo ofertas)[cite: 1].
5. **Auditoría Inmutable:** Registrar eventos críticos en la tabla `AuditoriaLog` (cambios de estado por worker, extensiones anti-sniping, acreditaciones y rechazos)[cite: 1].

## Estándares Técnicos y Convenciones
- **Endpoints RESTful:** Usar exclusivamente sustantivos plurales en minúsculas y jerarquías (`api/v1/auctions`, `api/v1/auctions/{id}/bids`)[cite: 1]. Prohibido usar verbos en las rutas[cite: 1].
- **Códigos de Estado HTTP:**
  - `200 OK` / `201 Created` para operaciones exitosas[cite: 1].
  - `400 Bad Request` ante errores de formato o validaciones estructurales[cite: 1].
  - `404 Not Found` cuando el recurso no existe.
  - `409 Conflict` ante conflictos de estado o concurrencia optimista (`DbUpdateConcurrencyException`)[cite: 1].
  - `422 Unprocessable Entity` cuando el saldo disponible sea insuficiente para la puja[cite: 1].
- **Documentación OpenAPI:** Todo endpoint debe contener comentarios XML descriptivos (`<summary>`, `<remarks>`, `<param>`, `<response>`) y atributos `[ProducesResponseType]` para todos los códigos HTTP posibles[cite: 1].
- **Clean Code:** Nomenclatura descriptiva en español o inglés consistente, funciones pequeñas y métodos orientados al principio de responsabilidad única (SRP)[cite: 1].