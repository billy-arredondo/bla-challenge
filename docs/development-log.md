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
