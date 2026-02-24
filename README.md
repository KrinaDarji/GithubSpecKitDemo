Spec-Kit provides a structured Spec-Driven Development workflow with these core slash commands:
| Command                 | Purpose                                       |               |
| ----------------------- | --------------------------------------------- | ------------- |
| `/speckit.constitution` | Define your project principles and guidelines |               |
| `/speckit.specify`      | Write functional requirements/specification   |               |
| `/speckit.plan`         | Create a technical implementation plan        |               |
| `/speckit.tasks`        | Generate actionable tasks                     |               |
| `/speckit.implement`    | Build the implementation from tasks           |               |
| `/speckit.clarify`      | Clarify ambiguous spec requirements           |               |
| `/speckit.analyze`      | Analyze spec/task consistency                 |               |
| `/speckit.checklist`    | Generate quality checklists                   | ([GitHub][1]) |

[1]: https://github.com/github/spec-kit?utm_source=chatgpt.com "GitHub - github/spec-kit: 💫 Toolkit to help you get started with Spec-Driven Development"

1️⃣ Initialize Spec-Kit
  specify init . (select the ai i.e  --ai claude)
2️⃣ Define Project Principles
  /speckit.constitution
Create project principles for a backend API that emphasize API consistency, use of .NET 10, C# 14 modern features, clean RESTful standards, use of EF Core (InMemory for demo), meaningful validation and error handling, and API documentation via Swagger.
3️⃣ Write the Specification
/speckit.specify
Define a specification for an Employee CRUD API using .NET 10 Web API. It must include:
1. GET all employees.
2. GET employee by id.
3. POST create employee.
4. PUT update employee.
5. DELETE employee.
Also define what the Employee entity fields are (Id, FirstName, LastName, Email, Salary, DateOfJoining), acceptance criteria, HTTP status codes, and success/error behavior.
4️⃣ Create a Technical Plan
/speckit.plan
Generate a technical development plan using .NET 10 Web API, minimal APIs, EF Core InMemory database. Mention project structure, entities, validation rules, and Swagger documentation.
5️⃣ Break Down Into Tasks
/speckit.tasks
Generate a list of implementation tasks from the plan for the Employee CRUD API, including data model, API endpoints, validations, unit tests, and Swagger setup.
6️⃣ (Optional) Clarify Spec Questions
/speckit.clarify
Ask questions to clarify acceptance criteria around validation rules, error responses, and example request/response formats for the Employee API.
7️⃣ Implement the Feature
/speckit.implement
Execute the tasks to build the .NET 10 Web API code for Employee CRUD.

**Notes & Best Practices

- Focus the spec on what behavior is expected, not how it will be coded. Then let Spec-Kit generate plan/tasks and implementation.
-Review and refine the specification and tasks before implementing — the better the spec, the more accurate the outcome.
-You can also use /speckit.checklist after planning to ensure quality requirements are covered.
