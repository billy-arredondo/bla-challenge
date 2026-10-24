# Implementation Plan

Working proposal for completing [the challenge](challenge.md), based on the
[brainstorming notes](brainstorming.md). Update this plan as decisions are made;
the structure below is proposed, not yet implemented.

## Proposed repository structure

```text
README.md                         # Overview, prerequisites, setup, and demo credentials
AGENTS.md                         # Stable guidance for coding agents
TaskManager.sln
src/
  TaskManager.Domain/             # Entities and domain rules
  TaskManager.Application/        # Use cases and persistence interfaces
  TaskManager.Infrastructure/     # Data access implementations
  TaskManager.Api/                # ASP.NET Core Web API
  task-manager-web/               # Frontend application
tests/
  TaskManager.UnitTests/
  TaskManager.IntegrationTests/
docs/
  challenge.md                    # Original requirements
  brainstorming.md                # Ideas and alternatives
  implementation-plan.md          # Agreed decisions and next steps
  user-story.md                   # User story, scope, and acceptance criteria
  architecture.md                 # Technical decisions and a simple diagram
  genai.md                        # Prompts, output samples, validation, and corrections
  presentation/                   # Slides and demo outline
scripts/                          # Setup and startup automation, if needed
```

Create supporting files as they become useful rather than adding empty placeholders.
Confirm the solution format when selecting the .NET SDK.

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
2. **Create scaffolding:** set up the solution, layer references, frontend, and
   test projects; verify the actual build and test commands.
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
