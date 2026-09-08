# Utility

Small static helpers in `Core/Utility/`.

## Grep

`Grep.cs` (top-level namespace) — static `GrepExtension.Grep` helper for line-by-line regex search over files on disk.

### API
- `GrepExtension.Grep(rootDir, pattern, searchPattern = "*.*")` — recursively enumerates files under `rootDir` matching `searchPattern`, and yields `(file, line, text)` for every line matching the regex `pattern`.

### Behavior / guarantees
- Lazy (`yield return`) — nothing is read from disk until the returned sequence is enumerated.
- `line` is 1-based; `text` is the matched line, trimmed.
- Plain C#/IO, not Editor-only — but its only current caller is [Reserialize](../Reserialize/Reserialize.md)'s `ReserializeEditor`.

## Extension

`Extension.cs` — `namespace Core.Extension`. Two static classes of collection extension methods for picking a random element.

### API
- `List<T>.GetRandom()` — returns `source[Random.Range(0, source.Count)]` (`UnityEngine.Random`).
- `T[].GetRadom()` — array equivalent, `source[Random.Range(0, source.Length)]`. Note the method name is missing an `n` (`GetRadom`, not `GetRandom`) — keep the typo in mind at call sites or rename it.

### Behavior / guarantees
- Uniform pick, `UnityEngine.Random` (frame-seeded, not deterministic — not for gameplay that must replay identically).
- **No empty-collection guard.** `Random.Range(0, 0)` returns `0`, so an empty list/array throws `ArgumentOutOfRangeException` on the index.
- `GetRandom` (list) is used by `SoundManager.PlaySoundAtPosition(SoundListEnum, ...)` — see [Sound](../Sound/Sound.md). `GetRadom` (array) currently has no callers.
