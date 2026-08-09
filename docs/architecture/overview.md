# AtomUI.Foundation Architecture Overview

AtomUI.Foundation is the foundation repository for shared AtomUI infrastructure. The first version contains a runtime package and a source generator package.

## Projects

| Project | Role |
|---|---|
| `AtomUI.Foundation` | Runtime foundational package for AtomUI ecosystem code. |
| `AtomUI.Foundation.Generator` | Roslyn analyzer/source generator package skeleton consumed as an analyzer. |
| `AtomUI.Foundation.Tests` | Tests for runtime package behavior and future regression coverage. |
| `AtomUI.Foundation.Generator.Tests` | Tests for source generator behavior and diagnostics. |

## Boundaries

- `AtomUI.Foundation` may reference `AtomUI.Foundation.Generator` only as an analyzer.
- `AtomUI.Foundation.Generator` must not depend on `AtomUI.Foundation`.
- Tests may reference both runtime and generator projects.
