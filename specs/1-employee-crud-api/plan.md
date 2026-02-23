# Implementation Plan: Employee CRUD API

**Branch**: `1-employee-crud-api` | **Date**: 2026-02-23

## Summary
Build a small .NET 10 minimal Web API that implements Employee CRUD using EF Core. Use the InMemory provider for development and tests; structure the code for easy replacement with a production provider later. Provide OpenAPI/Swagger docs, centralized validation, and ProblemDetails-style error responses.

## Technical Context
- Runtime: .NET 10
- Language: C# 14
- Web framework: ASP.NET Core minimal APIs (lightweight endpoints)
- Data access: EF Core (InMemory for demo/tests; production: SQL Server/Postgres)
- API docs: Swashbuckle/Swagger (OpenAPI)
- Testing: xUnit + Microsoft.AspNetCore.Mvc.Testing + EF Core InMemory for integration tests
- Logging/Observability: Structured logging (Microsoft.Extensions.Logging), include correlation IDs

## Project Structure (recommended)

backend/
├── src/
│   ├── EmployeeApi/                # Minimal API app
│   │   ├── Program.cs              # Minimal API configuration
│   │   ├── Models/                 # EF Core entity models (Employee, optional RowVersion)
│   │   ├── DTOs/                   # Request/response DTOs
│   │   ├── Data/                   # DbContext, migrations (if using real provider)
│   │   ├── Services/               # Business logic and repository abstractions
│   │   ├── Validation/             # FluentValidation or custom validators
│   │   └── Middleware/             # Error handling, logging, correlation
│   └── EmployeeApi.Tests/          # Unit & integration tests
│       ├── Integration/
│       └── Unit/
├── README.md
└── .github/workflows/ci.yml

## Entities
Employee (EF Core entity)
- Id: Guid (PK, server-generated)
- FirstName: string (required, max 100)
- LastName: string (required, max 100)
- Email: string (required, max 320, unique)
- Salary: decimal (required, precision appropriate, >= 0)
- DateOfJoining: DateTime (required)
- (optional) RowVersion: byte[] (concurrency token)

DTOs:
- EmployeeReadDto: Id, FirstName, LastName, Email, Salary, DateOfJoining
- EmployeeCreateDto: FirstName, LastName, Email, Salary, DateOfJoining
- EmployeeUpdateDto: Id, FirstName, LastName, Email, Salary, DateOfJoining

## Validation Rules
- FirstName/LastName: required, non-empty, trimmed, max length 100
- Email: required, valid email pattern, trimmed, unique (check at data layer)
- Salary: required, decimal >= 0
- DateOfJoining: required, must be <= today
- Id: GUID format validated on routes

Validation approach:
- Use FluentValidation or custom validation services for DTOs.
- Surface validation errors as RFC7807 ProblemDetails with field-level `errors` object.

## API Endpoints (Minimal API mapping)
Base path: `/api/v1/employees`

- GET /api/v1/employees
  - Returns: 200 OK with paged list { items: EmployeeReadDto[], page, pageSize, total }
  - Errors: 400 Bad Request (invalid query params)

- GET /api/v1/employees/{id}
  - Returns: 200 OK with EmployeeReadDto
  - Errors: 400 Bad Request (invalid id), 404 Not Found

- POST /api/v1/employees
  - Accepts: EmployeeCreateDto
  - Returns: 201 Created, Location: /api/v1/employees/{id}, body: EmployeeReadDto
  - Errors: 400 Bad Request (validation), 409 Conflict (email exists)

- PUT /api/v1/employees/{id}
  - Accepts: EmployeeUpdateDto
  - Returns: 200 OK with updated EmployeeReadDto
  - Errors: 400 Bad Request, 404 Not Found, 409 Conflict, 422 Unprocessable Entity (business rule)

- DELETE /api/v1/employees/{id}
  - Returns: 204 No Content
  - Errors: 404 Not Found, 403 Forbidden (business rule)

HTTP semantics and status codes strictly follow the specification in spec.md.

## Data Access & Repository Pattern
- Implement `EmployeeDbContext` inheriting from `DbContext` with `DbSet<Employee>`.
- For simple apps, the minimal API can use the DbContext directly via DI; prefer a small `IEmployeeRepository` for testability and to encapsulate uniqueness checks.
- Use EF Core InMemory for tests and local demos. When moving to production, switch provider in `Program.cs` and add migrations.

## Error Handling
- Register a centralized exception/error middleware that converts exceptions to ProblemDetails.
- Validation errors -> 400 with ProblemDetails `errors` property.
- Not found -> 404 ProblemDetails.
- Unique constraint violation -> 409 with a typed problem `type: /probs/conflict/email-exists`.
- Unexpected errors -> 500 with generic ProblemDetails (no internal stack traces in body).

## Swagger / OpenAPI
- Add Swashbuckle and enable Swagger UI in development.
- Annotate DTOs and endpoints for clear request/response examples and schemas.
- Expose `swagger.json` for automation and contract testing.

## Concurrency & Consistency
- Consider optimistic concurrency via `RowVersion` (byte[]) if concurrent updates are expected; detect concurrency exceptions and return 409 Conflict.

## Testing Strategy
- Unit tests: validators, services, repo logic with in-memory or mocks.
- Integration tests: use `WebApplicationFactory<T>` with EF Core InMemory provider to exercise the full pipeline (endpoints, validation, DB behavior).
- Contract tests: validate generated OpenAPI spec matches expectations.

## Security & Prod Notes (out of scope for demo)
- Add authentication/authorization middleware for production.
- Use a production DB provider and create EF Core migrations.
- Configure secrets for DB connection strings and other secrets via environment variables or secret store.

## Developer Quickstart
1. Create project:

```powershell
dotnet new web -o EmployeeApi --framework net10.0
cd EmployeeApi
dotnet add package Microsoft.EntityFrameworkCore.InMemory
dotnet add package Swashbuckle.AspNetCore
dotnet add package FluentValidation.AspNetCore
```

2. Scaffold minimal `Program.cs`, register `EmployeeDbContext` with InMemory, add endpoints, run:

```powershell
dotnet run
# Open https://localhost:5001/swagger
```

## Deliverables
- `src/EmployeeApi` with minimal API implementation and DI configuration
- `src/EmployeeApi.Tests` with unit and integration tests using InMemory
- OpenAPI/Swagger documentation enabled
- README with setup and run instructions

## Risks & Mitigations
- Risk: InMemory provider behavior differs from relational DBs. Mitigation: include integration tests against a lightweight relational provider (SQLite in-memory) before production cutover.
- Risk: Email uniqueness race conditions. Mitigation: enforce uniqueness at DB layer in production and handle conflicts gracefully.

---

**Plan status**: Ready — proceed to implementation (scaffold project and implement endpoints).