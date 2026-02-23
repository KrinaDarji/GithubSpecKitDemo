<!--
Sync Impact Report
- Version change: [unspecified placeholder] -> 1.0.0
- Modified principles:
	- [PRINCIPLE_1_NAME] -> API Consistency & Design (NON-NEGOTIABLE)
	- [PRINCIPLE_2_NAME] -> .NET 10 & C# 14 Platform Standard
	- [PRINCIPLE_3_NAME] -> EF Core Data Access (InMemory allowed for demos)
	- [PRINCIPLE_4_NAME] -> Validation & Error Handling
	- [PRINCIPLE_5_NAME] -> Documentation, Observability & API Contracts
- Added sections:
	- Technology Constraints
	- Development Workflow
- Removed sections: none
- Templates requiring updates / review status:
	- .specify/templates/plan-template.md: ✅ reviewed - contains a "Constitution Check" placeholder that references the constitution file; compatible with updated principles.
	- .specify/templates/spec-template.md: ✅ reviewed - generic spec template; no direct conflicts found.
	- .specify/templates/tasks-template.md: ✅ reviewed - task groupings align with new principles; update automation may populate concrete tasks.
	- .specify/templates/commands/*: ⚠ inspect - check any command docs for agent-specific names or guidance to generalize.
- Follow-up TODOs:
	- RATIFICATION_DATE: TODO(RATIFICATION_DATE): confirm the original adoption date and replace this placeholder.
	- If any templates are present in the workspace, reconcile "Constitution Check" entries to reference updated principles.
-->

# Backend API Constitution

## Core Principles

### API Consistency & Design (NON-NEGOTIABLE)
All public endpoints MUST follow consistent RESTful design conventions: noun-based resources, correct use
of HTTP verbs, standardized status codes, and clear resource modeling. Endpoints MUST use predictable
URLs, explicit versioning, and consistent query/pagination/filter semantics. Breaking API changes
MUST follow the Governance versioning rules and include a documented migration path.

### .NET 10 & C# 14 Platform Standard
The project MUST target .NET 10 and make pragmatic use of C# 14 modern language features where they
improve clarity, safety, or performance. Tooling, build scripts, and CI MUST compile against .NET 10.
Language and runtime features MUST be used intentionally and documented when introducing new patterns.

### EF Core Data Access (InMemory allowed for demos)
EF Core is the canonical data-access approach. For demos and unit/integration tests, the EF Core InMemory
provider MAY be used. Production deployments MUST use a durable provider and include schema migration
artifacts. Data access layers MUST be encapsulated behind repositories/services and be independently
testable.

### Validation & Error Handling
All incoming requests MUST be validated; invalid inputs MUST produce structured error responses (RFC7807
ProblemDetails or equivalent) with machine-readable error codes and human-readable messages. Exceptions
MUST be centrally handled; internal details MUST NOT be leaked to clients. Validation failures,
authorization failures, and server errors MUST use distinct, documented status codes and error schemas.

### Documentation, Observability & API Contracts
Every exposed endpoint MUST have OpenAPI/Swagger documentation produced from code annotations or
explicit contracts. APIs MUST include examples for common request/response shapes. Services MUST emit
structured logs with correlation IDs and include basic telemetry (request durations, error rates).

## Technology Constraints
The codebase MUST adhere to the following technology choices and constraints:
- Runtime & language: .NET 10 (target framework), C# 14
- Data access: EF Core (InMemory provider allowed for demos/tests only)
- API docs: Swagger/OpenAPI required for all public endpoints
- Persistence: Production providers and migration artifacts are required for non-demo deployments
- Dependencies: Prefer small, well-maintained libraries; new third-party dependencies MUST be
	justified in PR description and security-reviewed.

## Development Workflow
- Pull Requests: All changes to the API MUST be delivered via PR with peer review (minimum two reviewers,
	one of whom is a maintainer for breaking changes).
- Testing: Unit tests, integration tests (including in-memory EF Core where appropriate), and API contract
	tests are REQUIRED for behavior changes. CI MUST block merging on failing tests.
- API Versioning: Use explicit versioning strategy (URI versioning v1/v2 or header-based) and document
	the chosen approach in the project README.
- Deprecation: Deprecation notices MUST be documented in the OpenAPI spec and release notes; removal of
	deprecated endpoints follows the Governance process.

## Governance
Amendments to this constitution MUST be proposed as a PR to `.specify/memory/constitution.md`.
To ratify an amendment:
- The PR MUST include the proposed text, a rationale, and an impact analysis.
- Approval requires at least two reviewers, one of whom is a project maintainer for governance-affecting
	changes.
- The `Last Amended` date MUST be updated to the merge date.

Versioning policy (semantic rules):
- MAJOR: Backward-incompatible governance or principle removals/renames.
- MINOR: Addition of new principle/section or material expansion of guidance.
- PATCH: Clarifications, wording fixes, and non-semantic refinements.

Compliance expectations:
- All API-related PRs MUST include a short checklist referencing the relevant principles (design,
	validation, docs, versioning).
- Security, privacy, and compliance reviews MUST be scheduled for material changes to data handling.

**Version**: 1.0.0 | **Ratified**: TODO(RATIFICATION_DATE): confirm adoption date | **Last Amended**: 2026-02-23
