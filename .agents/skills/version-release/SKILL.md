---
name: atomui-foundation-version-release
description: Use when preparing an AtomUI.Foundation version release, including version files, changelog readiness, release commits, package validation, and final release notes.
---

# AtomUI.Foundation Version Release

## Workflow

1. Identify intended version and release scope.
2. Inspect `git status --short`.
3. Inspect `build/Version.props`, package metadata, changelog files, recent tags, and recent commits.
4. Update only files required by the release.
5. Keep versions consistent across MSBuild props, docs, changelog, and release notes.
6. Run release validation commands.
7. Summarize changed files, validation results, and remaining manual release steps.

## Safety

Do not create tags, push commits, publish packages, or delete release artifacts unless the user explicitly asks for that action.
