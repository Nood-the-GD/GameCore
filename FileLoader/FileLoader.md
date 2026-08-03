# FileLoader

`FileLoader.cs` — chainable, multi-source async file loader (`Core.FileLoader` namespace). Tries a chain of sources in order, each with its own retry/timeout policy, until one succeeds.

## API
- `new FileLoader()` then chain any of:
  - `.AddFileLoader(filePath)` — read from a local file path.
  - `.AddRemoteLoader(link, localPath)` — download from `link`, saving to `localPath`.
  - `.AddResourcesLoader(filePath)` — load a `TextAsset` from `Resources`.
  - `.WithSuccess(Action<byte[]> onSuccess)` / `.WithError(Action<string> onError)` — result callbacks.
- `await Start()` — runs the configured loaders in the order they were added.

```csharp
await new FileLoader()
    .AddFileLoader(localPath)
    .AddRemoteLoader(url, localPath)
    .WithSuccess(bytes => { /* use bytes */ })
    .WithError(msg => Debug.LogError(msg))
    .Start();
```

## Behavior / guarantees
- Loaders run **in the order added**, one at a time — not in parallel.
- Each loader is retried up to its own `IFileLoader.RetryCount`, waiting 0.5s between attempts, with each attempt capped at `IFileLoader.TimeOut` seconds.
- On the first successful `LoadAsync()`, `onSuccess` fires with the bytes and `Start()` returns immediately — later loaders in the chain are never tried.
- If a loader exhausts its retries, `FileLoader` moves on to the next loader in the chain; only if the **last** loader in the chain fails does `onError` fire.
- Calling `Start()` with no loaders added invokes `onError` immediately.

## Built-in loaders (`Core.FileLoader`, internal `IFileLoader`)
| Loader | RetryCount | TimeOut | Source |
|---|---|---|---|
| `LocalFileLoader` | 1 | 3s | `System.IO.File.ReadAllBytes` |
| `RemoteFileLoader` | 3 | 10s | `UnityWebRequest` download to `localPath` |
| `ResourcesLoader` | 1 | 10s | `Resources.LoadAsync<TextAsset>` |

## Gotcha
`RemoteFileLoader.LoadAsync()` returns `null` (instead of throwing) when the `UnityWebRequest` fails. Since `FileLoader`'s retry/fallback logic only reacts to a thrown exception, a failed download is treated as a **success with `null` data** — `onSuccess` still fires, `onError` does not, and no retry happens.
