# Utility

`Grep.cs` (top-level namespace) — static `GrepExtension.Grep` helper for line-by-line regex search over files on disk.

## API
- `GrepExtension.Grep(rootDir, pattern, searchPattern = "*.*")` — recursively enumerates files under `rootDir` matching `searchPattern`, and yields `(file, line, text)` for every line matching the regex `pattern`.

## Behavior / guarantees
- Lazy (`yield return`) — nothing is read from disk until the returned sequence is enumerated.
- `line` is 1-based; `text` is the matched line, trimmed.
- Plain C#/IO, not Editor-only — but its only current caller is [Reserialize](../Reserialize/Reserialize.md)'s `ReserializeEditor`.
