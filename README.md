# AtomUI.Base

[![License: LGPL-3.0](https://img.shields.io/badge/license-LGPL--3.0-white?labelColor=black&style=flat-square)](LICENSE)

Documentation Language: [English](README.md) | [简体中文](README.zh-CN.md)

## Overview

AtomUI.Base is the foundational infrastructure repository for the AtomUI ecosystem. It provides shared runtime primitives
and source-generation support that other AtomUI packages can build on without depending on a specific control library.

The first release focuses on modularity:

- `AtomUI.Base` contains module identifiers, module descriptors, dependency graph resolution, lifecycle hosting and
  module service registration primitives.
- `AtomUI.Base.Generator` contains Roslyn source generators and diagnostics for module catalog generation.

## Packages

| Package | Description |
|---|---|
| `AtomUI.Base` | Runtime infrastructure package for shared AtomUI foundation APIs. |
| `AtomUI.Base.Generator` | Analyzer/source-generator package for generated AtomUI infrastructure. |

Current prerelease version:

```bash
dotnet add package AtomUI.Base --version 1.0.0-alpha.1
```

If your project needs generated module catalogs, reference the generator as an analyzer:

```xml
<PackageReference Include="AtomUI.Base.Generator" Version="1.0.0-alpha.1" PrivateAssets="all" />
```

## Modularity

Mark module classes with `ModuleAttribute` and derive from `ModuleBase`:

```csharp
using AtomUI.Modularity;

[Module(DisplayName = "Core")]
public sealed partial class CoreModule : ModuleBase
{
}

[Module(DisplayName = "Selection", Dependencies = [typeof(CoreModule)])]
public sealed partial class SelectionModule : ModuleBase
{
}
```

The module catalog generator emits an internal `ModuleGeneratedCatalog` type with a `CreateRegistrations()` method:

```csharp
using AtomUI.Modularity;

var registrations = ModuleGeneratedCatalog.CreateRegistrations();
var host = new ModuleHost(registrations);
var result = await host.InitializeAsync();
```

Generated catalog naming can be customized with MSBuild properties:

```xml
<PropertyGroup>
    <AtomUIModularityCatalogNamespace>MyApp.Generated</AtomUIModularityCatalogNamespace>
    <AtomUIModularityCatalogTypeName>MyModuleCatalog</AtomUIModularityCatalogTypeName>
</PropertyGroup>
```

## Local Development

Build the full solution:

```bash
dotnet build AtomUI.Base.slnx
```

Run runtime tests:

```bash
dotnet test tests/AtomUI.Base.Tests/AtomUI.Base.Tests.csproj --framework net10.0
```

Run generator tests:

```bash
dotnet test tests/AtomUI.Base.Generator.Tests/AtomUI.Base.Generator.Tests.csproj --framework net10.0
```

Publish packages to a local NuGet source:

```powershell
pwsh -File scripts/PublishToLocalSources.ps1
pwsh -File scripts/PublishToLocalSources.ps1 -localSourcesDir /path/to/nuget.local -buildType Release
```

## Repository Layout

| Path | Purpose |
|---|---|
| `src/AtomUI.Base` | Runtime package source. |
| `src/AtomUI.Base.Generator` | Roslyn generator package source. |
| `tests/AtomUI.Base.Tests` | Runtime tests. |
| `tests/AtomUI.Base.Generator.Tests` | Generator tests. |
| `build/` | Centralized MSBuild versioning, package metadata and output configuration. |
| `docs/` | Architecture and engineering documentation. |
| `scripts/` | Local development and publishing scripts. |

## License

AtomUI.Base uses the same license as AtomUI: GNU Lesser General Public License v3.0. See [LICENSE](LICENSE).
