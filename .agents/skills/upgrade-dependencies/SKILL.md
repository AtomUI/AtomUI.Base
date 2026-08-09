---
name: atomui-foundation-upgrade-dependencies
description: Use when upgrading AtomUI.Foundation NuGet dependencies, Roslyn packages, test packages, or any third-party version where compatibility must be evaluated before implementation.
---

# AtomUI.Foundation Dependency Upgrade

## Rules

- Do not modify dependency versions during evaluation.
- Upgrade one dependency family at a time unless the user approves grouping.
- Check `build/Version.props`, `Directory.Packages.props`, and affected project files.
- Preserve AtomUI.Foundation behavior and package boundaries.

## Evaluation

Report:

- Current version and target version.
- Owning version file.
- Source or release-note evidence used.
- AtomUI.Foundation usage impact from code search.
- Required code or test changes.
- Verification commands.

## Execution

After user approval, update version declarations, apply compatibility changes, run build/tests, and report residual risk.
