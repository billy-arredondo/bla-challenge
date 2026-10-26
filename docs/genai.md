# Generative AI Record

This document records how generative AI was used during repository setup, scaffolding, and documentation review, and how its output was reviewed. It will be updated as application functionality is generated and validated.

## Scope So Far

Work so far includes repository workflow automation, backend scaffolding, and
the first authentication slice. The repository contains a .NET 10 solution with
Domain, Application, Infrastructure, API, and four matching xUnit test projects.
User registration, SQLite user persistence, password hashing, JWT authentication,
and authentication tests are implemented. Task functionality and the frontend
are not implemented yet.

## Tooling and Review Policy

This is a small project, so no custom OpenCode skills are used. Repository-wide
guidance is kept in `AGENTS.md`, which is sufficient for the current scope.

Custom commands should normally provide suggestions, generated text, or proposed
changes rather than applying changes directly. This preserves a deliberate human
review step before modifying files, committing code, or changing project state.
The `/new-branch` command is the explicit exception: after receiving an exact
valid branch name, it performs the requested Git branch creation. The other
workflow commands remain advisory and do not create commits or modify files.

## Starting Context

The work started by reviewing `docs/brainstorming.md` and `docs/challenge.md`. The challenge document provides the acceptance criteria and constraints. The brainstorming document provides initial ideas to evaluate, not fixed architectural decisions. The implementation is being iterated against both sources while treating the challenge requirements as authoritative.

The initial GenAI-assisted workflow was therefore:

1. Review the challenge requirements and the initial project ideas.
2. Identify repository, documentation, and development constraints.
3. Turn the useful ideas into small, reviewable workflow improvements.
4. Validate each generated change against the repository guidance before continuing.

### Backend structure

GenAI was used to compare possible .NET solution layouts against the challenge
requirements. The selected structure separates Domain, Application, and
Infrastructure projects and gives each layer a corresponding test project. The
solution is named `BlaChallenge.slnx` and is located under `src/backend/`.

The structure was deliberately kept independent of the task-management example
in the challenge. The API host and its test project were added after the initial
three layers. The frontend and task functionality remain next steps; the current
implementation contains the backend foundation and the first authentication use
cases.

## Prompts and Outcomes

### Branch workflow commands

The request was to create an OpenCode command that creates a Git branch according to `docs/contributing.md`. The requirement was later refined so that `/new-branch` accepts an exact branch name, while `/suggest-branch-names` accepts a purpose description and returns three English candidates.

The resulting commands are:

- `.opencode/commands/new-branch.md`
- `.opencode/commands/suggest-branch-names.md`

The commands enforce the allowed branch types, English names, and short kebab-case descriptions. The creation command validates the exact requested name and does not infer or modify it.

### Commit message command

The request was to create an OpenCode command that reviews staged changes and generates a commit message following the repository conventions.

The resulting command is `.opencode/commands/commit-message-staged.md`. It instructs OpenCode to inspect only `git diff --cached`, ignore unstaged and untracked files, produce one English commit subject, and avoid creating the commit or modifying files.

## Representative Generated Code

The following plugin selects a DeepSeek model when available for the workflow commands while preserving the current session model as a fallback:

```js
const deepseek = models.find(({ providerID, modelID }) =>
  providerID === "opencode" && /deepseek/i.test(modelID),
) ?? models.find(({ providerID, modelID }) =>
  /deepseek/i.test(`${providerID}/${modelID}`),
)

if (deepseek) message.model = deepseek
```

The plugin is located at `.opencode/plugins/branch-commands-model.js` and applies to `/new-branch`, `/suggest-branch-names`, and `/commit-message-staged`.

## Validation and Corrections

- Read `docs/contributing.md` before defining the commands.
- Confirmed that branch types and commit types match the repository conventions.
- Added explicit validation for branch names using a regular expression and `git check-ref-format --branch`.
- Ensured branch suggestions are generated in English even when the purpose is described in another language.
- Ensured commit-message-staged generation considers staged changes only.
- Ran `node --check` against the plugin successfully.
- Ran `git diff --check` successfully.
- Reviewed the staged diff manually before generating a commit message.
- Corrected the plugin name and export after its scope expanded from one command to three commands.

## Edge Cases and Safety

- Empty or vague branch purposes must request clarification.
- Invalid exact branch names must be rejected before Git commands run.
- Existing local branch names must not be reused.
- No custom command may reset, commit, push, or modify project files unless explicitly requested.
- The commit-message-staged command reports when no files are staged.
- If no DeepSeek model is available or the provider lookup fails, the current session model is retained.

## Current Limitations

The dynamic model-selection plugin has passed JavaScript syntax validation, but its OpenCode hook behavior has not yet been verified in a live OpenCode `1.19.4` session. The commands should therefore be tested after restarting OpenCode.

Future entries will continue documenting generated application code, test
results, authentication decisions, and any corrections made during
implementation.

## Task Management Iteration 001

### User prompt

The user requested implementation of the backend Task Management feature with
CRUD use cases, JWT authentication and authorization, Clean Architecture,
SQLite parameterized SQL, TDD tests across all layers, and continuous updates to
the required documentation. The user explicitly prohibited Entity Framework,
Dapper, and Mediator, and requested that the frontend not be modified.

### Proposals, decisions, and representative output

- The implementation proposed and applied a `Task` domain entity with a closed
  `TaskStatus` enum, direct Application use cases, an `ITaskRepository` port, a
  SQLite adapter, and a protected `TasksController`.
- Confirmed decisions were `Pending`, `InProgress`, and `Completed` statuses;
  optional descriptions capped at 2,000 characters; required due dates stored
  in UTC; and titles capped at 200 characters. Past due dates remain valid.
- The representative ownership enforcement is the parameterized repository
  predicate:

```csharp
WHERE Id = $id AND OwnerId = $ownerId;
```

- The controller never accepts an owner id. It reads `ClaimTypes.NameIdentifier`
  from the authenticated principal and passes that value to the Application
  command or query.

### Corrections and validation

- The first build failed because introducing a domain type named `Task` changed
  unqualified asynchronous return types into the domain entity. The affected
  contracts, implementations, and tests were corrected with explicit
  `System.Threading.Tasks.Task` qualification.
- JSON enum conversion was explicitly enabled so clients send and receive the
  documented string statuses rather than numeric enum values.
- A build after the initial implementation passed with zero warnings and zero
  errors. The first `--no-build` test command ran previously built test binaries;
  a fresh build must precede the final test result.
- Final validation after a fresh build passed 27 tests across Domain, Application,
  Infrastructure, and API test projects. Formatting verification and
  `git diff --check` also passed.

### Limitations and pending work

- The frontend, automatic seeded demo data, and presentation materials remain
  pending from the broader challenge. No real credentials or secrets were added.

## Documentation Review 001

### Prompt

The user requested a project review and updates to `docs/`, `AGENTS.md`, or
`README.md` where necessary (translated from the original Spanish request).

### Output and corrections

- Inspected tracked files, the solution, project references, API startup, launch
  profiles, and challenge requirements before revising documentation.
- Corrected claims that scaffolding had not started or that API projects were
  still missing. Documented .NET 10, xUnit, actual project dependencies, and
  reproducible build/test commands.
- Documented API launch profiles from configuration, distinguishing the host and
  OpenAPI registration from business functionality. Launch instructions were
  reviewed against configuration; a live HTTP smoke test was not performed.
- Preserved the distinction between proposed frontend/database choices and
  implemented decisions. Translated a remaining Spanish brainstorming line.

Representative reviewed scaffold (`src/backend/BlaChallenge.Api/Program.cs`;
pre-existing code, not generated by this review):

```csharp
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
```

### Validation and limitations

- `dotnet --version`: `10.0.402`.
- `dotnet build src/backend/BlaChallenge.slnx`: succeeded with zero warnings and
  zero errors, including package restore.
- `dotnet test src/backend/BlaChallenge.slnx --no-build`: no tests available in
  any of the four test assemblies. This does not establish functional coverage.
- Authentication and task ownership edge cases cannot yet be validated because
  those features and their tests have not been implemented.

## Documentation Review 002

### Prompt

The user asked for a brief implementation plan for a Task Management API based
on `docs/challenge.md`, followed by an update to the project documentation.

### Output and review

- Reviewed `docs/challenge.md`, `docs/implementation-plan.md`,
  `docs/brainstorming.md`, `AGENTS.md`, and the existing GenAI record.
- Updated the implementation plan with the remaining workflow: confirm scope and
  rules, define the HTTP contract, implement persistence and use cases with TDD,
  integrate the frontend, verify the demo, and prepare the presentation.
- Recorded SQLite with `Microsoft.Data.Sqlite` and parameterized SQL, a single API
  host with separate controllers, private user-owned tasks, and React with Vite
  as proposals rather than implemented facts.
- Kept JWT Bearer versus cookie authentication, task statuses, and description and
  due-date validation as pending decisions because the user has not selected them.
- Preserved the challenge prohibition on Entity Framework, Dapper, and Mediator.

No application code, configuration, tests, or runtime validations were created
or performed during this documentation review. Pending decisions require human
confirmation before they become project commitments.

## Authentication Iteration 001

### User prompt

The user requested implementation of the first functional backend iteration for
BLA Challenge after reviewing `AGENTS.md`, `docs/challenge.md`,
`docs/implementation-plan.md`, `docs/development-log.md`, this file, and
`README.md`. The requested scope was user registration and authentication only,
with .NET 10, Clean Architecture, one controller-based API host, SQLite through
`Microsoft.Data.Sqlite` with parameterized SQL, JWT Bearer authentication, xUnit
TDD, OpenAPI, and no Entity Framework, Dapper, or Mediator. The user also
required continuous updates to the project documentation and explicitly said
not to implement tasks or the frontend.

No additional user prompts were used to make technical decisions during this
iteration.

### Proposals, decisions, and implementation

- The implementation used a `User` domain entity, Application interfaces and
  use cases, a SQLite repository and initializer, and API controllers. This was
  accepted because it preserves the existing inward-facing project references.
- Passwords are stored as PBKDF2-derived hashes with random salts and compared
  using a fixed-time comparison. Plaintext passwords are not persisted.
- JWT configuration requires a key supplied outside the repository and at least
  32 characters long. No secret or demo credential was added to source control.
- The challenge's ambiguous “second API” wording was handled as separate
  authentication endpoints in the existing single API host. No second service
  was introduced.

Representative implemented code (`BlaChallenge.Infrastructure/Authentication/PasswordHasher.cs`):

```csharp
var salt = RandomNumberGenerator.GetBytes(SaltSize);
var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySize);
return $"v1.{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
```

### Corrections and validation

- The initial application test run failed to compile because the authentication
  contracts and use cases had not been implemented. Those were added before the
  next run.
- A test assertion initially accessed the registration result at the wrong
  level; it was corrected to use the nested user response.
- The first full test run found temporary SQLite database files still locked
  during teardown because connection pooling was active. Tests now clear SQLite
  pools before deleting temporary databases.
- `dotnet build src/backend/BlaChallenge.slnx` passed with zero warnings and zero
  errors.
- `dotnet test src/backend/BlaChallenge.slnx` passed 13 tests across Domain,
  Application, Infrastructure, and API test projects.
- API integration tests verified public ping, unauthorized protected access,
  registration, login, JWT-authenticated `me`, and invalid-password handling.

### Limitations and pending work

- A live API process smoke test was performed with a temporary SQLite path and
  an externally supplied JWT key. It returned `200` for `/api/auth/ping` and
  `/openapi/v1.json`; temporary files were removed afterward.
- Scalar UI was not added; the existing development-only OpenAPI document remains
  available at `/openapi/v1.json`.
- Task models, task CRUD, ownership enforcement for task records, frontend
  integration, seeded demo data, and presentation materials remain pending.

## Documentation Review 003

### Prompt

The user requested applying the simpler backend folder structure used for
application operations and recording it as a repository guideline in `docs/`.

### Output and corrections

- Moved command files directly under `Users/Commands` and query files directly
  under `Users/Queries`.
- Preserved descriptive file and type names while removing operation-specific
  subfolders and updating namespaces and consuming directives.
- Recorded the rule in `docs/contributing.md` and reflected the current layout
  in `docs/implementation-plan.md`.

This was the intermediate layout recorded before the subsequent operation-folder
refactor in Documentation Review 004.

### Validation

- Ran `dotnet restore src/backend/BlaChallenge.slnx` successfully.
- Ran `dotnet build src/backend/BlaChallenge.slnx --no-restore` successfully with
  zero warnings and zero errors.
- Ran `dotnet test src/backend/BlaChallenge.slnx --no-build`; all 14 tests passed
  across the four test projects.

## Documentation Review 004

### Prompt

The user requested grouping each Application operation into its own folder and
placing its command or query and use case in conventionally named files, with
the rule documented in the repository.

### Output and corrections

- Grouped login and registration under `Users/Commands/{Operation}/`.
- Grouped the current-user query under `Users/Queries/GetCurrentUser/`.
- Used `Command.cs`, `Query.cs`, and `UseCase.cs` file names as applicable,
  preserving the existing public type names and behavior.
- Updated the current repository guidance in `docs/contributing.md` and
  `docs/implementation-plan.md`.

This review superseded the intermediate flat operation layout recorded in
Documentation Review 003. The current convention is one folder per operation,
with the conventional file names described above.

### Validation

- Ran `dotnet restore src/backend/BlaChallenge.slnx` successfully.
- Ran `dotnet build src/backend/BlaChallenge.slnx --no-restore` successfully with
  zero warnings and zero errors.
- Ran `dotnet test src/backend/BlaChallenge.slnx --no-build`; all 14 tests passed
  across the four test projects.
- Confirmed the Application operation files use the documented folder and file
  naming convention.

## Documentation Review 005

### Prompt

The user requested a repository-wide C# convention requiring the namespace on
the first line, followed by a blank line and ordered, necessary `using`
directives, with enforcement through `.editorconfig` or equivalent tooling. The
scope of the root `.editorconfig` also needed to be documented explicitly.

### Output and corrections

- Added the root `.editorconfig` with file-scoped namespace, using placement,
  sorting, and unused-import diagnostics.
- Clarified that the root `.editorconfig` intentionally applies to the backend
  and other repository content rather than being limited to `src/backend`.
- Applied the layout to backend production and test files and removed unused
  imports identified by formatting.
- Converted the API `Program.cs` top-level statements to an explicit
  `Program.Main` entry point because C# does not allow a namespace declaration
  before top-level statements.
- Documented the convention, the `dotnet format` verification command, and the
  configuration file location and scope in `docs/contributing.md` and
  `docs/implementation-plan.md`.

### Validation

- Ran `dotnet format src/backend/BlaChallenge.slnx --no-restore` successfully.
- Ran `dotnet build src/backend/BlaChallenge.slnx --no-restore` successfully with
  zero warnings and zero errors.
- Ran `dotnet test src/backend/BlaChallenge.slnx --no-build`; all 14 tests passed
  across the four test projects.
- `git diff --check` reported no whitespace errors.

## Documentation Review 006

### Prompt

The user requested a convention affecting `src/backend` and `docs`: classes
should use primary constructors whenever possible, except when that would make
the code or logic more complex instead of simpler.

### Output and corrections

- Inspected backend production and test classes before changing constructors.
- Converted suitable dependency-injected classes in Application, Infrastructure,
  and API to primary constructors.
- Kept traditional constructors or existing declarations for the `User` domain
  entity, configuration and exception types, and classes where primary
  constructors would obscure validation, initialization, inheritance, or logic.
- Added `csharp_style_prefer_primary_constructors = true:suggestion` and the
  suggestion-level `IDE0290` diagnostic to the root `.editorconfig`.
- Documented the convention and its exception criteria in
  `docs/contributing.md` and `docs/implementation-plan.md`.

### Validation

- Reviewed the changed constructors and their dependency usage manually.
- Ran `dotnet format src/backend/BlaChallenge.slnx --no-restore
  --verify-no-changes` successfully.
- Ran `dotnet build src/backend/BlaChallenge.slnx --no-restore` successfully
  with zero warnings and zero errors.
- Ran `dotnet test src/backend/BlaChallenge.slnx --no-build`; all 14 tests
  passed across the four test projects.
- `git diff --check` reported no whitespace errors.

## Documentation Review 007

### Prompt

The user asked whether controller endpoints and records should be grouped in
`#region` blocks for readability and maintainability.

### Output and corrections

- Recommended against using `#region` blocks as the primary organization
  mechanism because they can hide the file structure without improving
  maintainability.
- Recommended keeping small records that are exclusive to a controller beside
  it, and moving records to separate files when they are reused or grow enough
  to make the controller harder to read.
- Documented the convention in `docs/contributing.md` and
  `docs/implementation-plan.md`.

### Validation

- Confirmed the convention was not already explicitly documented before adding
  it.
- Reviewed the documentation changes for consistency with the existing backend
  organization rules.

## Documentation Review 008

### Prompt

The user asked for an analysis of `src/backend/` and for the documentation in
`docs/` to be updated wherever it no longer matched the implementation.

### Output and corrections

- Compared the solution structure, project references, API controller, startup
  configuration, SQLite initializer, JWT configuration, launch profiles, and
  test projects with `README.md`, `docs/challenge.md`,
  `docs/implementation-plan.md`, `docs/development-log.md`, and this record.
- Corrected the formatting documentation to match the actual split between the
  repository-wide `.editorconfig` and `src/backend/.editorconfig`.
- Added `.gitattributes` after validation showed that the local
  `core.autocrlf=true` setting could re-materialize CRLF in backend files.
- Added setup guidance for the API project's configured .NET user-secrets
  identifier and documented the current JWT defaults.
- Kept pending requirements explicitly pending: task CRUD, the frontend, seeded
  demo data, and presentation materials are not implemented in the backend.

Representative reviewed configuration (`src/backend/.editorconfig`):

```ini
[*]
indent_size = 4

[*.cs]
csharp_style_namespace_declarations = file_scoped:error
dotnet_diagnostic.IDE0005.severity = error
```

### Validation and limitations

- Reviewed all backend source, project, configuration, and test files matched by
  the repository structure before editing the documentation.
- Confirmed the documented API endpoints and JWT configuration against the
  controller and startup code.
- No backend behavior was changed during this review.
- `dotnet format src/backend/BlaChallenge.slnx --no-restore
  --verify-no-changes` passed after LF normalization.
- `dotnet build src/backend/BlaChallenge.slnx --no-restore` passed with zero
  warnings and zero errors.
- `dotnet test src/backend/BlaChallenge.slnx --no-build` passed 14 tests across
  the four test projects.
- `git diff --check` passed and all tracked files were confirmed to use LF line
  endings.

## Documentation Review 009

### Prompt

The user asked for documentation explaining what the application is testing.

### Output and review

- Reviewed every current xUnit test file in the four backend test projects before
  writing the test inventory.
- Added `docs/testing.md` with the test strategy, behavior covered by each layer,
  reproducible commands, and explicit gaps.
- Avoided claiming frontend coverage, seeded-demo coverage, or a percentage
  coverage target because those are not implemented or published yet.
- Linked the new guide from `README.md`.

### Validation and corrections

- The test count and documented scenarios were checked against the test source
  files rather than inferred from the challenge requirements.
- The full solution test command was run after the documentation change; its
  result is recorded in the development log.

## Documentation Review 010

### Prompt

The user requested that `docs/testing.md` identify the method that performs each
documented test, allowing the inventory to use tables instead of bullet points.

### Output and review

- Re-read all current xUnit test files and mapped every behavior to its exact
  test file and method name.
- Replaced the grouped behavior lists in `docs/testing.md` with tables for the
  Domain, Application, Infrastructure, and API test projects.
- Documented the distinction between test methods and executed test cases for
  the parameterized domain theory.

### Validation

- Confirmed that all 26 test methods are represented and that the latest test
  run still reports 27 executed cases.
- No application behavior or test implementation was changed.
