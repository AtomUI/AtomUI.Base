# AtomUI.Base Project Structure Design

## Context

`AtomUI.Base` is the foundational library repository for the AtomUI ecosystem. The repository is currently empty except for Git metadata and an untracked `.idea/` directory. Its `AGENTS.md` entry points to `docs/global-engineering-guidelines.md`, but that file does not exist yet.

`AtomUIV6` provides the reference structure. Its build system uses lightweight root MSBuild files, a `build/` props directory, Central Package Management, `.slnx`, `src/`, `tests/`, `docs/`, `resources/`, and a project-local `.agents/skills` workflow directory.

## Goals

- Create an AtomUIV6-style repository structure for `AtomUI.Base`.
- Keep the first version complete enough to build, test, and pack.
- Avoid copying AtomUIV6 Gallery, tools, control-library, or migration-specific structure before it is needed.
- Add a source generator skeleton project, but do not implement real generator behavior yet.
- Add project-local agent skills for reusable repository workflows.
- Provide durable documentation entry points for architecture, build, engineering, and generator rules.

## Non-Goals

- No `tools/` directory in the first version.
- No Gallery, sample app, browser host, desktop host, or control showcase.
- No Avalonia control package structure.
- No real source generator implementation beyond a compileable skeleton.
- No Gallery release workflow or platform application packaging workflow in this pass.

## Chosen Approach

Use an AtomUIV6-style Base minimum complete structure:

```text
AtomUI.Base/
├── .agents/
│   └── skills/
├── .github/
│   └── workflows/
├── AGENTS.md
├── AtomUI.Base.slnx
├── Directory.Build.props
├── Directory.Build.targets
├── Directory.Packages.props
├── global.json
├── build/
├── docs/
├── resources/
├── src/
└── tests/
```

This keeps the repository shape familiar to AtomUI maintainers while limiting first-pass implementation to `AtomUI.Base`, `AtomUI.Base.Generator`, and `AtomUI.Base.Tests`.

## Build System

The root build system mirrors AtomUIV6's lightweight entry pattern:

- `global.json` pins SDK `10.0.300` with `rollForward` set to `latestFeature`.
- `Directory.Build.props` enables nullable reference types, implicit usings, and latest C# language version, then imports:
  - `build/Version.props`
  - `build/Common.props`
  - `build/PackageMetaInfo.props`
  - `build/Output.props`
- `Directory.Packages.props` enables Central Package Management.
- `Directory.Build.targets` removes `*.csproj.DotSettings` from `None` items.
- `build/Version.props` defines `AtomUIBaseVersion` as `1.0.0-alpha.1`.
- `build/Common.props` defines target framework strategy:
  - `AtomUIBaseDevelopTargetFramework=net10.0`
  - `AtomUIBaseProductionTargetFramework=net8.0`
  - Debug builds target `net10.0`.
  - Release builds target `net10.0;net8.0`.
- `build/PackageMetaInfo.props` centralizes package metadata and defaults `PackageId` to `$(MSBuildProjectName)`.
- `build/Output.props` redirects package, binary, and intermediate outputs into `output/`.

Package metadata uses:

- `Title`: `AtomUI.Base`
- `RepositoryUrl`: `https://github.com/AtomUI/AtomUI.Base`
- `Version`: `$(AtomUIBaseVersion)`
- Authors/company/license/readme/logo conventions aligned with AtomUIV6 where local files exist.

## Project Structure

Create these first-version projects:

```text
src/
├── AtomUI.Base/
│   ├── AtomUI.Base.csproj
│   └── BaseAssemblyMarker.cs
└── AtomUI.Base.Generator/
    ├── AtomUI.Base.Generator.csproj
    └── AtomUIBaseGeneratorMarker.cs

tests/
└── AtomUI.Base.Tests/
    ├── AtomUI.Base.Tests.csproj
    └── BaseAssemblyMarkerTests.cs
```

### AtomUI.Base

`AtomUI.Base` is the main foundational library package.

- Uses `TargetFrameworks=$(AtomUIBaseTargetFrameworks)`.
- Uses `RootNamespace=AtomUI.Base`.
- References `AtomUI.Base.Generator` as an analyzer:

```xml
<ProjectReference Include="../AtomUI.Base.Generator/AtomUI.Base.Generator.csproj"
                  OutputItemType="Analyzer"
                  ReferenceOutputAssembly="false"
                  PrivateAssets="all" />
```

- Contains only `BaseAssemblyMarker` initially so the project has a stable compileable public surface.

### AtomUI.Base.Generator

`AtomUI.Base.Generator` is a source generator skeleton.

- Targets `netstandard2.0`.
- Sets:
  - `IsRoslynComponent=true`
  - `EnforceExtendedAnalyzerRules=true`
  - `SuppressDependenciesWhenPacking=true`
  - `IncludeBuildOutput=false`
  - `IncludeInPublish=false`
  - analyzer compatibility properties disabled where appropriate, matching AtomUIV6 generator style.
- References Roslyn packages from Central Package Management.
- Packs the built DLL under `analyzers/dotnet/cs`.
- Contains only a marker type in the first version; no `ISourceGenerator` or incremental generator is implemented yet.

### AtomUI.Base.Tests

`AtomUI.Base.Tests` validates the initial build/test chain.

- Targets `net10.0`.
- Is not packable.
- Uses xUnit v3, Shouldly, and Microsoft.NET.Test.Sdk from Central Package Management.
- References `AtomUI.Base`.
- References `AtomUI.Base.Generator` as an analyzer.
- Adds a marker test proving the main assembly can be referenced.

## Solution

Create `AtomUI.Base.slnx` with:

```xml
<Solution>
  <Project Path="src/AtomUI.Base/AtomUI.Base.csproj" />
  <Project Path="src/AtomUI.Base.Generator/AtomUI.Base.Generator.csproj" />
  <Project Path="tests/AtomUI.Base.Tests/AtomUI.Base.Tests.csproj" />
</Solution>
```

## Documentation Structure

Create these documentation entry points:

```text
docs/
├── architecture/
│   ├── build-and-packaging.md
│   ├── dependency-graph.md
│   └── overview.md
├── engineering/
│   ├── agent-guidelines.md
│   └── source-generator-guidelines.md
└── global-engineering-guidelines.md
```

`AGENTS.md` stays small. It should link to `docs/global-engineering-guidelines.md` and route readers to the architecture, build, dependency, agent, and source generator documents.

`docs/global-engineering-guidelines.md` becomes the durable global entry point required by the current `AGENTS.md` instruction.

## Local Agent Skills

Create a project-local `.agents/skills` directory based on AtomUIV6, but only include reusable workflow skills:

```text
.agents/
└── skills/
    ├── changelog-collect/
    │   └── SKILL.md
    ├── commit-msg/
    │   ├── SKILL.md
    │   └── references/
    │       └── commit-message-convention.md
    ├── create-pr/
    │   └── SKILL.md
    ├── issue-reply/
    │   └── SKILL.md
    ├── upgrade-dependencies/
    │   └── SKILL.md
    └── version-release/
        └── SKILL.md
```

Adapt skill wording from AtomUIV6 to `AtomUI.Base`:

- Repository name and package names use `AtomUI.Base`.
- Version files point to `build/Version.props` and `Directory.Packages.props`.
- Validation commands use `dotnet build AtomUI.Base.slnx`, `dotnet test tests/AtomUI.Base.Tests/AtomUI.Base.Tests.csproj --framework net10.0`, and `git diff --check`.
- Commit and PR guidance keeps AtomUI-style conventional commit patterns.

Do not migrate these AtomUIV6-specific skills in the first version:

- `update-gallery-maintain-docs`
- `atomui-control-optimization`
- `atomui-control-performance`
- `create-skeleton-app`
- `migrate-to-avalonia12`
- `atomui-resource-lifecycle`

## GitHub Workflows

Create a `.github/workflows/release-atomui-base.yml` workflow adapted from AtomUIV6's `release-atomui.yml`.

The workflow should keep the same release pipeline shape:

- `workflow_dispatch` only.
- Inputs:
  - `TargetBranch`, default `release/1.0`.
  - `BuildConfiguration`, default `Release`.
  - `PublishToNuget`, default `false`.
- Environment variables:
  - `SOURCE_DIR=${{ github.workspace }}`
  - `BASE_OUTPUT_DIR=${{ github.workspace }}/output`
  - `LOCAL_NUGET_DIR=${{ github.workspace }}/nuget`
- Single Windows job for NuGet package release validation.
- Use `actions/checkout@v6` with the selected target branch.
- Use `actions/setup-dotnet@v5` with `dotnet-version: 10.0.x`.
- Create a local NuGet feed and add it with `dotnet nuget add source`.
- Build `AtomUI.Base.slnx` using the selected configuration.
- Run `AtomUI.Base.Tests` on `net10.0`.
- Pack:
  - `src/AtomUI.Base/AtomUI.Base.csproj`
  - `src/AtomUI.Base.Generator/AtomUI.Base.Generator.csproj`
- Push built packages into the local NuGet feed to catch package validity problems before publishing.
- Upload `*.nupkg` artifacts with `actions/upload-artifact@v7`.
- Publish to nuget.org only when `PublishToNuget=true`, using `secrets.NUGET_API_KEY` and `--skip-duplicate`.

Do not create `release-gallery.yml`, app packaging workflows, matrix runtime packaging, or native installer packaging in this first version.

Do not add `.github/copilot-instructions.md` in the first version unless a later request asks for Copilot-specific guidance. The repository-level agent entry remains `AGENTS.md`.

## Resources

Create `resources/.gitkeep` so the directory exists without introducing package assets before the logo/readme/license files are available. Package metadata should only include asset files when they exist locally.

## Verification

After implementation, run:

```bash
dotnet build AtomUI.Base.slnx
dotnet test tests/AtomUI.Base.Tests/AtomUI.Base.Tests.csproj --framework net10.0
git diff --check
```

If package metadata references local package assets, also run a pack verification after those files exist:

```bash
dotnet pack src/AtomUI.Base/AtomUI.Base.csproj -c Release
```

After adding the workflow, YAML should be reviewed against AtomUIV6's release workflow structure and checked for project-path drift.

## Risks And Decisions

- `AtomUI.Base.Generator` is intentionally a skeleton. Adding a real generator should happen in a later design with tests for generated output.
- Package icon, readme, and license should not be included as package assets until actual files exist in this repository.
- The repository starts with `.idea/` untracked. This design does not require changing or deleting it.
- Because the repository has no existing commits, the first implementation commit will establish the baseline structure.
- The Base release workflow is intentionally NuGet-only. Gallery/application release workflows should be designed separately when such applications exist.
