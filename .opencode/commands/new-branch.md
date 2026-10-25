---
description: Create an exact branch name from the current HEAD
---

Create a new local Git branch from the currently checked-out branch using this exact requested name: $ARGUMENTS

Do not infer, translate, shorten, or otherwise modify the requested name. Follow `docs/contributing.md`: the exact name must use English and the format `<type>/<short-kebab-case-description>`, with only `feat`, `fix`, `docs`, `test`, `refactor`, or `chore` as the type. If the argument is empty or does not comply, explain the violation and ask for a corrected exact name before running Git commands.

Check the current branch. Before creating anything, ensure the name matches `^(feat|fix|docs|test|refactor|chore)/[a-z0-9]+(-[a-z0-9]+)*$`, passes `git check-ref-format --branch <name>`, and is not an existing local branch. Create and switch to the new branch from the current HEAD with `git switch -c <name>`. Do not reset, commit, push, or modify files. Report the old and new branch names.
