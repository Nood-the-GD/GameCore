# Reserialize

`Reserialize.cs` (`Core.Reserialize` namespace, Editor-only) — force-reserializes every scene/prefab/asset that references a script tagged `[Reserialize]`, so renamed serialized fields get rewritten to their current name instead of silently keeping stale YAML keys.

## API
- `[Reserialize]` — attribute to tag a class (e.g. a `MonoBehaviour`/`ScriptableObject` whose serialized field was renamed).
- `ReserializeEditor.Reserialize()` — menu item `Tools/Reserialize`; also runs automatically on every domain reload via `[InitializeOnLoadMethod]`.

## Behavior / guarantees
- Editor-only (`#if UNITY_EDITOR`) — no runtime/build cost.
- Scans all loaded assemblies for classes marked `[Reserialize]`, resolves each to its script asset GUID, then uses [Utility](../Utility/Utility.md)'s `GrepExtension.Grep` to find every `.unity`/`.asset` file under `Assets/_Project` referencing that GUID, and force-reserializes the matches (`AssetDatabase.ForceReserializeAssets`).
- `GetScriptGUID` throws if no script asset matches the tagged type's name — a renamed/moved script file (not just a renamed field) will break this at the GUID-lookup step.
- No class in the codebase is currently tagged `[Reserialize]` — the attribute must be applied manually to whichever type just had a field renamed; the DirtyPlate/Pizza dish-data assets were reserialized by hand this session, not via this tool.
