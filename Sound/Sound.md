# Sound

`SoundManager.cs` — plain-C# audio service (music + positional SFX), backed by a single
`SoundDatabase` ScriptableObject that is loaded lazily via Addressables.

Registered as a global service in `MainServiceLoader`:

```csharp
ServiceManager.Register<SoundManager>();
// anywhere:
ServiceManager.Get<SoundManager>().PlaySoundAtPosition(SoundListEnum.Robot_OutOfEnergy, transform.position);
```

## Files
- `SoundManager.cs` — the service. `namespace Core.SoundManager`.
- `SoundDatabase.cs` — `ScriptableObject` holding the clip maps + Editor enum generators. `[CreateAssetMenu(menuName = "SoundDatabase")]`.
- `SoundDatabaseEditor.cs` — custom inspector: one "Generate Sound & Music Enum" button.
- `SoundEnum.cs` / `MusicEnum.cs` / `SoundListEnum.cs` — **generated** enums, one member per database key. Do not hand-edit; regenerate from the database.
- `GDKSymbol/Generated/Module_Sound.GDKDebug.cs` — the `Debug` wrapper `SoundManager` logs through (`Module_Sound` symbol; no-op when the symbol is off).

## SoundDatabase

Three `SerializedDictionary` maps (from `AYellowpaper.SerializedCollections`), all keyed by `string`:

| Field | Type | Purpose | Enum generated |
|---|---|---|---|
| `musicCipDic` | `string → AudioClip` | looping background tracks | `MusicEnum` |
| `soundClipDic` | `string → AudioClip` | one-shot SFX (fixed clip) | `SoundEnum` |
| `soundListDic` | `string → List<AudioClip>` | one-shot SFX with a random variant per play | `SoundListEnum` |

Lookups at runtime use `enumValue.ToString()` as the dictionary key, so the enum member
name **must** match the database key exactly — that is what the generators guarantee.

The asset is loaded by key `"SoundDataBase"`:
`SmartAddressable.LoadAsync<SoundDatabase>("SoundDataBase")`. It must exist as an
Addressable under that address.

### Enum generation

`SoundDatabaseEditor`'s button (or the individual `[ContextMenu]` items on the asset)
calls `GenerateMusicEnum` / `GenerateSoundEnum` / `GenerateSoundListDic`. Each one:
- deletes the existing `MusicEnum.cs` / `SoundEnum.cs` / `SoundListEnum.cs` next to `SoundDatabase.cs`,
- rewrites it with one member per current dictionary key,
- calls `AssetDatabase.Refresh()`.

Workflow: add clips + keys to the database asset → click the button → the enum recompiles →
reference the new members in code.

## SoundManager API

- `PlaySoundAtPosition(SoundEnum, Vector3 position)` — plays the single clip mapped in `soundClipDic` at a world position via `AudioSource.PlayClipAtPoint`.
- `PlaySoundAtPosition(SoundListEnum, Vector3 position)` — picks a random clip from that key's `List<AudioClip>` (`List.GetRandom()`, see [Utility](../Utility/Utility.md)) and plays it at a world position.
- `PlayMusic(MusicEnum)` — sets the clip on the main music source, `volume = 1`, `loop = true`, `Play()`.
- `ChangeMusic(MusicEnum, float duration = 0.5f)` — DOTween crossfade: fades the backup source up and the main source down over `duration`, then swaps the new clip onto the main source and stops the backup.

## Behavior / guarantees

- **Async DB load, not awaited.** The constructor fires `LoadDataBaseIfNeed().Forget()`. Any call made before the Addressable resolves will `NullReferenceException` on `_dataBase`. In practice the DB is small and loads during the loading pipeline, but there is no readiness gate — treat early calls as unsafe.
- **Missing key** on `soundClipDic` / `musicCipDic` logs an error through `Module_Sound.Debug.LogError` and returns `null` (Unity then warns about the null clip). `soundListDic` misses are **not** guarded — a missing key throws `KeyNotFoundException`.
- **Music sources are never assigned.** `_mainMusicSource` / `_backupMusicSource` are private fields with no setter, no `AudioSource` creation, and no injection point. `PlayMusic` / `ChangeMusic` will `NullReferenceException` as written — music playback is effectively **not wired up yet**; only the positional-SFX paths are usable today.
- `PlayClipAtPoint` spawns a temporary `AudioSource` GameObject that Unity destroys when the clip finishes — no pooling, fire-and-forget, not stoppable.
- Not a `MonoBehaviour`: no scene presence, survives scene loads as long as the service registration does.

## Current data (generated enums)

- `MusicEnum`: `BGM`, `Attack`
- `SoundEnum`: *(empty)*
- `SoundListEnum`: `Robot_Choose`, `Robot_Assign_Success`, `Robot_Assign_Error`, `Robot_OutOfEnergy`, `Robot_Rest`
