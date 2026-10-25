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
```

The solution currently contains the three production projects and their
corresponding test projects. The current project dependencies are:

- Application references Domain.
- Infrastructure references Application and Domain.
- Each test project references the production project it tests.

The backend scaffolding builds successfully with the current .NET SDK. The API
project is now scaffolded and references Application and Infrastructure. The
The API test project is now scaffolded and references the API project. The
frontend has not been scaffolded yet.

The following projects remain to be added:

```text
src/backend/
src/frontend/                      # Frontend application
```

Create supporting files as they become useful rather than adding empty
placeholders. Confirm the solution format when selecting the .NET SDK.

## Decisions to confirm

- Use a task manager as the application scope: tasks belong to users and have a
  title, description, status, and due date.
- Select the .NET version, frontend framework, data store, and testing tools.
  React with Vite and SQLite with `Microsoft.Data.Sqlite` are initial suggestions.
- Use Clean Architecture. If using CQRS, call command and query use cases directly
  without a mediator or generic dispatcher.
- Define authentication, public/protected endpoints, and task ownership rules.
  Clarify the challenge's wording about a “second API” before finalizing API boundaries.

## Implementation sequence

1. **Define scope:** write the informal user story and acceptance criteria for
   registration, login, and task CRUD.
2. **Complete scaffolding:** set up the frontend and verify the actual build and
   test commands. The initial Domain, Application, Infrastructure, API, and
   backend test project scaffolding is complete.
3. **Implement the backend incrementally with TDD:** start with a complete use
   case, then cover persistence, business rules, authentication, and API behavior.
   Do not use Entity Framework, Dapper, or Mediator.
4. **Integrate the frontend:** implement responsive authentication and task CRUD
   screens, including validation, loading, empty, and error states.
5. **Verify the demo:** seed data and credentials; check the complete user flow,
   authorization, and tests for data access, business logic, and API endpoints.
6. **Prepare delivery:** document reproducible setup/start commands, finalize
   architecture notes, and prepare a concise presentation with a working demo.

Record GenAI prompts, representative output, validation, corrections, and handling
of edge cases/authentication throughout implementation, not only at the end.
Consider Azure deployment after the required functionality and delivery work are complete.
