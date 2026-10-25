# Generative AI Record

This document records how generative AI was used during the repository setup and how its output was reviewed. It will be updated as application code is generated and validated.

## Scope So Far

The work completed so far concerns repository workflow automation. No backend, frontend, database, authentication, or business logic implementation has been generated yet.

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
in the challenge. The API and frontend projects were not added prematurely; they
remain explicit next steps because the current implementation contains only the
backend foundation.

## Prompts and Outcomes

### Branch workflow commands

The request was to create an OpenCode command that creates a Git branch according to `docs/contributing.md`. The requirement was later refined so that `/new-branch` accepts an exact branch name, while `/suggest-branch-names` accepts a purpose description and returns three English candidates.

The resulting commands are:

- `.opencode/commands/new-branch.md`
- `.opencode/commands/suggest-branch-names.md`

The commands enforce the allowed branch types, English names, and short kebab-case descriptions. The creation command validates the exact requested name and does not infer or modify it.

### Commit message command

The request was to create an OpenCode command that reviews staged changes and generates a commit message following the repository conventions.

The resulting command is `.opencode/commands/commit-message.md`. It instructs OpenCode to inspect only `git diff --cached`, ignore unstaged and untracked files, produce one English commit subject, and avoid creating the commit or modifying files.

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

The plugin is located at `.opencode/plugins/branch-commands-model.js` and applies to `/new-branch`, `/suggest-branch-names`, and `/commit-message`.

## Validation and Corrections

- Read `docs/contributing.md` before defining the commands.
- Confirmed that branch types and commit types match the repository conventions.
- Added explicit validation for branch names using a regular expression and `git check-ref-format --branch`.
- Ensured branch suggestions are generated in English even when the purpose is described in another language.
- Ensured commit-message generation considers staged changes only.
- Ran `node --check` against the plugin successfully.
- Ran `git diff --check` successfully.
- Reviewed the staged diff manually before generating a commit message.
- Corrected the plugin name and export after its scope expanded from one command to three commands.

## Edge Cases and Safety

- Empty or vague branch purposes must request clarification.
- Invalid exact branch names must be rejected before Git commands run.
- Existing local branch names must not be reused.
- No custom command may reset, commit, push, or modify project files unless explicitly requested.
- The commit-message command reports when no files are staged.
- If no DeepSeek model is available or the provider lookup fails, the current session model is retained.

## Current Limitations

The dynamic model-selection plugin has passed JavaScript syntax validation, but its OpenCode hook behavior has not yet been verified in a live OpenCode `1.19.4` session. The commands should therefore be tested after restarting OpenCode.

Future entries will document generated application code, test results, authentication decisions, and any corrections made during implementation.
