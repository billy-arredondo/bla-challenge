# Task Manager Challenge

A task management application for the .NET technical interview challenge. The solution includes a .NET API, SQLite persistence, JWT authentication, role-based task management, and a responsive React frontend.

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

- `src/backend/BlaChallenge.slnx` contains four .NET 10 production projects: Domain, Application, Infrastructure, and an ASP.NET Core API host, plus matching test projects.
- Each production project has a matching xUnit test project, with Coverlet configured. Tests cover domain validation, application use cases, SQLite persistence, and authentication endpoints.
- The API exposes public registration/login/ping endpoints, a protected `me` endpoint, protected task endpoints, Admin/Superadmin user lookup for assignment, and Superadmin-only user creation and role-management endpoints. SQLite initializes and migrates the `Users` and `Tasks` tables at startup.
- `src/frontend` contains the React/Vite application with authentication, task CRUD, role-aware assignment, user management, loading/error states, and responsive light/dark themes.
- A configurable Superadmin credential seed is available. The demo flow and credentials are documented below; task records can be created through the UI or API.

Project dependencies and the next implementation steps are documented in the [implementation plan](docs/implementation-plan.md).

## Project notes

- [`docs/brainstorming.md`](docs/brainstorming.md): initial ideas and proposals.
- [`docs/implementation-plan.md`](docs/implementation-plan.md): current architecture, confirmed decisions, pending scope, and implementation sequence.
- [`docs/development-log.md`](docs/development-log.md): chronological record of development work and verification.
- [`docs/genai.md`](GENAI.md): GenAI prompts, representative output, review evidence, and limitations.
- [`docs/testing.md`](docs/testing.md): tested behavior by backend layer, test commands, and current testing gaps.
- [`AGENTS.md`](AGENTS.md): repository-specific guidance for coding agents.

## Development and demo

### Prerequisites

- Install the .NET 10 SDK. The current scaffold was verified with SDK `10.0.402`; no `global.json` pins an SDK version.
- Package restore requires access to the configured NuGet package sources.
- Set a development-only JWT signing key outside the repository. It must contain at least 32 characters.
- Optionally set `Superadmin:Email` and `Superadmin:Password` outside the repository to seed an idempotent Superadmin account.
- Install Node.js 22 or later and pnpm for the frontend.

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
$env:Superadmin__Email = "superadmin@example.com"
$env:Superadmin__Password = "replace-with-a-local-password-of-at-least-8-characters"
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

### Run the frontend

With the API running on `http://localhost:5278`, start a second terminal:

```sh
cd src/frontend
pnpm install
pnpm dev
```

Open the Vite URL shown in the terminal, normally `http://localhost:5173`.
The development proxy forwards `/api` requests to the HTTP API. Set
`VITE_API_URL` when the API runs at another address.

Validate the frontend before the demo:

```sh
pnpm lint
pnpm build
```

The API endpoints implemented in this iteration are:

- `GET /api/auth/ping`: public health-style endpoint.
- `POST /api/auth/register`: creates a user and returns `201 Created`.
- `POST /api/auth/login`: validates credentials and returns a JWT.
- `GET /api/auth/me`: protected endpoint requiring `Authorization: Bearer <token>`.
- `POST /api/tasks`: creates a task. Users are assigned automatically; Admins can choose any assigned user.
- `GET /api/tasks`: lists assigned tasks for Users, or all tasks for Admins and Superadmins.
- `GET /api/tasks/{id}`: gets an assigned task for Users, or any task for Admins and Superadmins.
- `PUT /api/tasks/{id}`: updates a task according to role and status rules.
- `DELETE /api/tasks/{id}`: physically deletes a task according to role and status rules.
- `GET /api/users`: lists users for Superadmins.
- `POST /api/users`: creates a User or Admin for Superadmins.
- `PUT /api/users/{id}/role`: grants or removes Admin privileges for Superadmins.

Task request fields are `title` (required, up to 200 characters), `description` (optional, up to 2,000 characters), `status` (`Pending`, `InProgress`, `Completed`, or `Canceled`), `dueDateUtc` (required ISO-8601 date), and optional `assignedToId` for Admins. `Canceled` is terminal; `Completed` cannot be reopened. Due dates in the past are accepted to represent overdue tasks.

The SQLite database is created as `bla-challenge.db` in the API working directory. The `Users` and `Tasks` tables are initialized automatically. Do not commit the database file or JWT key.

For local HTTPS, trust the development certificate where supported and use the HTTPS profile:

```sh
dotnet dev-certs https --trust
dotnet run --project src/backend/BlaChallenge.Api --launch-profile https
```

The HTTPS profile uses `https://localhost:7226` and `http://localhost:5278`. Database initialization and the optional Superadmin seed are automatic.

### Demo credentials and flow

The API does not contain a password or signing key. Configure local demo
credentials before starting it:

```powershell
$env:Jwt__Key = "local-demo-signing-key-with-at-least-32-characters"
$env:Superadmin__Email = "superadmin@example.com"
$env:Superadmin__Password = "superadmin123"
```

The Superadmin account is created or promoted idempotently at startup. Use it
to create an Admin and a User from the Users screen. Public registration can
also create a normal User with any valid email and a password of at least eight
characters. The complete presentation walkthrough is in
[`docs/presentation.md`](docs/presentation.md), and the informal user story is
in [`docs/user-story.md`](docs/user-story.md).

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

The configured Superadmin is seeded automatically when both configuration values are present. Public registration always creates a `User`; only a Superadmin can create users or change Admin privileges. Never commit passwords, JWT keys, or the SQLite database.
