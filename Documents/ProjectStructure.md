# Project Structure

## Solution: Dimenship.sln

```
Dimenship/
├── Dimenship.sln
│
├── Documents/                          ← this folder
│   └── ProjectStructure.md
│
├── Art/                                ← concept art assets
│   ├── concept art dimenship.png
│   ├── 3rd bot concept.png
│   └── ship.jpg
│
│── [Legacy – net6.0] ──────────────────────────────────────
│
├── DimenshipBase/                      net6.0 class library
│   ├── DimenshipSystem.cs              top-level system entry point
│   ├── GameTime.cs                     game clock
│   ├── Facilities.cs                   facility definitions
│   ├── Process.cs                      production process model
│   ├── Storage.cs                      inventory storage
│   ├── StaticData.cs                   JSON-backed static data loader
│   ├── SerializationExtension.cs       DataContractJson helpers
│   ├── ISystemStateSet.cs              subsystem access interface
│   ├── InstructionBase.cs              base class for instructions
│   ├── Broker/
│   │   └── SimpleBroker.cs
│   ├── FungibleItems/
│   │   └── ComponentItems.cs
│   ├── Production/
│   │   ├── ProductionPlanner.cs
│   │   └── ProductionSteps.cs
│   └── SubSystems/
│       ├── BotsSubSystem.cs
│       ├── FacilitySubSystem.cs
│       ├── ItemStorageSubSystem.cs
│       ├── NotificationSubSystem.cs
│       ├── ProcessSubSystem.cs
│       └── ResearchSubSystem.cs
│
├── DimenshipBaseTests/                 net6.0 NUnit 3 test suite
│   ├── StaticDataIntegrityTest.cs
│   ├── SerializationTests.cs
│   ├── ProcessTests.cs
│   ├── FacilitiesTests.cs
│   ├── RunningTests.cs
│   ├── BotAssemblyTests.cs
│   ├── SystemTestHelper.cs
│   ├── Utils.cs
│   └── Data/
│       ├── StaticDataGenerated.json
│       ├── StaticDataRegression.json
│       └── StaticDataTests.json
│
├── DimenshipConsole/                   net6.0 console app (dev harness)
│   ├── Program.cs
│   ├── UserConsole.cs
│   ├── Initializer.cs
│   ├── SystemRunner.cs
│   ├── Commands/
│   │   ├── ICommand.cs
│   │   ├── BuildCommand.cs
│   │   ├── ListCommand.cs
│   │   └── ProcessesCommand.cs
│   └── Data/
│       └── StaticDataGenerated.json
│
│── [New layer – net10.0] ───────────────────────────────────
│
├── DcCoreLib/                          net10.0 class library
│   │                                   Shared utilities; may be referenced
│   │                                   by projects outside this solution.
│   ├── Result.cs                       Result<T> — success/failure union
│   ├── Guard.cs                        Argument validation helpers
│   └── IdGenerator.cs                  Thread-safe monotonic int IDs
│
├── DcCoreLib.Tests/                    net10.0 NUnit 4 test suite
│   ├── ResultTests.cs
│   └── GuardTests.cs
│
├── DimenshipCommon/                    net10.0 class library
│   │                                   Data classes stored in cloud;
│   │                                   repository interfaces for access.
│   │                                   References: DcCoreLib
│   ├── Data/
│   │   ├── GameStateDto.cs             Cloud-persisted game state
│   │   └── PlayerDto.cs                Player profile
│   └── Interfaces/
│       ├── IGameStateRepository.cs     Load / Save / Delete game state
│       └── IPlayerRepository.cs        GetById / Upsert player
│
├── DimenshipLogic/                     net10.0 class library
│   │                                   Core game logic.
│   │                                   References: DcCoreLib, DimenshipCommon
│   ├── Game/
│   │   ├── IGameContext.cs             Subsystem access interface
│   │   └── GameContext.cs              Service-registry implementation
│   ├── Units/
│   │   ├── UnitDefinition.cs           Static unit class (id, name, tags)
│   │   └── UnitInstance.cs             Live unit with durability
│   └── Production/
│       └── ProductionOrder.cs          Queued production order
│
├── DimenshipLogic.Tests/               net10.0 NUnit 4 test suite
│   │                                   References: DimenshipLogic
│   ├── Game/
│   │   └── GameContextTests.cs
│   └── Units/
│       └── UnitInstanceTests.cs
│
└── DimenshipUnity/                     Unity 6 LTS project (2D strategy UI)
    │                                   Opened via Unity Hub — not dotnet-built.
    │                                   Editor version: 6000.0.32f1
    ├── Assets/
    │   └── Scripts/
    │       └── DimenshipUnity.asmdef   Assembly definition for game scripts
    ├── Packages/
    │   └── manifest.json               ugui, physics2d, tilemap, ui modules
    └── ProjectSettings/
        └── ProjectVersion.txt
```

## Dependency Graph

```
DcCoreLib
    ├── DcCoreLib.Tests
    ├── DimenshipCommon
    │       ├── DimenshipLogic
    │       │       └── DimenshipLogic.Tests
    │       └── (future cloud backend projects)
    └── DimenshipLogic

DimenshipBase  (legacy, standalone)
    ├── DimenshipBaseTests
    └── DimenshipConsole

DimenshipUnity  (Unity Editor build, references DimenshipLogic DLLs via Plugins)
```

## Framework & Tooling

| Group | Target | Test framework |
|---|---|---|
| Legacy projects | net6.0 | NUnit 3.13 |
| New projects | net10.0 | NUnit 4.2 |
| DimenshipUnity | Unity 6 (C# 9) | Unity Test Framework |

## Notes

- The solution file (`Dimenship.sln`) contains all projects. `DimenshipUnity` is
  registered as a **Solution Folder** entry so it appears in the IDE tree but is
  not built by `dotnet build`.
- The new net10.0 layer does **not** reference the legacy net6.0 projects.
  `DimenshipBase` and the new projects are separate, parallel implementations
  that will be consolidated over time.
- `DcCoreLib` is intentionally dependency-free so it can be packaged as a
  standalone NuGet library for use in other projects.
