# ServiceManager

`ServiceManager.cs` — global service locator for controller/presenter **instances**. Lets one Presenter find another (or any cross-cutting service) by interface, without a hard scene reference (`GetComponent`, public field drag-in, etc.).

## API
- `ServiceManager.Register(instance).As<T>()` — register `instance` under service type `T`. Chainable: `.As<T1>().As<T2>()` registers the same instance under multiple types in one call.
- `ServiceManager.Register(instance).AsImplementedInterfaces()` — register `instance` under every interface its concrete type implements.
- `ServiceManager.Unregister(instance).As<T>()` — remove `instance`'s registration for type `T`. Chainable the same way as `Register`.
- `ServiceManager.Unregister(instance).AsAll()` — remove every registration currently pointing at `instance`.
- `ServiceManager.Get<T>()` — fetch the registered instance for `T`, or `null` (with a logged error) if nothing is registered.
- `ServiceManager.TryGet<T>(out T service)` — same lookup without logging, for call sites where a missing service is expected/optional.
- `ServiceManager.Init()` — clears the registry; call on app startup or test/domain reload reset.

## Usage pattern
Register by the controller's **interface**, not its concrete type, so callers depend on an abstraction. Follow the project's standard MonoBehaviour lifecycle convention: register in `OnEnable`, unregister in `OnDisable`.

```csharp
public interface ICameraController
{
    void Focus(Vector3 worldPosition);
}

public interface IZoomable
{
    void SetZoom(float size);
}

private void OnEnable()
{
    ServiceManager.Register(this).As<ICameraController>().As<IZoomable>();
}

private void OnDisable()
{
    ServiceManager.Unregister(this).As<ICameraController>().As<IZoomable>();
}
```

Other code retrieves it anywhere with `ServiceManager.Get<ICameraController>()` or `ServiceManager.TryGet<ICameraController>(out var camera)`.

## Behavior / guarantees
- `Register(null)` logs an error; the returned chain is a safe no-op (`.As<T>()`/`.AsImplementedInterfaces()` do nothing further), no exception.
- `.As<T>()` when the instance doesn't actually implement `T` logs an error and skips just that registration, without breaking the rest of the chain.
- Re-registering an already-registered type logs a warning and overwrites (last one wins) — never throws.
- `Unregister(instance).As<T>()` only removes the entry if it still points at that exact instance, so it can't evict a different instance that re-registered under that type later.
- `Get<T>()` for an unregistered type logs an error and returns `null` rather than throwing; use `TryGet<T>` when a missing service shouldn't log.
- `Init()` clears the registry, mirroring `MessageBus.Init()`.
