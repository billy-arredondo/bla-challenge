# Implementation Plan

Working plan for completing [the challenge](challenge.md), based on the
[brainstorming notes](brainstorming.md). Update this plan as decisions are made.

## Current Repository Structure

```text
src/backend/
  BlaChallenge.slnx
  BlaChallenge.Api/                # ASP.NET Web API
  BlaChallenge.Domain/             # Entities and domain rules
  BlaChallenge.Application/        # Use cases and application contracts
  BlaChallenge.Infrastructure/     # Data access implementations
  BlaChallenge.Domain.Tests/       # Domain tests
  BlaChallenge.Application.Tests/  # Application tests
  BlaChallenge.Infrastructure.Tests/ # Infrastructure tests
  BlaChallenge.Api.Tests/          # API tests
```

The solution currently contains four .NET 10 production projects and their
corresponding test projects. The current project dependencies are:

- Application references Domain.
- Infrastructure references Application and Domain.
- API references Application and Infrastructure.
- Each test project references the production project it tests.

The backend scaffold builds with SDK `10.0.402` with zero warnings and zero
errors. The four test projects use xUnit and Coverlet, but contain no test cases;
`dotnet test` currently reports no tests available. The API host registers
controllers and development-only OpenAPI generation, but no business endpoints
are implemented. Domain, Application, and Infrastructure have no implementation
classes yet. The frontend has not been scaffolded.

The frontend remains to be added:

```text
src/frontend/                      # Frontend application
```

Create supporting files as they become useful rather than adding empty
placeholders. See the root [README](../README.md) for setup, build, test, and API
launch commands. The solution already uses the `.slnx` format; no `global.json`
currently pins the SDK.

## Technical Decisions

Each item is explicitly classified so that proposals are not mistaken for
implemented or approved decisions.

| Area | Status | Decision or open question |
| --- | --- | --- |
| Persistence | Proposed | Use SQLite with `Microsoft.Data.Sqlite` and parameterized SQL. This avoids Entity Framework and Dapper, both prohibited by the challenge. Confirmation is still required before implementation. |
| API organization | Proposed | Use one ASP.NET API host with separate controllers for authentication/users and tasks. The challenge's reference to a “second API” is ambiguous; confirm whether separate services are required. No business controllers exist yet. |
| Authentication | Pending | Choose either JWT Bearer authentication or cookie authentication. Registration and login are required; the mechanism and token/session lifecycle have not been selected. |
| Authorization | Proposed | Tasks should be private to their owning user. A user may access, change, or delete only that user's tasks. Confirm whether roles, sharing, or administrative access are out of scope. |
| Task status | Pending | Define the allowed statuses. `Pending`, `InProgress`, and `Completed` are candidate values, not a confirmed contract. |
| Description and due date | Pending | Confirm whether description and due date are optional or required, and whether past due dates are allowed. The challenge requires these task fields but does not define their validation rules. |
| Frontend | Proposed | Use React with Vite for the responsive CRUD frontend. Confirm this choice or select another framework before scaffolding `src/frontend/`. |

The following requirements and constraints come from the challenge or the
current scaffold:

- Use .NET/C# and Clean Architecture, keeping business rules independent of
  persistence and API code.
- Preserve the current dependencies: Application references Domain;
  Infrastructure references Application and Domain; API references Application
  and Infrastructure.
- Do not use Entity Framework, Dapper, or Mediator. If CQRS is used, call use
  cases directly without a mediator or generic dispatcher.
- Write tests for the data access, business logic, and API layers; TDD is the
  preferred development approach.

## Implementation sequence

1. **Confirm scope and rules:** write the informal user story and acceptance
   criteria for registration, login, authorization, and task CRUD. Resolve the
   pending authentication, API-boundary, status, and field-validation decisions.
2. **Maintain the backend foundation:** the Domain, Application, Infrastructure,
   API, and matching test project scaffolding is complete, and build/test commands
   have been verified. Add meaningful tests alongside each implemented use case.
3. **Implement the backend incrementally with TDD:** begin with the confirmed
   domain rules, then implement the data access layer with SQLite and
   parameterized SQL, application use cases, authentication, authorization, and
   API behavior. Add tests before or alongside each use case.
4. **Define and implement the API contract:** use appropriate HTTP verbs,
   parameters, status codes, validation errors, and protected/public routes for
   user and task operations.
5. **Scaffold and integrate the frontend:** after the framework is confirmed,
   implement responsive authentication and task CRUD screens, including
   validation, loading, empty, and error states.
6. **Verify the demo:** seed data and credentials; check the complete user flow,
   authorization boundaries, invalid input, authentication failures, and tests
   for data access, business logic, and API endpoints.
7. **Prepare delivery:** document reproducible setup/start commands, finalize
   architecture notes, and prepare a concise presentation with the user story,
   design choices, technical architecture, GenAI review evidence, and working
   demo.

Record GenAI prompts, representative output, validation, corrections, and handling
of edge cases/authentication throughout implementation, not only at the end.
Consider Azure deployment after the required functionality and delivery work are complete.
