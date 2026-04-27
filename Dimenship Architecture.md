# Dimenship Big-Chunk Architecture Plan

## Context

Dimenship is an offline-first Android deterministic asynchronous strategy sim (C#/.NET 10). The GDD was refreshed to v0.6 (`C:\Users\gatov\Documents\Dimenship\Dimenship_GDD_Foundation_v0.6.md`), and the existing `DimenshipBase` / `DimenshipConsole` / `DimenshipBaseTests` code under `D:\DEV_Research\dimenship\` is now treated as reference-only while the project is rebuilt. Before writing new code, the goal is to lock down the macro module structure so everything downstream — save format, RNG plumbing, UI boundary, and milestone slicing — has a consistent shape to plug into.

This plan covers **big-chunk architecture only**: module boundaries, responsibilities, and the contracts between chunks. Class-level and method-level design is intentionally deferred.

### Constraints this plan must uphold

- Player-facing planning dominance with system-level replayability via per-action seeded RNG, generated at commit and persisted with the action.
- Single-threaded resolution with a predefined evaluation order.
- Save portability across devices using pinned vendored RNG (not `System.Random`) plus integer/fixed-point math on any RNG-feeding or tick-accumulating path.
- Compute-slot scheduler (~8 base), priority queue, `ComputeDeferred` marking, and blocked-reason reporting.
- Mission resolution where yields and durations are formulaic, and the seed drives only random finds and damage.
- Doctrine implemented as WHEN-THEN rule cards evaluated during resolution; reports must show which rules fired and why.
- Content (`strata`, missions, recipes, modules, doctrine templates, quests) is data-driven.
- Milestone sequence: M1 minimum loop -> M2 variance -> M3 doctrine -> M4 content breadth -> M5 Android UI.

## A. Chunk map

Nine chunks form a clean DAG. Dependencies point downward; there are no cycles.

```text
            UIShell (M5)
                 |
         AppHost / Session
        /        |        \
Reporting   ActionCommit   Persistence
                 |
              SimCore
   (tick engine, scheduler, resolvers)
                 |
              Domain
    (pure state + value types)
                 |
            StaticData
       (content catalogs)
                 |
              Platform
   (Rng, FixedMath, Tick, Clock)
```

## 1. Platform

One-line: deterministic primitives; the bedrock.

- Vendored PRNG, for example PCG or xorshift64* (~20 lines), pinned and versioned.
- Fixed-point numeric type, for example Q32.32, for all resolution math.
- `Tick` value type (`ulong` wrapper).
- `DeterministicClock` abstraction — the only thing allowed to read wall-clock, and only at session boundaries.
- Structured ID types plus canonical JSON serialization helpers.
- **Does not do:** any game logic, any I/O, any `DateTime.UtcNow` read, or any `System.Random` reference.

## 2. StaticData

One-line: immutable content catalogs.

- Load JSON/YAML for strata, missions, recipes, modules, frames, doctrine templates, and quest steps.
- Validate schema and referential integrity at load time, carrying forward the existing `StaticDataIntegrityTest` pattern.
- Expose read-only catalogs keyed by string ID.
- Compute a `contentHash` stamped into saves.
- **Does not do:** mutate at runtime or know about player state.
- **Carries forward:** pattern from existing `DimenshipBase/StaticData.cs` (`StaticDataSubSystem`).

## 3. Domain

One-line: pure plain-data state model.

- Immutable record types and the aggregate `WorldState`: facilities, storages, energy ledger, robot roster, groups, active processes, active missions, quest state, event-log cursor, and current tick.
- Strongly typed IDs for every entity kind.
- **Does not do:** mutate itself, read content, advance time, or emit events.

## 4. SimCore

One-line: the deterministic tick engine.

- Tick stepper with dynamic chunk sizing (~5s active, ~30s+ offline replay).
- **Scheduler:** priority queue over active processes; compute-slot dispatcher (~8 base); `ComputeDeferred` marking; blocked-reason capture (`MissingInput`, `FacilityBusy`, `PowerCap`).
- **Process resolver:** production / refit / logistics with facility and transport duration-locks, replacing the existing `(ProcessId, FacilityId)` lock model.
- **Mission resolver:** formulaic yields/durations; consumes only the action's seeded RNG; splits into named sub-streams (`hazardStream`, `findsStream`) via SplitMix-style derivation so adding a new consumer cannot shift existing draws.
- **Doctrine evaluator:** invoked during mission resolution and emits `DoctrineRuleFired(ruleId, reason, tick)` records.
- Append-only `EventBatch` emission per step.
- Single-threaded, predefined evaluation order.
- **Does not do:** persist, commit actions (consumes already-committed ones), read files, read the clock, or touch the UI.
- **Replaces:** existing `DimenshipBase/SubSystems/ProcessSubSystem.cs`.
- **Carries forward:** `ProcessBase` / `StepBase` workflow shape from `DimenshipBase/Process.cs`.

## 5. ActionCommit

One-line: boundary where player intent becomes deterministic input.

- Validate an intended action against current `WorldState` (group idle, loadout locked, energy available, stratum unlocked, etc.).
- At commit, stamp `(tick, actionId, seed=Hash(masterSeed, actionId), contentHash)`.
- Snapshot the chosen doctrine by `(id, version)` rather than by runtime pointer, for replay fidelity.
- Append to an append-only `ActionLog`.
- **Does not do:** execute the action, mutate `WorldState` (SimCore does that on the next step), or decide outcomes.

## 6. Persistence

One-line: save portability and replay foundation.

- Save bundle: `{ snapshot(WorldState), ActionLog, contentHash, masterSeed, lastSimulatedTick, rngAlgoVersion }`.
- Version-stamp for future migration; atomic file write suitable for Android.
- Snapshot compaction policy: newest snapshot plus `ActionLog` since that tick.
- **Does not do:** game logic or content semantics.

## 7. Reporting

One-line: turns the EventLog into player-facing reports.

- Ingest typed `EventRecord`s and roll them into `MissionReport` aggregates: summary metrics, timeline, rules-fired-with-reasons, anomalies, and damage breakdown.
- Dashboard alert projection, filtered queries (per-mission, per-quest, per-group), and chart-ready time series later.
- **Does not do:** modify state, invent events, or read content except for localization labels later.

## 8. AppHost / Session

One-line: orchestration glue.

- On launch, load save and compute `deltaTicks = (wallNow - savedWallNow)` at the single permitted clock boundary, then drive SimCore forward in chunks with progress feedback.
- Accept UI action intents, route to `ActionCommit`, and schedule re-simulation.
- Expose session commands (deploy mission, edit doctrine, reprioritize process) and observable state plus report queries.
- **Owns the one allowed wall-clock read.** Anywhere else reading `DateTime.UtcNow` is a bug.
- **Replaces:** existing `DimenshipConsole/SystemRunner.cs` by stripping wall-clock from the loop and making it tick-driven.

## 9. UIShell

One-line: player surface.

- M1-M4: console REPL, evolving the existing `DimenshipConsole` harness minus `SystemRunner`'s wall-clock.
- M5: mobile UI with Dashboard / Processes / Strata / Robotics / Logs tabs per GDD §9.
- Binds to `IGameSession` only and holds no authoritative state.

## B. Determinism contract

- **RNG:** platform-owned, pinned algorithm, versioned. No global RNG. Every `CommittedAction` has a `seed`.
- SimCore splits per-action streams by named sub-stream keys so adding a new consumer cannot perturb existing ones.
- **Time:** SimCore is tick-pure. AppHost at session boundaries converts wall-clock delta into integer tick count and hands SimCore the target tick.
- **Persisted:** snapshot (`WorldState`), `ActionLog`, `contentHash`, `masterSeed`, `lastSimulatedTick`, `rngAlgoVersion`.
- **Re-derived on load:** `EventLog` / reports (rebuilt by replay), scheduler queue ordering, process estimates.
- **Integer/fixed-point mandatory on:** any value that feeds an RNG comparison, accumulates across ticks (energy, storage, HP, durations), or determines branching. Floats are allowed only in UI charts and cosmetic interpolation.

## C. Worked flow — player deploys a mining mission

1. UI collects stratum + mission + group + doctrine + energy allocation, then submits `IntendedAction.DeployMission` via `IGameSession`.
2. AppHost -> ActionCommit validates against `WorldState`.
3. ActionCommit stamps `(tick, actionId, seed, contentHash)`, snapshots doctrine by `(id, version)`, appends to `ActionLog`, and persists incrementally.
4. AppHost calls `SimCore.Step` up to the action's tick.
5. Scheduler assigns the new Mission process a compute slot; resolver begins and splits action RNG into `hazardStream` + `findsStream`.
6. During resolution, doctrine evaluator checks WHEN-THEN rules against the events so far; fired rules emit `DoctrineRuleFired` records.
7. Hazards and finds roll against their streams and emit `HazardEvent`, `FindEvent`, and `DamageEvent`. Yields and duration are formulaic.
8. On completion, SimCore emits `MissionCompleted(summary)` and updates `WorldState` (resources -> storage, robot HP, quest evidence).
9. Reporting consumes the `EventBatch` and assembles `MissionReport`.
10. UI shows an alert; the player drills into the report via `IReportQuery`.

## D. M1-M5 chunk completeness

| Chunk | M1 | M2 | M3 | M4 | M5 |
|---|---|---|---|---|---|
| Platform | full (Rng + Fix + Tick present, unused consumers) | full | full | full | full |
| StaticData | 1 stratum, mining + scav, 1 chain | + hazard tables | + doctrine templates | + 2nd stratum, investigation, quests | full |
| Domain | core state | + hazard/event fields | + doctrine state | + quest/evidence | full |
| SimCore | tick step, formulaic resolvers, scheduler + compute slots + blocked reasons | + RNG plumbed, hazards | + doctrine evaluator | + investigation mission | full |
| ActionCommit | `seed` field present but not yet consumed | seeded-for-real | seeded | seeded | seeded |
| Persistence | snapshot + ActionLog | + rngAlgoVersion stamp | — | — | + Android file-path conventions |
| Reporting | summary + flat event list | + tagged timeline | + rules-fired panel | + quest evidence feed | + charts |
| AppHost / Session | console bootstrap, tick driver | same | same | same | Android lifecycle hooks |
| UIShell | console REPL | console | console | console | Android UI |

## E. Open decisions

These are flagged because they change chunk shape or contents. Recommendations are included.

1. **UI framework for M5:** propose **Avalonia** (C# native, shared Domain types, one language, strong on .NET 10). Alternatives: **MAUI** (first-party Android, more idiomatic), **Godot with C# bindings** (best mobile UX ergonomics, adds engine), **Unity** (heaviest, overkill for a UI-centric sim). Tradeoff: Avalonia's Android mobile story is less mature than MAUI's.
2. **Doctrine granularity:** propose **per-group with per-robot overrides**. Alternatives: per-group only (simpler, loses fine-grained control) or per-robot only (verbose on mobile). Tradeoff: slightly coarser reactions than full per-robot.
3. **EventLog shape:** propose **typed append-only records with a `kind` discriminator**. Alternative: full event-sourcing (replay authority already lives in `ActionLog`; doubling it adds complexity with no benefit).
4. **Quest structure:** propose **node graph with state-per-node**. Alternative: linear chain (cheaper UI, worse fit for an investigation).
5. **Compute-slot mechanic:** keep as agreed (~8 base +1 from companion, visible as a minor resource bar, not a progression gate). Confirm it is not going to be quietly dropped.
6. **Snapshot cadence:** propose **on session close + every 24 game-hours**. `ActionLog` carries everything else. Alternative: every commit (simpler, fatter saves).

## F. Explicitly not in the M1 skeleton

Keeping M1 small so the determinism / tick / scheduler / persistence loop is the only thing being proven:

- Doctrine evaluator (M3).
- Any RNG consumers (M1 has the RNG plumbing but no one drawing from it).
- Companion system, quest graph, investigation evidence.
- Multi-stratum modifiers / stability energy tax.
- Charts, bottleneck detection, anomaly analytics.
- Android lifecycle, background services, notifications.
- Any UI beyond console REPL.
- Save migration framework (stamp a version, don't build migration code yet).
- Pub/sub messaging — the existing `SimpleBroker` stays scrapped unless a real need appears.

## G. Reference files

Carry forward the **shape** of:

- `D:\DEV_Research\dimenship\DimenshipBase\GameTime.cs` -> Platform `Tick`.
- `D:\DEV_Research\dimenship\DimenshipBase\StaticData.cs` -> StaticData `IContentCatalog`.
- `D:\DEV_Research\dimenship\DimenshipBase\Process.cs` -> SimCore scheduler workflow.
- `D:\DEV_Research\dimenship\DimenshipBase\ISystemStateSet.cs` -> Domain / AppHost subsystem lookup (only if a registry is still useful; may not be needed in the cleaner chunk split).

Replace outright:

- `D:\DEV_Research\dimenship\DimenshipBase\SubSystems\ProcessSubSystem.cs` -> SimCore scheduler with priority, compute slots, deferred state, blocked-reason capture.
- `D:\DEV_Research\dimenship\DimenshipConsole\SystemRunner.cs` -> AppHost tick driver (no wall-clock in loop).
- `D:\DEV_Research\dimenship\DimenshipBase\Broker\SimpleBroker.cs` -> delete; not needed.
- `D:\DEV_Research\dimenship\DimenshipBase\Facilities.cs` -> lock model -> SimCore facility-lock with duration tracking.

## H. Verification

How the architecture will be judged correct before declaring M1 done, in a headless test project:

1. **Replay test:** start from empty `WorldState`, apply a scripted `ActionLog` (10 production orders, 2 mission deploys), simulate forward, snapshot the resulting `WorldState`, reload from the same snapshot + `ActionLog`, and assert bit-exact equality of the resulting state.
2. **Cross-tick-size equivalence:** run the same scripted workload stepping at 5s chunks vs 30s chunks; assert identical terminal `WorldState` and identical `EventLog` after reordering-by-tick.
3. **Reprioritization doesn't cross-contaminate:** deploy two missions concurrently; run once with priority A then swap to priority B; assert each mission's own resolution outcome is unchanged, proving per-action RNG streams do not bleed through global state.
4. **Scheduler blocked-reason capture:** queue a process missing an ingredient; assert it appears with `Blocked(MissingInput)` and does not consume a compute slot's real work.
5. **No `DateTime.UtcNow` in SimCore / Domain / StaticData / Platform:** static check or lint; any such reference outside AppHost is a build failure.
6. **No `System.Random` or `new Random(...)` anywhere:** same static check; any construction outside the vendored RNG is a build failure.
