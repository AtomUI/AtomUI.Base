---
name: atomui-base-create-pr
description: Use when preparing an AtomUI.Base pull request summary from local git changes, including overview, validation, risks, and reviewer notes.
---

# Create AtomUI.Base PR

## Workflow

1. Inspect `git status --short`.
2. Inspect staged and unstaged diffs for the requested change.
3. Read nearby docs or code when the diff alone does not explain behavior or risk.
4. Summarize behavior and repository impact rather than file churn.
5. Include validation that was actually run.
6. Call out risks, migration notes, or reviewer focus areas when relevant.

## PR Template

```markdown
## Summary
-

## Validation
-

## Notes
-
```
