# Source Generator Guidelines

`AtomUI.Base.Generator` is packaged as a Roslyn analyzer.

## Targeting

- Target `netstandard2.0`.
- Use `IsRoslynComponent=true`.
- Package the generator assembly under `analyzers/dotnet/cs`.

## Dependency Rules

- Do not reference `AtomUI.Base` from `AtomUI.Base.Generator`.
- Keep generator dependencies private with `PrivateAssets="all"`.
- Runtime projects consume the generator through analyzer references only.

## Testing

When real generator behavior is added, add focused generator tests before implementation. Tests must verify generated source content and compiler diagnostics.
