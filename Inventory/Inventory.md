# Inventory

`Inventory.cs` / `IInventoryItem.cs` — a plain-C# item-count map keyed by `IInventoryItem`, identity compared by `Id` rather than reference.

## API
- `IInventoryItem` — implement this on any item type; only requirement is a stable `Id`.
- `Inventory.Add(IInventoryItem data, int amount = 1)` — adds to the item's count (creates the entry if new).
- `Inventory.TryRemove(IInventoryItem data, int amount)` — removes `amount` if present and sufficient; returns `false` (and logs an error) otherwise, without modifying the count.
- `Inventory.RemoveTo0(IInventoryItem data, out int @return, int amount = 1)` — removes as much of `amount` as available, down to 0, then deletes the entry; `@return` is the shortfall (0 if fully removed).
- `Inventory.ItemDict` — read-only-by-convention access to the backing `Dictionary<IInventoryItem, int>`.
- `Inventory.ToJson()` — serializes the inventory via `Newtonsoft.Json`.

## Behavior / guarantees
- Two `IInventoryItem`s with the same `Id` are treated as the same key (`InventoryItemComparer`), even if they're different object instances — lookups/adds are by `Id`, not reference.
- `TryRemove` is all-or-nothing: if the requested `amount` exceeds the current count, nothing is removed and it returns `false`.
- `RemoveTo0` is partial-safe: it never removes more than what's available and never leaves a negative count; when the requested `amount` is more than available, the item is removed entirely and `@return` reports how much was short.
- `TryRemove`/`RemoveTo0` on an item not in the inventory both log an error (`TryRemove` also returns `false`; `RemoveTo0` returns `@return = amount` unchanged).
