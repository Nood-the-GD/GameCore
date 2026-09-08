# GDKSymbol

`GDKSymbol.cs` (top-level namespace) — a `ScriptableObject` holding a `SerializedDictionary<string, bool>` of namespace → in-build. `GDKSymbolEditor.cs` (Editor-only) generates, per entry, a namespace-scoped `Debug` wrapper whose methods are `[System.Diagnostics.Conditional("<Namespace>")]`, and syncs those namespace names as **scripting define symbols** in Player Settings. If the define is present, the wrapper's calls compile in; if not, the C# compiler strips every call (and its arguments) out of the build — nothing to strip at IL2CPP time, no runtime cost, no string work.

## API
- `GDKSymbol.Entries` (`SerializedDictionary<string, bool>`, from `AYellowpaper.SerializedCollections`) — namespace → include-in-build, edited directly in the Inspector as a key/value table.
- "Apply" button (custom inspector):
  1. regenerates `Core/GDKSymbol/Generated/{NameSpace}.GDKDebug.cs` for every entry, and deletes any previously generated file whose namespace is no longer in the list (`.cs` + `.meta`);
  2. syncs the define symbols on **every** non-obsolete `NamedBuildTarget`: adds the define for each enabled entry, removes the define for each disabled/removed entry, and leaves all non-GDK defines (e.g. `DOTWEEN`) untouched.

## How to use
1. Create (or open) a `GDKSymbol` asset via `Assets > Create > RobotCafe > GDKSymbol`.
2. Add an entry: `Namespace` = a module tag that is also a valid define symbol, e.g. `Module_Robot` (dots are replaced with `_` for both the filename and the define). `In Build` = whether its logs ship.
3. Click **Apply**, let scripts recompile.
4. In any script: `using Debug = GDKSymbol.Module_Robot.Debug;` then call `Debug.Log(...)` / `LogWarning` / `LogError` as usual. (Or the fully-qualified `GDKSymbol.Module_Robot.Debug.Log(...)`.)
5. Flip `In Build` and Apply again — the define is added/removed and the calls are compiled in or stripped out, with no change to call sites.

## Behavior / guarantees
- **Real strip, not a no-op.** Disabled module → its define is absent → `[Conditional]` makes the compiler drop every `Debug.Log(...)` call to that module, including evaluation of the interpolated message. Enabled module → calls forward to `UnityEngine.Debug.*` with a `[Module_X]` prefix.
- The generated file is the **same** regardless of the toggle (always `[Conditional]` + a real body); only the define set changes. So a `using` alias to a disabled module still resolves — the class exists, its calls are just erased.
- Editor-only generation/sync (`#if UNITY_EDITOR`). The generated `Debug` classes are plain runtime C# with no `#if`.
- Apply is a full resync: every entry's file is rewritten; generated files for namespaces no longer in `Entries` are deleted so stale wrappers can't linger. **Removing an entry also removes its define — remove or update the call sites first, or they won't compile.**
- Define sync writes to all non-obsolete build targets so device builds and the Editor stay consistent without re-Applying per platform.
- Wraps `Log`, `LogWarning`, `LogError` (each `(object message)` only) — no format/context overloads.
