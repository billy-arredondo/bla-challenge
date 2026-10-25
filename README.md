# Task Manager Challenge

A task management application is planned for the .NET technical interview challenge. The application will provide a web frontend and API for managing user-owned tasks. **Backend scaffolding is in place; business functionality is not implemented yet.**

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
- Each production project has a matching xUnit test project, with Coverlet configured. No test cases have been implemented yet.
- The API host registers controllers and development-only OpenAPI generation. There are no business endpoints, task/user models, persistence, or authentication yet.
- The frontend and data store remain to be selected and implemented. Demo data and credentials are also pending.

Project dependencies and the next implementation steps are documented in the [implementation plan](docs/implementation-plan.md).

## Project notes

- [`docs/brainstorming.md`](docs/brainstorming.md): initial ideas and proposals.
- [`docs/implementation-plan.md`](docs/implementation-plan.md): proposed structure, decisions to confirm, and implementation sequence.
- [`docs/development-log.md`](docs/development-log.md): chronological record of development work and verification.
- [`docs/genai.md`](docs/genai.md): GenAI prompts, representative output, review evidence, and limitations.
- [`AGENTS.md`](AGENTS.md): repository-specific guidance for coding agents.

## Development and demo

### Prerequisites

- Install the .NET 10 SDK. The current scaffold was verified with SDK `10.0.402`; no `global.json` pins an SDK version.
- Package restore requires access to the configured NuGet package sources.

### Build and test

Run these commands from the repository root:

```sh
dotnet restore src/backend/BlaChallenge.slnx
dotnet build src/backend/BlaChallenge.slnx --no-restore
dotnet test src/backend/BlaChallenge.slnx --no-build
```

The scaffold builds with zero warnings and zero errors. The test command currently reports no tests available in all four test projects; this is not evidence of functional test coverage.

### Run the API host

```sh
dotnet run --project src/backend/BlaChallenge.Api --launch-profile http
```

The HTTP development profile is configured for `http://localhost:5278`. In Development, the OpenAPI document is mapped at `/openapi/v1.json`; no Swagger UI is configured. There are no business routes yet, so `/` and task/authentication routes are not available.

For local HTTPS, trust the development certificate where supported and use the HTTPS profile:

```sh
dotnet dev-certs https --trust
dotnet run --project src/backend/BlaChallenge.Api --launch-profile https
```

The HTTPS profile uses `https://localhost:7226` and `http://localhost:5278`. These commands start the scaffold only; frontend setup, database initialization, and seeded demo credentials will be documented as those features are implemented.
