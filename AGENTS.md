# AtomUI.Base Agent Guide

This file gives AI coding agents the stable entry points for working in AtomUI.Base. Keep this file small. Put concrete, detailed rules in focused documents under `docs/`, then link them here.

## Project Profile

AtomUI.Base is the foundational library repository for the AtomUI ecosystem. It contains runtime base infrastructure, source generator infrastructure, tests, build configuration, local workflow skills, and NuGet release automation.

```text
AtomUI.Base/
├── src/
│   ├── AtomUI.Base
│   └── AtomUI.Base.Generator
├── tests/
│   └── AtomUI.Base.Tests
├── docs/
├── build/
├── resources/
├── .agents/
└── .github/
```

## Required Reading

- Global engineering rules: [docs/global-engineering-guidelines.md](docs/global-engineering-guidelines.md)
- Architecture overview: [docs/architecture/overview.md](docs/architecture/overview.md)
- Build and packaging: [docs/architecture/build-and-packaging.md](docs/architecture/build-and-packaging.md)
- Dependency graph: [docs/architecture/dependency-graph.md](docs/architecture/dependency-graph.md)
- Agent collaboration: [docs/engineering/agent-guidelines.md](docs/engineering/agent-guidelines.md)
- Source generators: [docs/engineering/source-generator-guidelines.md](docs/engineering/source-generator-guidelines.md)

## Common Commands

```bash
dotnet build AtomUI.Base.slnx
dotnet test tests/AtomUI.Base.Tests/AtomUI.Base.Tests.csproj --framework net10.0
git diff --check
```
