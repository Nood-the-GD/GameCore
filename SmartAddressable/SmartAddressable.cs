using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class SmartAddressable
{
    private static readonly Dictionary<string, AsyncOperationHandle> _keyHandles = new();
    private static readonly Dictionary<string, int> _keyRefCounts = new();

    private static readonly Dictionary<string, AsyncOperationHandle> _labelHandles = new();
    private static readonly Dictionary<string, int> _labelRefCounts = new();

    private static readonly Dictionary<AssetReference, AsyncOperationHandle> _referenceHandles = new();
    private static readonly Dictionary<GameObject, AsyncOperationHandle<GameObject>> _instanceHandles = new();

    public static async UniTask<T> LoadAsync<T>(string key, CancellationToken ct = default)
    {
        if (_keyHandles.TryGetValue(key, out var existing))
        {
            _keyRefCounts[key]++;
            return existing.Convert<T>().Result;
        }

        var handle = Addressables.LoadAssetAsync<T>(key);
        _keyHandles[key] = handle;
        _keyRefCounts[key] = 1;

        return await handle.ToUniTask(cancellationToken: ct);
    }

    public static void Release(string key)
    {
        if (!_keyRefCounts.ContainsKey(key)) return;

        _keyRefCounts[key]--;
        if (_keyRefCounts[key] > 0) return;

        Addressables.Release(_keyHandles[key]);
        _keyHandles.Remove(key);
        _keyRefCounts.Remove(key);
    }

    public static async UniTask<List<T>> LoadAssetsAsync<T>(string label, CancellationToken ct = default)
    {
        if (_labelHandles.TryGetValue(label, out var existing))
        {
            _labelRefCounts[label]++;
            return existing.Convert<List<T>>().Result;
        }

        var handle = Addressables.LoadAssetsAsync<T>(label, null);
        _labelHandles[label] = handle;
        _labelRefCounts[label] = 1;

        return await handle.ToUniTask(cancellationToken: ct) as List<T>;
    }

    public static void ReleaseLabel(string label)
    {
        if (!_labelRefCounts.ContainsKey(label)) return;

        _labelRefCounts[label]--;
        if (_labelRefCounts[label] > 0) return;

        Addressables.Release(_labelHandles[label]);
        _labelHandles.Remove(label);
        _labelRefCounts.Remove(label);
    }

    public static async UniTask<T> LoadAsync<T>(AssetReference reference, CancellationToken ct = default)
    {
        var handle = reference.LoadAssetAsync<T>();
        _referenceHandles[reference] = handle;
        return await handle.ToUniTask(cancellationToken: ct);
    }

    public static void Release(AssetReference reference)
    {
        if (!_referenceHandles.ContainsKey(reference)) return;

        reference.ReleaseAsset();
        _referenceHandles.Remove(reference);
    }

    public static async UniTask<GameObject> InstantiateAsync(string key, Transform parent = null, CancellationToken ct = default)
    {
        var handle = Addressables.InstantiateAsync(key, parent);
        var instance = await handle.ToUniTask(cancellationToken: ct);
        _instanceHandles[instance] = handle;
        return instance;
    }

    public static async UniTask<GameObject> InstantiateAsync(AssetReference reference, Transform parent = null, CancellationToken ct = default)
    {
        var handle = reference.InstantiateAsync(parent);
        var instance = await handle.ToUniTask(cancellationToken: ct);
        _instanceHandles[instance] = handle;
        return instance;
    }

    public static void ReleaseInstance(GameObject instance)
    {
        if (!_instanceHandles.TryGetValue(instance, out var handle)) return;

        Addressables.ReleaseInstance(handle);
        _instanceHandles.Remove(instance);
    }

    // Guards against stale handles surviving into a new run when domain reload
    // is disabled in the Editor, and releases everything cleanly on quit.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void OnRuntimeInit()
    {
        ReleaseAll();
        Application.quitting -= ReleaseAll;
        Application.quitting += ReleaseAll;
    }

    public static void ReleaseAll()
    {
        foreach (var handle in _keyHandles.Values)
            if (handle.IsValid()) Addressables.Release(handle);
        _keyHandles.Clear();
        _keyRefCounts.Clear();

        foreach (var handle in _labelHandles.Values)
            if (handle.IsValid()) Addressables.Release(handle);
        _labelHandles.Clear();
        _labelRefCounts.Clear();

        foreach (var pair in _referenceHandles)
            if (pair.Value.IsValid()) pair.Key.ReleaseAsset();
        _referenceHandles.Clear();

        foreach (var handle in _instanceHandles.Values)
            if (handle.IsValid()) Addressables.ReleaseInstance(handle);
        _instanceHandles.Clear();
    }
}
