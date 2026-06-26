# AtomUI.Base Git Commit Message Convention

## Format

```text
<type>(<scope>): <subject>
```

or:

```text
<type>: <subject>
```

## Types

| Type | Use |
|---|---|
| `feat` | New runtime API, generator capability, or user-facing behavior |
| `fix` | Bug fix |
| `refactor` | Code restructuring without behavior change |
| `docs` | Documentation only |
| `style` | Formatting only |
| `perf` | Performance improvement |
| `test` | Tests |
| `build` | MSBuild, package, or workflow changes |
| `chore` | Maintenance |
| `ci` | CI/CD configuration |
| `release` | Release preparation |
| `revert` | Revert a previous commit |

## Scopes

Recommended scopes include `Base`, `Generator`, `Packaging`, `deps`, `docs`, and `ci`.
