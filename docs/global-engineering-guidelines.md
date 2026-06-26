# AtomUI.Base Global Engineering Guidelines

This document is the global engineering rules entry point for AtomUI.Base.

## Documentation Placement

- Keep `AGENTS.md` small and route detailed rules to `docs/`.
- Put architecture documentation under `docs/architecture/`.
- Put engineering process and coding rules under `docs/engineering/`.
- Put development plans under `docs/superpowers/plans/`.
- Do not put architecture or design documents under `docs/superpowers/`.

## Change Discipline

- Understand the affected module before changing code.
- Keep changes scoped to the requested project boundary.
- Prefer root-cause fixes over trigger-point patches.
- Preserve public API compatibility unless a breaking change is explicitly approved.
- Do not hand-edit generated files; change the generator or source input instead.

## Verification

Choose verification based on the touched area:

- Build system or project structure: `dotnet build AtomUI.Base.slnx`.
- Runtime library behavior: `dotnet test tests/AtomUI.Base.Tests/AtomUI.Base.Tests.csproj --framework net10.0`.
- Source generator changes: add generator tests before changing generator behavior.
- Every code or project-file change: run `git diff --check`.
