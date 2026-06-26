# Dependency Graph

This document records the first-version AtomUI.Base project dependency rules.

```mermaid
flowchart TD
    Generator["AtomUI.Base.Generator\nnetstandard2.0 analyzer package"]
    Base["AtomUI.Base\nruntime package"]
    Tests["AtomUI.Base.Tests\nxUnit v3 tests"]

    Generator -. analyzer .-> Base
    Base --> Tests
    Generator -. analyzer .-> Tests
```

## Rules

- `AtomUI.Base.Generator` must stay independent of `AtomUI.Base`.
- `AtomUI.Base` consumes `AtomUI.Base.Generator` with `OutputItemType="Analyzer"` and `ReferenceOutputAssembly="false"`.
- Test projects may reference runtime projects normally and generator projects as analyzers.
