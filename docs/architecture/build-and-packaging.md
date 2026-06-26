# Build And Packaging

AtomUI.Base uses centralized MSBuild configuration.

## Target Frameworks

`build/Common.props` defines:

- Development target framework: `net10.0`.
- Production target framework: `net8.0`.
- Debug builds target `net10.0`.
- Release builds target `net10.0;net8.0`.

`AtomUI.Base.Generator` targets `netstandard2.0` because analyzer packages must load in compiler contexts beyond the runtime library target.

## Package Management

`Directory.Packages.props` enables Central Package Management:

```xml
<ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
```

Package versions must be declared centrally unless a package has a documented reason to opt out.

## Output Paths

`build/Output.props` redirects build outputs to `output/` so repository source directories stay clean.

## Release Workflow

`.github/workflows/release-atomui-base.yml` builds, tests, packs, pushes packages into a local NuGet feed for validation, uploads package artifacts, and optionally publishes to nuget.org.
