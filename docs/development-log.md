# Development Log

Chronological record of the steps taken to build the application. Keep entries
focused on significant work, decisions, and verification results.

## Challenge preparation

- I asked ChatGPT to convert the PDF into Markdown, ensuring all content was preserved. Afterward, I manually verified that 100% of the content was present. This step is critical because every detail matters, and any omission could invalidate my solution.

## Repository setup

- Defined and documented Git naming conventions for branches, commits, and pull requests in `docs/contributing.md`, so the repository history stays consistent.

## OpenCode workflow

- Added custom commands to suggest branch names, create an exact branch, and generate commit messages from staged changes, with DeepSeek model preference and fallback to the current session model.

## Backend scaffolding

- Created the initial `BlaChallenge.slnx` solution under `src/backend/` with Domain, Application, and Infrastructure projects and matching test projects. The API, API tests, and frontend remain to be added.
- Configured project references so Application depends on Domain, Infrastructure depends on Application and Domain, and each test project references the production project it tests.
- Verified the backend solution with `dotnet build BlaChallenge.slnx --no-restore`: the build succeeded with zero warnings and zero errors.
- Added the ASP.NET API project with references to Application and Infrastructure, and removed the template WeatherForecast endpoint and local project artifacts. The API test project and frontend remained to be added at that point.
- Added the API test project, referenced the API project, and registered it in the solution. The frontend remains to be added.

## GenAI review policy

- Decided not to add custom OpenCode skills because the project is small and `AGENTS.md` provides sufficient repository guidance. Custom commands should generally return suggestions or text for human review instead of applying changes directly; `/new-branch` remains an explicit exception because it performs the requested branch creation.

## Documentation Review 001

- Reviewed the current scaffold against the challenge and existing documentation.
- Updated the README and agent guidance to describe the four .NET 10 production
  projects and matching xUnit test projects, with root-relative setup/build/test
  commands and configured API launch profiles.
- Corrected stale project counts and pending-project claims in the implementation
  plan and GenAI record. Frontend/database choices, business functionality,
  authentication, demo data, and meaningful tests remain pending.
- Verified SDK `10.0.402` and ran `dotnet build src/backend/BlaChallenge.slnx`:
  succeeded with zero warnings and zero errors.
- Ran `dotnet test src/backend/BlaChallenge.slnx --no-build`: all four assemblies
  reported no tests available. Recorded this as an empty test scaffold rather
  than a passing functional suite.
- Recorded the review prompt, representative scaffold, corrections, validation,
  and limitations in `docs/genai.md`.

## Documentation Review 002

- Reviewed the challenge, current implementation plan, brainstorming notes,
  repository guidance, and GenAI record before updating the plan.
- Added a classified technical-decision table and expanded the implementation
  sequence for scope confirmation, API contract design, TDD, persistence,
  authentication, authorization, frontend integration, and delivery.
- Recorded the proposed SQLite, single-host API, private-task, and React/Vite
  choices without presenting them as implemented or confirmed.
- Kept authentication mechanism, task status values, and description/due-date
  validation pending human confirmation.
- No application code or runtime validation was performed during this review.

## Authentication Iteration 001

- Implemented the first functional backend slice: user registration, password
  hashing, login, JWT generation, a public `ping` endpoint, and a protected
  `me` endpoint. Task CRUD and the frontend remain pending.
- Confirmed SQLite with `Microsoft.Data.Sqlite`, parameterized SQL, one API host,
  JWT Bearer authentication, and private-user authorization as the current
  implementation decisions. The JWT signing key is supplied outside the
  repository through configuration.
- Added the `User` domain entity, Application authentication contracts and use
  cases, SQLite initialization and repository code, PBKDF2 password hashing,
  JWT token creation, API dependency injection, and authentication controller.
- Added xUnit tests for domain validation, application behavior, SQLite
  persistence, public/protected API behavior, registration, login, and invalid
  credentials. The first application test run failed because the contracts did
  not exist yet; implementation then made the tests pass.
- The first full test run exposed SQLite file-lock failures during cleanup.
  Clearing SQLite connection pools before deleting temporary test databases
  corrected the test teardown.
- Verified `dotnet build src/backend/BlaChallenge.slnx`: succeeded with zero
  warnings and zero errors.
- Verified `dotnet test src/backend/BlaChallenge.slnx`: 13 tests passed across
  the four test projects.
- Ran the API process with a temporary SQLite path and an externally supplied
  JWT key. The live smoke test returned `200` for `/api/auth/ping` and
  `/openapi/v1.json`; temporary database and log files were removed afterward.
- Next steps are task domain and CRUD functionality, task ownership tests,
  frontend selection/integration, and seeded demo data.

## Documentation Review 003

- Applied the repository organization guideline to the backend Application
  project by moving operation files directly under `Users/Commands` and
  `Users/Queries`.
- Updated namespaces and consuming `using` directives without changing the
  command, query, or use-case type names.
- Documented the rule in `docs/contributing.md` and `docs/implementation-plan.md`.
- Verified `dotnet restore src/backend/BlaChallenge.slnx` successfully.
- Verified `dotnet build src/backend/BlaChallenge.slnx --no-restore`: succeeded
  with zero warnings and zero errors.
- Verified `dotnet test src/backend/BlaChallenge.slnx --no-build`: 14 tests passed
  across the four test projects.

## Documentation Review 004

- Reorganized Application operations so each command or query has a dedicated
  folder containing conventionally named `Command.cs`, `Query.cs`, and
  `UseCase.cs` files as applicable.
- Updated namespaces and consuming `using` directives while preserving the
  public command, query, and use-case type names.
- Updated the repository organization guideline to describe the operation-folder
  convention.
- Verified `dotnet restore src/backend/BlaChallenge.slnx` successfully.
- Verified `dotnet build src/backend/BlaChallenge.slnx --no-restore`: succeeded
  with zero warnings and zero errors.
- Verified `dotnet test src/backend/BlaChallenge.slnx --no-build`: 14 tests passed
  across the four test projects.

## Documentation Review 005

- Added the root `.editorconfig` with C# rules for file-scoped namespaces,
  namespace-first layout, ordered imports, and unused-import diagnostics.
- Clarified that the root `.editorconfig` is intentionally repository-wide and
  applies the C# layout rules to `src/backend`.
- Applied the convention across backend production and test projects, removing
  unused `using` directives and converting `Program.cs` from top-level statements
  to an explicit `Program.Main` entry point so it follows the same layout.
- Documented the convention and `dotnet format` verification command in
  `docs/contributing.md` and `docs/implementation-plan.md`, including the
  configuration file location and scope.
- Verified `dotnet build src/backend/BlaChallenge.slnx --no-restore`: succeeded
  with zero warnings and zero errors.
- Verified `dotnet test src/backend/BlaChallenge.slnx --no-build`: 14 tests passed
  across the four test projects.

## Documentation Review 006

- Added a repository convention to prefer primary constructors for backend
  classes when they simplify dependency initialization and readability.
- Converted suitable application, infrastructure, and API classes to primary
  constructors without changing their public behavior.
- Kept traditional constructors for the `User` domain entity, configuration
  and exception types, and other cases where the primary-constructor form would
  make validation, initialization, or behavior less clear.
- Documented the convention and its justified exception in
  `docs/contributing.md` and `docs/implementation-plan.md`.
- Updated the root `.editorconfig` with the primary-constructor style
  preference and its suggestion-level diagnostic.
- Verified `dotnet format src/backend/BlaChallenge.slnx --no-restore
  --verify-no-changes`, `dotnet build src/backend/BlaChallenge.slnx
  --no-restore`, and `git diff --check` successfully.
- Verified `dotnet test src/backend/BlaChallenge.slnx --no-build`: 14 tests
  passed across the four test projects.

## Documentation Review 007

- Documented that `#region` blocks should not be used as the primary way to
  organize API controllers.
- Documented that small endpoint-specific request and response records may
  remain beside their controller, while reused or growing records should move
  to separate files.
- Added the convention to `docs/contributing.md` and
  `docs/implementation-plan.md`.

## Documentation Review 008

- Compared the implemented backend structure, authentication flow, persistence
  configuration, API launch profiles, and test projects with the documentation.
- Corrected the formatting guidance to identify `src/backend/.editorconfig` as
  the source of backend C# rules, while the root file provides repository-wide
  defaults and the backend's four-space indentation override.
- Added `.gitattributes` to keep repository checkouts on LF even when a local
  Git configuration enables automatic line-ending conversion.
- Documented the configured .NET user-secrets support for the local JWT signing
  key and clarified the JWT defaults.
- Confirmed that the current iteration still implements user authentication and
  SQLite user persistence only; task CRUD, the frontend, seeded demo data, and
  presentation materials remain pending.
- No backend source code was changed during this review.
- Verified `dotnet format src/backend/BlaChallenge.slnx --no-restore
  --verify-no-changes` successfully after LF normalization.
- Verified `dotnet build src/backend/BlaChallenge.slnx --no-restore`: succeeded
  with zero warnings and zero errors.
- Verified `dotnet test src/backend/BlaChallenge.slnx --no-build`: 14 tests
  passed across the four test projects.
- Verified `git diff --check` and confirmed all tracked files use LF line
  endings.

## Task Management Iteration 001

- Reviewed `AGENTS.md`, the challenge, implementation plan, development log,
  GenAI record, README, all backend layers, and the existing xUnit projects before
  changing source code.
- Confirmed the task contract: `Pending`, `InProgress`, and `Completed` statuses;
  optional descriptions up to 2,000 characters; required UTC due dates; and
  titles up to 200 characters. Past due dates are accepted to represent overdue
  work.
- Implemented the `Task` domain entity and validation, direct Application CRUD
  use cases, SQLite `Tasks` persistence with parameterized SQL, and an authorized
  `TasksController`.
- Ownership is derived from the JWT `NameIdentifier` claim. Repository lookups
  require both task id and owner id, so another user's task is indistinguishable
  from a missing task and returns `404`.
- The first build exposed a C# name collision between the domain `Task` entity
  and `System.Threading.Tasks.Task`; explicit type qualification was applied in
  asynchronous contracts, implementations, and tests without renaming the
  domain entity.
- Added domain, application, infrastructure, and API tests for validation,
  CRUD, ownership isolation, authentication, invalid titles, and missing tasks.
- Updated JSON configuration so task statuses are represented as strings in the
  API contract. Automatic demo seeding and the frontend remain pending.
- Final validation passed: `dotnet format ... --verify-no-changes`,
  `git diff --check`, and `dotnet build src/backend/BlaChallenge.slnx
  --no-restore` completed successfully. `dotnet test
  src/backend/BlaChallenge.slnx --no-build` passed 27 tests across the four test
  projects.

## Documentation Review 009

- Added `docs/testing.md` to explain what is tested at the Domain, Application,
  Infrastructure, and API levels.
- Documented the current authentication, task CRUD, persistence, ownership, and
  validation scenarios, along with testing gaps for the pending frontend and
  seeded demo flow.
- Linked the testing guide from the root README.
- Verified `dotnet test src/backend/BlaChallenge.slnx --no-build`: 27 tests
  passed across the four test projects, and `git diff --check` passed.

## Documentation Review 010

- Expanded `docs/testing.md` with the exact test file and method name for every
  current Domain, Application, Infrastructure, and API test.
- Converted the behavior inventory from grouped bullet points into per-project
  tables so each documented behavior can be traced to its implementation.
- Clarified that the 27 executed test cases include two cases generated by the
  parameterized `Create_rejects_empty_title` method.

## Documentation Review 011

- Documented that the frontend screen designs were extracted from Google Stitch.
- Added the Stitch screen mapping and clarified that the HTML files are design
  references rather than production frontend code.

## Documentation Review 012

- Recorded the preference to create the project structure and initial
  dependencies manually rather than through prompts or automatically generated
  scaffolding.

## Documentation Review 013

- Implemented role-based authorization with `User`, `Admin`, and `Superadmin` roles.
- Added task creator, assignee, and last-updated audit fields, plus `Canceled` status rules and terminal-state enforcement.
- Added SQLite schema migration support, JWT role claims, Superadmin seeding from configuration, and Superadmin-only user/role management endpoints.
- Updated task authorization so Users see only assigned tasks, while Admins and Superadmins can manage all non-canceled tasks and assign work to any user.
- Added API coverage for completed-task restrictions and Superadmin/Admin assignment flow.
- Verified `dotnet format src/backend/BlaChallenge.slnx --no-restore --verify-no-changes`, a zero-warning build, and 32 passing tests.
