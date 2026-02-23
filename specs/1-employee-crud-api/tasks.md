---
description: "Task list for Employee CRUD API implementation"
---

# Tasks: Employee CRUD API

**Input**: `specs/1-employee-crud-api/spec.md`, `specs/1-employee-crud-api/plan.md`

## Phase 1: Setup (Shared Infrastructure)

- [ ] T001 [P] Create project scaffold `backend/src/EmployeeApi` (dotnet project files and folder layout)
- [ ] T002 [P] Initialize .NET 10 minimal API in `backend/src/EmployeeApi/Program.cs` (use `dotnet new web` and basic Program.cs)
- [ ] T003 [P] Add packages in `backend/src/EmployeeApi/` (`Microsoft.EntityFrameworkCore.InMemory`, `Swashbuckle.AspNetCore`, `FluentValidation.AspNetCore`)
- [ ] T004 [P] Create test project scaffold `backend/src/EmployeeApi.Tests` and add test deps (xUnit, Microsoft.AspNetCore.Mvc.Testing)
- [ ] T005 [P] Add CI workflow file `.github/workflows/ci.yml` with build and test steps for .NET 10

---

## Phase 2: Foundational (Blocking Prerequisites)

- [ ] T006 Create EF Core `Employee` entity in `backend/src/EmployeeApi/Models/Employee.cs`
- [ ] T007 [P] Create `EmployeeDbContext` in `backend/src/EmployeeApi/Data/EmployeeDbContext.cs` and register `DbSet<Employee>`
- [ ] T008 [P] Create DTOs in `backend/src/EmployeeApi/DTOs/`:
  - `EmployeeReadDto.cs`, `EmployeeCreateDto.cs`, `EmployeeUpdateDto.cs`
- [ ] T009 Create repository interface `IEmployeeRepository` in `backend/src/EmployeeApi/Services/IEmployeeRepository.cs`
- [ ] T010 [P] Implement `EmployeeRepository` in `backend/src/EmployeeApi/Services/EmployeeRepository.cs` (use EF Core via `EmployeeDbContext`)
- [ ] T011 Create validators in `backend/src/EmployeeApi/Validation/`:
  - `EmployeeCreateValidator.cs`, `EmployeeUpdateValidator.cs` (FirstName/LastName/email/salary/date rules)
- [ ] T012 Create centralized error handling middleware in `backend/src/EmployeeApi/Middleware/ErrorHandlingMiddleware.cs`
- [ ] T013 [P] Create mapping helpers or AutoMapper profile in `backend/src/EmployeeApi/Mapping/EmployeeProfile.cs`
- [ ] T014 Implement DI registration and minimal API skeleton in `backend/src/EmployeeApi/Program.cs` (register DbContext, repository, validators, Swagger)
- [ ] T015 [P] Add logging + correlation ID middleware in `backend/src/EmployeeApi/Middleware/CorrelationIdMiddleware.cs`

**Checkpoint**: Foundational components ready — repositories, DbContext, DTOs, validation, error handling, and DI registered.

---

## Phase 3: User Story 1 - List Employees (Priority: P1) 🎯 MVP

**Goal**: Implement paged `GET /api/v1/employees` endpoint

- [ ] T016 [P] [US1] Implement `GET /api/v1/employees` endpoint in `backend/src/EmployeeApi/Endpoints/EmployeeEndpoints.cs` (or `Program.cs`)
- [ ] T017 [P] [US1] Add service method `GetPagedAsync` in `backend/src/EmployeeApi/Services/EmployeeService.cs` (or repository)
- [ ] T018 [P] [US1] Create integration test `backend/src/EmployeeApi.Tests/Integration/Employees_GetAll_Tests.cs`
- [ ] T019 [US1] Create unit test for pagination logic `backend/src/EmployeeApi.Tests/Unit/EmployeeRepository_Pagination_Tests.cs`

**Independent Test**: Run integration test to verify 200 OK with paged schema.

---

## Phase 4: User Story 2 - Get Employee by Id (Priority: P1)

**Goal**: Implement `GET /api/v1/employees/{id}`

- [ ] T020 [P] [US2] Implement `GET /api/v1/employees/{id}` endpoint in `backend/src/EmployeeApi/Endpoints/EmployeeEndpoints.cs`
- [ ] T021 [P] [US2] Add service method `GetByIdAsync` in `backend/src/EmployeeApi/Services/EmployeeService.cs`
- [ ] T022 [P] [US2] Create integration test `backend/src/EmployeeApi.Tests/Integration/Employees_GetById_Tests.cs` (success and 404)
- [ ] T023 [US2] Create unit test for repository `backend/src/EmployeeApi.Tests/Unit/EmployeeRepository_GetById_Tests.cs`

---

## Phase 5: User Story 3 - Create Employee (Priority: P1)

**Goal**: Implement `POST /api/v1/employees` with validation and uniqueness checks

- [ ] T024 [P] [US3] Implement `POST /api/v1/employees` endpoint in `backend/src/EmployeeApi/Endpoints/EmployeeEndpoints.cs`
- [ ] T025 [US3] Implement server-side uniqueness check and create logic in `backend/src/EmployeeApi/Services/EmployeeService.cs` or `EmployeeRepository.cs` (ensure 409 on email conflict)
- [ ] T026 [P] [US3] Create integration test `backend/src/EmployeeApi.Tests/Integration/Employees_Create_Tests.cs` (201 Location + body)
- [ ] T027 [US3] Create unit tests for validators `backend/src/EmployeeApi.Tests/Unit/Validators/EmployeeCreateValidator_Tests.cs`

---

## Phase 6: User Story 4 - Update Employee (Priority: P1)

**Goal**: Implement `PUT /api/v1/employees/{id}` with full replacement semantics and concurrency handling

- [ ] T028 [P] [US4] Implement `PUT /api/v1/employees/{id}` endpoint in `backend/src/EmployeeApi/Endpoints/EmployeeEndpoints.cs`
- [ ] T029 [US4] Implement update logic including optional concurrency token handling in `backend/src/EmployeeApi/Services/EmployeeService.cs`
- [ ] T030 [P] [US4] Create integration tests `backend/src/EmployeeApi.Tests/Integration/Employees_Update_Tests.cs` (success, 404, 409)
- [ ] T031 [US4] Create unit tests for business rules `backend/src/EmployeeApi.Tests/Unit/Employee_Update_BusinessRule_Tests.cs`

---

## Phase 7: User Story 5 - Delete Employee (Priority: P2)

**Goal**: Implement `DELETE /api/v1/employees/{id}` with forbidden-checks

- [ ] T032 [P] [US5] Implement `DELETE /api/v1/employees/{id}` endpoint in `backend/src/EmployeeApi/Endpoints/EmployeeEndpoints.cs`
- [ ] T033 [US5] Implement delete logic and business-rule checks in `backend/src/EmployeeApi/Services/EmployeeService.cs`
- [ ] T034 [P] [US5] Create integration tests `backend/src/EmployeeApi.Tests/Integration/Employees_Delete_Tests.cs` (204, 404, 403)
- [ ] T035 [US5] Create unit tests for delete guards `backend/src/EmployeeApi.Tests/Unit/Employee_Delete_Guard_Tests.cs`

---

## Phase 8: Polish & Cross-Cutting Concerns

- [ ] T036 [P] Add Swagger examples and XML comments in `backend/src/EmployeeApi/` to improve generated OpenAPI
- [ ] T037 [P] Add OpenAPI/contract tests in `backend/src/EmployeeApi.Tests/Contract/OpenApiSpec_Tests.cs`
- [ ] T038 [P] Add README updates and quickstart in `backend/src/EmployeeApi/README.md`
- [ ] T039 [P] Add production provider configuration placeholder `backend/src/EmployeeApi/appsettings.Production.json` and documentation
- [ ] T040 [P] Run static analysis / formatting config files (e.g., `.editorconfig`, `dotnet format`) in repo root

---

## Dependencies & Execution Order

- Phase 1 (Setup) → Phase 2 (Foundational) → Phase 3..7 (User Stories) → Phase 8 (Polish)
- Within each user story: tests (integration/unit) can be written in parallel with implementation tasks marked [P] where they touch different files.

## Parallel Opportunities
- Package installs, scaffolding, and CI tasks (T001-T005) can run in parallel.
- DTOs, DbContext, repository interface, validators, and mapping (T007-T013) can be implemented in parallel by different developers.
- Each user story's tests and service logic are parallelizable when implemented in separate files.

## Implementation Strategy
- MVP: Complete Phases 1, 2, and Phase 3 (List Employees) to produce a demoable API.
- Incremental delivery: Add Create, Read by Id, Update, Delete in priority order with tests per story.

***

