---
description: Generate a commit message from staged changes
---

Generate one commit message for the currently staged changes.

First inspect the staged changes with `git diff --cached --stat` and `git diff --cached`. Do not consider unstaged or untracked files. If there are no staged changes, report that and do not propose a message.

Follow `docs/contributing.md`: write the message in English using `<type>: <imperative summary>` or `<type>(<scope>): <imperative summary>`, using only `feat`, `fix`, `docs`, `test`, `refactor`, or `chore`. Keep the subject short, lowercase after the colon, and without a trailing period. Return only the proposed commit message in a code block. Do not create a commit or modify any files.
