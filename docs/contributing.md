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
