# Implementation Plan

Working plan for completing [the challenge](challenge.md), based on the
[brainstorming notes](brainstorming.md). Update this plan as decisions are made.

## Current Repository Structure

```text
src/backend/
  BlaChallenge.slnx
  BlaChallenge.Api/                # ASP.NET Web API
  BlaChallenge.Domain/             # Entities and domain rules
  BlaChallenge.Application/        # Entity-based commands, queries, and contracts
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

The backend implementation currently includes a `User` domain entity,
registration and login use cases, SQLite user persistence, PBKDF2 password
hashing, JWT Bearer authentication, and authentication controllers. The four
test projects use xUnit and Coverlet and contain domain, application,
infrastructure, and API tests for this iteration. The task model and CRUD
endpoints are not implemented. The frontend has not been scaffolded.

Application features are organized by entity. The current `Users` feature is
split into `Commands` for registration and login and `Queries` for retrieving
the authenticated user. Operation files are placed directly in those folders;
each operation has its own folder with conventional `Command.cs`, `Query.cs`,
and `UseCase.cs` files as applicable. Use cases are invoked directly by the API;
no Mediator or generic dispatcher is used.

C# files use file-scoped namespaces on the first line, followed by a blank line
and ordered, necessary `using` directives. The repository-wide `.editorconfig`
provides UTF-8, LF line endings, and two-space indentation defaults. The
`src/backend/.editorconfig` override uses four-space indentation and enforces
file-scoped namespaces, using placement, sorting, and removal of unnecessary
imports for backend C# files.
Classes should prefer primary constructors when they simplify dependency
initialization and readability. Traditional constructors remain appropriate
when the primary-constructor form would make validation, initialization,
inheritance, or other logic more complex.
Controllers should remain organized by normal file order rather than `#region`
blocks. Small endpoint-specific request and response records may live beside
the controller; move them to separate files when they are reused or make the
controller harder to read.

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
| Persistence | Confirmed | Use SQLite with `Microsoft.Data.Sqlite` and parameterized SQL. This avoids Entity Framework and Dapper, both prohibited by the challenge. The current iteration initializes a `Users` table at API startup. |
| API organization | Confirmed | Use one ASP.NET API host with separate controllers for authentication/users and tasks. The current iteration implements the authentication controller; task controllers remain pending. The challenge's reference to a “second API” is treated as an endpoint grouping rather than a separate service. |
| Authentication | Confirmed | Use JWT Bearer authentication. Registration and login are implemented; the signing key is supplied through configuration outside the repository. |
| Authorization | Confirmed | Tasks will be private to their owning user. The current iteration demonstrates protected access with `/api/auth/me`; task ownership enforcement remains pending with task CRUD. |
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

1. **Current iteration complete:** implemented registration, login, SQLite user
   storage, password hashing, JWT generation, public `ping`, and protected `me`
   endpoints with tests across the layers.
2. **Define the task scope:** write the informal user story and acceptance
   criteria for task CRUD. Resolve the pending task status, description, and
   due-date validation decisions.
3. **Implement task functionality with TDD:** add the task domain model, data
   access layer, application use cases, ownership checks, and API contract using
   SQLite and parameterized SQL. Add tests before or alongside each use case.
4. **Scaffold and integrate the frontend:** after the framework is confirmed,
   implement responsive authentication and task CRUD screens, including
   validation, loading, empty, and error states.
5. **Verify the demo:** seed data and credentials; check the complete user flow,
   authorization boundaries, invalid input, authentication failures, and tests
   for data access, business logic, and API endpoints.
6. **Prepare delivery:** document reproducible setup/start commands, finalize
   architecture notes, and prepare a concise presentation with the user story,
   design choices, technical architecture, GenAI review evidence, and working
   demo.

Record GenAI prompts, representative output, validation, corrections, and handling
of edge cases/authentication throughout implementation, not only at the end.
Consider Azure deployment after the required functionality and delivery work are complete.
