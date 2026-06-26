# AtomUI.Base AI Collaboration Guidelines

## Scope Control

- Keep changes focused on the requested module or workflow.
- Do not introduce Gallery, application hosts, control packages, or tools unless requested.
- Preserve the repository documentation boundary: architecture docs under `docs/architecture/`, engineering rules under `docs/engineering/`, plans under `docs/superpowers/plans/`.

## Verification

- Run the narrowest useful command while iterating.
- Run `dotnet build AtomUI.Base.slnx` before reporting project structure or build-system work complete.
- Run `dotnet test tests/AtomUI.Base.Tests/AtomUI.Base.Tests.csproj --framework net10.0` before reporting runtime behavior complete.
- Run `git diff --check` before reporting any file-editing work complete.
