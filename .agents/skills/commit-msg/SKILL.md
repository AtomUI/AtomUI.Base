---
name: atomui-foundation-commit-msg
description: Generate a single-line commit message for AtomUI.Foundation by reading staged changes and recent commit style. Use when the user asks for a commit message, says "msg", "commit msg", "写提交信息", "创建 commit", or wants one-line text that covers staged changes.
---

# AtomUI.Foundation Commit Message Generation

## Workflow

1. Run `git status --short`.
2. Run `git diff --cached --stat`.
3. Run `git diff --cached`.
4. Run `git log --oneline -10 --no-merges`.
5. If there are no staged changes, state that a commit message cannot be generated.
6. Generate one concise line that summarizes the staged change intent.

## Format

Use one of:

```text
<type>(<scope>): <subject>
<type>: <subject>
```

Common types: `feat`, `fix`, `refactor`, `docs`, `style`, `perf`, `test`, `build`, `chore`, `ci`, `release`, `revert`.

Rules:

- Use lowercase type.
- Use English type and scope.
- Use imperative subject.
- Do not end with punctuation.
- Keep the line at 72 characters or fewer when practical.
