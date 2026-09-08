# FileUtil

`FileUtility.cs` / `JsonSaveLoad.cs` (`Core.FileUtil` / `Core.Json` namespaces) — static helpers for reading/writing files under a platform-correct writeable path, plus JSON convenience wrappers on top.

## FileUtility API

### Path resolution
- `FileUtility.GetWriteablePath(relativePath)` — resolves `relativePath` to a writeable location:
  - In the Editor: `<project root>/WriteablePath/<relativePath>`.
  - In a build: `Application.persistentDataPath/<relativePath>`.
- `FileUtility.GetDataParentPath(relativePath)` — resolves `relativePath` against the folder that contains `Assets/` (i.e. the project root), regardless of Editor/build.
- `FileUtility.GetFilePath(fileName, FileType type)` — **Editor-only.** Looks the asset up via `AssetDatabase.FindAssetGUIDs("t:<type> <fileName>")` and returns the `Assets/`-relative path of the **first** match. `FileType` is `Script` / `Prefab` / `ScriptableObject`. Returns `""` in a build. Throws (`IndexOutOfRangeException`) if nothing matches. Used by `SoundDatabase`'s enum generators to locate `SoundDatabase.cs`.

### Writeable-path I/O
- `FileUtility.WriteToPath(relativePath, content)` — writes `content` to the writeable path (`GetWriteablePath`), creating missing directories first.
- `FileUtility.ReadTextFromPath(relativePath)` — returns the file's text from the writeable path, or `string.Empty` if it doesn't exist.
- `FileUtility.ReadBytesFromPath(relativePath)` — returns the file's bytes from the writeable path. **No existence check** — throws if the file is missing (unlike `ReadTextFromPath`).

### Project-tree writes (Editor codegen)
- `FileUtility.WriteToRelativePath(relativePath, content)` — for writing generated source next to existing assets; `relativePath` starts with `Assets/`. Creates the target directory, then writes. Mainly used by the `SoundDatabase` enum generators.

## JsonSaveLoad API
- `JsonSaveLoad.SaveToJson(obj, savePath)` / `LoadFromJson<T>(savePath)` — serialize/deserialize `obj` via `Newtonsoft.Json` at an explicit writeable-relative path.
- `JsonSaveLoad.QuickSaveToJson(obj, fileName)` — same as `SaveToJson`, named for the common case of a bare filename.
- `JsonSaveLoad.QuickLoadFromJson<T>(fileName)` — loads `T`, returning `new T()` if the file doesn't exist (requires `T : new()`).
- `JsonSaveLoad.QuickLoadFromJson<T>(fileName, @default)` — same, but returns `@default` instead of `new T()` when the file is missing.

## Behavior / guarantees
- All paths are resolved through `FileUtility.GetWriteablePath` — writes and JSON saves always land in the same place reads look for them.
- `WriteToPath` auto-creates any missing intermediate directories; callers never need to `Directory.CreateDirectory` themselves.
- `ReadBytesFromPath` has no existence check and throws on a missing file; `ReadTextFromPath` never throws, returning `string.Empty` instead — the two are not symmetric.

## Gotchas

`WriteToRelativePath` computes `finalPath` (project-root + `relativePath`) and creates the directory for *that*, but the final `File.WriteAllText` call passes the bare `relativePath`, not `finalPath`. It only lands in the right place because the working directory is the project root at the time it's called (Editor codegen). The `dataPathFolder` / `finalPath` computation is effectively dead — the write relies on the process CWD instead.

`QuickLoadFromJson<T>`'s missing-file fallback (`new T()` / `@default`) never actually triggers. `ReadTextFromPath` returns `string.Empty` (not `null`) when the file is missing, but the guard is `if (json != null)` — an empty string still passes that check, so it falls through to `JsonConvert.DeserializeObject<T>("")`, which returns `default(T)` (`null` for reference types). A missing save file therefore yields `null`, not `new T()`/`@default`, for reference `T`.
