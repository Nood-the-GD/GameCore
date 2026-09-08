# Core

Core-level systems that live for the entire app lifetime and are usable from anywhere in the codebase. No MonoBehaviour, no scene object, no manual setup — just call the static API.

## Features
- [MessageBus](MessageBus.md) — global pub/sub for struct-based events.
- [ServiceManager](ServiceManager.md) — global service locator for controller/presenter instances.
- [FileLoader](FileLoader/FileLoader.md) — chainable multi-source async file loader (local / remote / Resources) with per-source retry.
- [FileUtil](FileUtil/FileUtil.md) — writeable-path file I/O and JSON save/load helpers.
- [Inventory](Inventory/Inventory.md) — item-count map keyed by `IInventoryItem.Id`.
- [Reserialize](Reserialize/Reserialize.md) — Editor tool to force-reserialize assets referencing `[Reserialize]`-tagged scripts after a field rename.
- [GDKSymbol](GDKSymbol/GDKSymbol.md) — per-namespace toggleable `Debug.Log` wrapper generator; inactive namespaces compile to a no-op.
- [Sound](Sound/Sound.md) — `SoundManager` service + `SoundDatabase` SO; positional SFX (music path not wired yet), enums generated from the database keys.
- [Utility](Utility/Utility.md) — static helpers: `GrepExtension.Grep` (file search) and `Core.Extension` random-element extension methods.
