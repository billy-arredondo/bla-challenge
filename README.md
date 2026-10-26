# Task Manager Challenge

A task management application is being built for the .NET technical interview challenge. The backend implements user registration, JWT authentication, and user-owned task CRUD; the frontend remains pending.

## Challenge requirements

The solution is expected to:

- Use .NET/C# and ASP.NET, following Clean Architecture principles.
- Provide a data store for application records and users, plus API operations for CRUD, user registration, login, and authorized and non-authorized access.
- Include a frontend framework of choice with a responsive interface and CRUD functionality.
- Include tests for data access, business logic, and API behavior, using TDD as the development approach.
- Provide setup instructions and seeded demo data/credentials.
- Document the user story, present and explain the implementation, and show how GenAI-generated suggestions were validated and improved.
- Avoid Entity Framework, Dapper, and “Mediator,” as stated in the challenge. The meaning of “Mediator” is ambiguous; see the implementation plan before making a design decision.

See [`docs/challenge.md`](docs/challenge.md) for the complete requirements.

## Repository status

- `src/backend/BlaChallenge.slnx` contains four .NET 10 projects: Domain, Application, Infrastructure, and an ASP.NET Core API host.
- Each production project has a matching xUnit test project, with Coverlet configured. Tests cover domain validation, application use cases, SQLite persistence, and authentication endpoints.
- The API exposes public registration/login/ping endpoints, a protected `me` endpoint, and protected task CRUD endpoints. SQLite initializes the `Users` and `Tasks` tables at startup.
- The frontend remains to be selected and implemented. Seeded task demo data and credentials are pending.

Project dependencies and the next implementation steps are documented in the [implementation plan](docs/implementation-plan.md).

## Project notes

- [`docs/brainstorming.md`](docs/brainstorming.md): initial ideas and proposals.
- [`docs/implementation-plan.md`](docs/implementation-plan.md): current architecture, confirmed decisions, pending scope, and implementation sequence.
- [`docs/development-log.md`](docs/development-log.md): chronological record of development work and verification.
- [`docs/genai.md`](docs/genai.md): GenAI prompts, representative output, review evidence, and limitations.
- [`docs/testing.md`](docs/testing.md): tested behavior by backend layer, test commands, and current testing gaps.
- [`AGENTS.md`](AGENTS.md): repository-specific guidance for coding agents.

## Development and demo

### Prerequisites

- Install the .NET 10 SDK. The current scaffold was verified with SDK `10.0.402`; no `global.json` pins an SDK version.
- Package restore requires access to the configured NuGet package sources.
- Set a development-only JWT signing key outside the repository. It must contain at least 32 characters.

### Build and test

Run these commands from the repository root:

```sh
dotnet restore src/backend/BlaChallenge.slnx
dotnet build src/backend/BlaChallenge.slnx --no-restore
dotnet test src/backend/BlaChallenge.slnx --no-build
```

The backend builds with zero warnings and zero errors. The test command runs the current domain, application, infrastructure, and API test suites.

### Run the API host

```sh
dotnet run --project src/backend/BlaChallenge.Api --launch-profile http
```

The HTTP development profile is configured for `http://localhost:5278`. In Development, the OpenAPI document is mapped at `/openapi/v1.json`; no Scalar UI is configured in this iteration.

Before starting the API in PowerShell, configure the key for the current process:

```powershell
$env:Jwt__Key = "replace-with-a-local-random-key-of-at-least-32-characters"
dotnet run --project src/backend/BlaChallenge.Api --launch-profile http
```

The API project supports .NET user secrets for local development. Set the key
once instead of exporting it for each process:

```sh
dotnet user-secrets --project src/backend/BlaChallenge.Api \
  set "Jwt:Key" "replace-with-a-local-random-key-of-at-least-32-characters"
```

The key must contain at least 32 characters. The issuer, audience, and token
expiration can be overridden through the `Jwt` configuration section; the
defaults are `BlaChallenge`, `BlaChallenge.Client`, and 60 minutes.

The API endpoints implemented in this iteration are:

- `GET /api/auth/ping`: public health-style endpoint.
- `POST /api/auth/register`: creates a user and returns `201 Created`.
- `POST /api/auth/login`: validates credentials and returns a JWT.
- `GET /api/auth/me`: protected endpoint requiring `Authorization: Bearer <token>`.
- `POST /api/tasks`: creates an authenticated user's task. The owner is taken from the JWT, not the request body.
- `GET /api/tasks`: lists only the authenticated user's tasks.
- `GET /api/tasks/{id}`: gets one owned task, or returns `404` for a missing or another user's task.
- `PUT /api/tasks/{id}`: updates an owned task.
- `DELETE /api/tasks/{id}`: deletes an owned task.

Task request fields are `title` (required, up to 200 characters), `description` (optional, up to 2,000 characters), `status` (`Pending`, `InProgress`, or `Completed`), and `dueDateUtc` (required ISO-8601 date). Due dates in the past are accepted to represent overdue tasks.

The SQLite database is created as `bla-challenge.db` in the API working directory. The `Users` and `Tasks` tables are initialized automatically. Do not commit the database file or JWT key.

For local HTTPS, trust the development certificate where supported and use the HTTPS profile:

```sh
dotnet dev-certs https --trust
dotnet run --project src/backend/BlaChallenge.Api --launch-profile https
```

The HTTPS profile uses `https://localhost:7226` and `http://localhost:5278`. Database initialization is automatic; frontend setup, task seed data, and demo credentials will be documented as those features are implemented.

### Test the authentication and task endpoints

With the API running, register and log in using any HTTP client. For example, the following PowerShell commands use the HTTP profile:

```powershell
$body = @{ email = "demo@example.com"; password = "password123" } | ConvertTo-Json
Invoke-RestMethod -Method Post -Uri http://localhost:5278/api/auth/register -ContentType "application/json" -Body $body
$login = Invoke-RestMethod -Method Post -Uri http://localhost:5278/api/auth/login -ContentType "application/json" -Body $body
Invoke-RestMethod -Method Get -Uri http://localhost:5278/api/auth/me -Headers @{ Authorization = "Bearer $($login.accessToken)" }
$headers = @{ Authorization = "Bearer $($login.accessToken)" }
$task = @{ title = "Prepare demo"; description = "Show task CRUD"; status = "Pending"; dueDateUtc = "2026-12-31T00:00:00Z" } | ConvertTo-Json
Invoke-RestMethod -Method Post -Uri http://localhost:5278/api/tasks -Headers $headers -ContentType "application/json" -Body $task
Invoke-RestMethod -Method Get -Uri http://localhost:5278/api/tasks -Headers $headers
```

The repository does not currently seed credentials or task records automatically. The commands above create disposable demo data locally; seed automation remains a delivery follow-up.
