# MessageBus

`MessageBus.cs` — global pub/sub for struct-based events. Decouples systems so they don't need direct references to each other.

## API
- `MessageBus.Subscribe<T>(Action<T> handler)` — register a handler for event type `T` (`T` must be a `struct`).
- `MessageBus.Unsubscribe<T>(Action<T> handler)` — remove a previously registered handler.
- `MessageBus.Publish<T>(T message)` — broadcast an event to all subscribers of `T`.
- `MessageBus.Init()` — clears all subscriptions; call this once on game/app startup, or on domain reload / test reset, to reset the bus to a clean state.

## Usage pattern
Follow the project's standard MonoBehaviour lifecycle convention: subscribe in `OnEnable`, unsubscribe in `OnDisable`.

```csharp
public readonly struct OrderCompletedEvent
{
    public readonly int OrderId;
    public OrderCompletedEvent(int orderId) => OrderId = orderId;
}

private void OnEnable()
{
    MessageBus.Subscribe<OrderCompletedEvent>(OnOrderCompleted);
}

private void OnDisable()
{
    MessageBus.Unsubscribe<OrderCompletedEvent>(OnOrderCompleted);
}

private void OnOrderCompleted(OrderCompletedEvent e)
{
    // react to the order
}
```

## Behavior / guarantees
- Event payloads are always `struct` — never `null`.
- `Subscribe` with a `null` handler logs an error and is ignored, no exception.
- `Publish`/`Unsubscribe` for an event type with no subscribers is a normal no-op — not an error.
- Each subscriber is invoked independently inside its own try/catch. A throwing subscriber gets its exception logged via `Debug.LogError` but does not stop delivery to other subscribers.
