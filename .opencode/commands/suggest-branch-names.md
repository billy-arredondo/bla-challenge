---
description: Suggest three branch names from a goal description
---

Suggest exactly three candidate branch names for this goal: $ARGUMENTS

Treat the argument as a description of the branch's purpose, not as an existing branch name. Follow `docs/contributing.md`: use English, the format `<type>/<short-kebab-case-description>`, and only these types: `feat`, `fix`, `docs`, `test`, `refactor`, or `chore`.

Return only a numbered list with three candidates. Keep each description concise, use lowercase kebab case, and do not create, switch, or modify any Git branch. If the goal is empty or too vague, ask for a clearer goal instead of proposing names.
