# Git Naming Conventions

Use English for branch names, commit messages, PR titles, and PR descriptions.

## Branches

- Format: `<type>/<short-kebab-case-description>`.
- Allowed types: `feat`, `fix`, `docs`, `test`, `refactor`, `chore`.
- Example: `feat/task-crud`.

## Commits

- Format: `<type>: <imperative summary>`; add an optional scope when useful: `<type>(<scope>): <imperative summary>`.
- Use the same types as branch names. Keep the subject short, lowercase after the colon, and without a trailing period.
- Examples: `docs: add challenge requirements`, `feat(api): create task endpoint`.

## Pull requests

- Title: use the same format as a commit subject, describing the overall change.
- Description: include `Summary` (what and why) and `Validation` (commands run and results, or `Not run` with a reason).

## Backend Feature Organization

- Organize application operations by layer and feature context: place command
  folders under `Commands/` and query folders under `Queries/`.
- Give each operation its own folder, such as `Commands/LoginUser/` or
  `Queries/GetCurrentUser/`.
- Within an operation folder, use the conventional file names `Command.cs`,
  `Query.cs`, and `UseCase.cs` as applicable. Keep descriptive operation names
  in the containing folder and in the type names.

## C# File Layout

- Put the file-scoped `namespace` declaration on the first line of every `.cs`
  file.
- Leave one blank line after the namespace, then place the ordered `using`
  directives.
- Keep only required `using` directives; `src/backend/.editorconfig` marks
  unnecessary imports as errors and sorts system directives first for backend
  C# files.
- Run `dotnet format src/backend/BlaChallenge.slnx --no-restore
  --verify-no-changes` before submitting changes.

The repository-wide `.editorconfig` provides UTF-8, LF line endings, and
two-space indentation defaults. The backend override uses four-space
indentation and contains the backend-specific C# layout and diagnostic rules.
`.gitattributes` also keeps Git checkouts on LF regardless of the local
`core.autocrlf` setting.

## Primary Constructors

- Prefer primary constructors for classes when they simplify dependency
  initialization and keep the class easier to read.
- Use a traditional constructor when a primary constructor would make the
  validation, initialization, inheritance, or overall logic more complex.
- Apply this convention to C# code under `src/backend` and document notable
  exceptions in the surrounding code or review record when useful.

## API File Organization

- Do not use `#region` blocks as the primary way to organize controllers.
- Keep small request and response records next to a controller when they are
  specific to its endpoints and remain easy to understand.
- Move records to separate files when they are reused by multiple endpoints or
  become large enough to make the controller harder to read.
