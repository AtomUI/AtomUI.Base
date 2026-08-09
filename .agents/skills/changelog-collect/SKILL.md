---
name: atomui-foundation-changelog-collect
description: Use when collecting AtomUI.Foundation changelog entries from git history, staged changes, issue references, or release notes and turning them into a concise Markdown changelog draft grouped by change type.
---

# AtomUI.Foundation Changelog Collect

Use this skill to collect user-facing changes for AtomUI.Foundation release notes or a changelog draft.

## Workflow

1. Inspect the requested commit range, tag range, branch, PR, or staged changes.
2. Read relevant diffs and commit messages.
3. Group entries as `Added`, `Changed`, `Fixed`, `Performance`, `Docs`, and `Build`.
4. Keep entries concise and user-facing.
5. Include issue or PR references when available.
6. Report uncertainty when a change looks incomplete or commit messages are unclear.

## Output

Produce a Markdown changelog draft and mention the source range used to build it.
