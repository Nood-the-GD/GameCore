# Core

Core-level systems that live for the entire app lifetime and are usable from anywhere in the codebase. No MonoBehaviour, no scene object, no manual setup — just call the static API.

## Features
- [MessageBus](MessageBus.md) — global pub/sub for struct-based events.
- [ServiceManager](ServiceManager.md) — global service locator for controller/presenter instances.
- [FileLoader](FileLoader/FileLoader.md) — chainable multi-source async file loader (local / remote / Resources) with per-source retry.
- [FileUtil](FileUtil/FileUtil.md) — writeable-path file I/O and JSON save/load helpers.
- [Inventory](Inventory/Inventory.md) — item-count map keyed by `IInventoryItem.Id`.
- [Reserialize](Reserialize/Reserialize.md) — Editor tool to force-reserialize assets referencing `[Reserialize]`-tagged scripts after a field rename.
- [Utility](Utility/Utility.md) — static file-search helpers (currently just `GrepExtension.Grep`).
