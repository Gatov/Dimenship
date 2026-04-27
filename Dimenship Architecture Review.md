# Dimenship Architecture Review

## Overall assessment

The architecture is strong and internally coherent for a deterministic, offline-first strategy sim. The nine-chunk split enforces clear responsibility boundaries and correctly centers determinism on three pillars: pinned RNG, tick-pure simulation, and replayable action logs.

## What is working well

1. **Clear deterministic boundary design**
   - `ActionCommit` is correctly treated as the point where intent becomes immutable deterministic input.
   - `SimCore` explicitly consumes committed inputs only, which avoids hidden state mutations.

2. **Good layering and dependency direction**
   - The DAG is understandable and practical (`Platform -> StaticData -> Domain -> SimCore` etc.).
   - The split between simulation (`SimCore`) and projection (`Reporting`) is particularly solid.

3. **Replay-first persistence model**
   - Save shape (`snapshot + action log + seed/content/rng version`) is appropriate for portability and long-term compatibility.
   - Rebuilding reports from replay avoids duplicative authority models.

4. **Pragmatic milestone slicing**
   - M1 scope discipline is good. Keeping doctrine and RNG consumers out of M1 reduces variance while validating core invariants first.

## Key risks and gaps to close before implementation

1. **Deterministic serialization format is underspecified**
   - The plan mentions canonical JSON helpers, but deterministic replay requires stricter rules:
     - property ordering,
     - number formatting rules,
     - culture-invariant parsing,
     - explicit null/default handling,
     - stable map iteration order.
   - Recommendation: define a short, testable serialization spec now (even if implementation is deferred).

2. **Action ordering semantics need a tie-break contract**
   - You define predefined evaluation order and priority scheduling, but not the complete tie-break set.
   - Recommendation: codify one total ordering key (e.g. `tick, priority, actionId, processId`) and freeze it in tests.

3. **RNG stream names need governance**
   - Named sub-stream derivation is excellent, but stream name drift can silently alter results.
   - Recommendation: maintain an enum/registry of stream keys with versioning rules and a "no rename without migration" policy.

4. **Fixed-point policy needs explicit boundaries**
   - Q32.32 is named, but conversion boundaries (UI inputs, static data literals, persisted values) are not.
   - Recommendation: document where float/decimal -> fixed conversions are legal and forbid implicit casts in simulation code.

5. **Snapshot cadence needs failure-mode validation**
   - "Session close + every 24 game-hours" may be too sparse for large logs on long-running profiles.
   - Recommendation: add a dual-threshold policy (`tick interval OR action-count threshold`) to cap replay cost.

6. **Static checks should be implementation-enforced early**
   - Architecture calls out bans on `DateTime.UtcNow` and `System.Random`, which is correct.
   - Recommendation: add these as CI analyzers in M1, not as post-hoc conventions.

## Suggested acceptance criteria additions (headless)

Add these to section H before declaring M1 complete:

1. **Serialization determinism test**
   - Same in-memory `WorldState` serialized on two machines/locales must produce byte-identical output.

2. **Scheduler total-order invariance test**
   - For same-tick, same-priority enqueues in different insertion orders, resulting execution order must be identical.

3. **RNG stream stability test**
   - Introducing a new stream consumer in one subsystem must not change historical outcomes of existing streams.

4. **Long replay budget test**
   - Replaying N (e.g. 100k) actions remains under target time/memory budget in headless mode.

## Open decisions feedback

1. **M5 UI choice**
   - Given Android as the only required client in current scope, MAUI is lower risk than Avalonia today.
   - If desktop parity is a near-term requirement, Avalonia remains attractive; otherwise MAUI minimizes mobile integration risk.

2. **Doctrine granularity**
   - Proposed per-group + per-robot override is a good default and balances UX complexity.

3. **Event model**
   - Typed append-only records with kind discriminator is correct for this architecture and avoids unnecessary event-sourcing duplication.

4. **Quest graph**
   - Node graph is the right long-term fit for investigation gameplay, but ensure tooling supports authoring/debugging from day one.

## Prioritized next steps

1. Write a one-page **Determinism Spec** (serialization, ordering, RNG stream governance, fixed-point boundaries).
2. Turn H.5 and H.6 bans into CI-enforced analyzers immediately.
3. Define scheduler tie-break contract and encode it in tests.
4. Add log compaction thresholds beyond session-close cadence.
5. Confirm M5 UI framework based on deployment risk tolerance (Android-first suggests MAUI).

## Final verdict

This architecture is ready to move into M1 implementation after tightening deterministic-spec details (serialization + ordering + stream governance). The macro decomposition is strong, milestone scoping is sensible, and the replay-first model is appropriate for the game's core design goals.
