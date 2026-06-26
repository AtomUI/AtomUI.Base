# AtomUI.Base Architecture Overview

AtomUI.Base is the foundation repository for shared AtomUI infrastructure. The first version contains a runtime package and a source generator package.

## Projects

| Project | Role |
|---|---|
| `AtomUI.Base` | Runtime foundational package for AtomUI ecosystem code. |
| `AtomUI.Base.Generator` | Roslyn analyzer/source generator package skeleton consumed as an analyzer. |
| `AtomUI.Base.Tests` | Tests for runtime package behavior and future regression coverage. |

## Boundaries

- `AtomUI.Base` may reference `AtomUI.Base.Generator` only as an analyzer.
- `AtomUI.Base.Generator` must not depend on `AtomUI.Base`.
- Tests may reference both runtime and generator projects.
