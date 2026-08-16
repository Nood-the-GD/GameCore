# GDKSymbol

`GDKSymbol.cs` (top-level namespace) — a `ScriptableObject` holding a `SerializedDictionary<string, bool>` of namespace → active. `GDKSymbolEditor.cs` (Editor-only) generates, per entry, a namespace-scoped `Debug.Log` wrapper: active namespaces log normally, inactive ones compile to a no-op — so a call site never has to `#if`/comment out logging, just flip the toggle and hit Apply.

## API
- `GDKSymbol.Entries` (`SerializedDictionary<string, bool>`, from `AYellowpaper.SerializedCollections`) — namespace → is-active, edited directly in the Inspector as a real key/value table.
- "Apply" button (custom inspector) — regenerates `Core/GDKSymbol/Generated/{NameSpace}.GDKDebug.cs` for every entry, and deletes any previously generated file whose namespace is no longer in the list.

## How to use
1. Create (or open) a `GDKSymbol` asset via `Assets > Create > RobotCafe > GDKSymbol`.
2. Add an entry: `NameSpace` = the C# namespace other scripts already live in (e.g. `RobotCafe.Debugging`), `IsActive` = whether its logs should show.
3. Click **Apply**. Other code then calls the fully-qualified `NameSpace.Debug.Log(...)` (not a bare `Debug.Log` via `using`, which would collide with `UnityEngine.Debug`).
4. Flip `IsActive` and Apply again to silence/restore that namespace's logs without touching call sites.

## Behavior / guarantees
- Editor-only generation (`#if UNITY_EDITOR`); the generated `Debug` classes themselves are plain runtime C# with no `#if` — inactive ones are simply empty method bodies.
- Apply is a full resync: every entry's file is rewritten, and generated files for namespaces no longer in `Entries` are deleted (`.cs` + `.meta`) so stale wrappers can't linger.
- Generated files are named `{NameSpace with '.' -> '_'}.GDKDebug.cs` under `Generated/` and are marked auto-generated — don't hand-edit them, they're overwritten on the next Apply.
- Wraps `Debug.Log`, `Debug.LogWarning`, `Debug.LogError` (each `(object)` only) — no format/context overloads.
- Active wrappers prefix the message with the namespace, e.g. `Module_Robot.Debug.Log("x")` prints `[Module_Robot] x`.
