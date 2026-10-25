# Task Manager Challenge

A task management application is being built for the .NET technical interview challenge. The current backend iteration implements user registration and JWT authentication; task management and the frontend remain pending.

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
- The API exposes public registration/login/ping endpoints and a protected `me` endpoint. SQLite initializes the `Users` table at startup; task models and CRUD endpoints are not implemented.
- The frontend remains to be selected and implemented. Seeded task demo data and credentials are pending.

Project dependencies and the next implementation steps are documented in the [implementation plan](docs/implementation-plan.md).

## Project notes

- [`docs/brainstorming.md`](docs/brainstorming.md): initial ideas and proposals.
- [`docs/implementation-plan.md`](docs/implementation-plan.md): current architecture, confirmed decisions, pending scope, and implementation sequence.
- [`docs/development-log.md`](docs/development-log.md): chronological record of development work and verification.
- [`docs/genai.md`](docs/genai.md): GenAI prompts, representative output, review evidence, and limitations.
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

The API endpoints implemented in this iteration are:

- `GET /api/auth/ping`: public health-style endpoint.
- `POST /api/auth/register`: creates a user and returns `201 Created`.
- `POST /api/auth/login`: validates credentials and returns a JWT.
- `GET /api/auth/me`: protected endpoint requiring `Authorization: Bearer <token>`.

The SQLite database is created as `bla-challenge.db` in the API working directory. The `Users` table is initialized automatically. Do not commit the database file or JWT key.

For local HTTPS, trust the development certificate where supported and use the HTTPS profile:

```sh
dotnet dev-certs https --trust
dotnet run --project src/backend/BlaChallenge.Api --launch-profile https
```

The HTTPS profile uses `https://localhost:7226` and `http://localhost:5278`. Database initialization is automatic; frontend setup, task seed data, and demo credentials will be documented as those features are implemented.

### Test the authentication endpoints

With the API running, register and log in using any HTTP client. For example, the following PowerShell commands use the HTTP profile:

```powershell
$body = @{ email = "demo@example.com"; password = "password123" } | ConvertTo-Json
Invoke-RestMethod -Method Post -Uri http://localhost:5278/api/auth/register -ContentType "application/json" -Body $body
$login = Invoke-RestMethod -Method Post -Uri http://localhost:5278/api/auth/login -ContentType "application/json" -Body $body
Invoke-RestMethod -Method Get -Uri http://localhost:5278/api/auth/me -Headers @{ Authorization = "Bearer $($login.accessToken)" }
```
