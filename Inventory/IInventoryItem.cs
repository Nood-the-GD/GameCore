using System.Collections.Generic;

public interface IInventoryItem
{
    public string Id { get; }
}

public class InventoryItemComparer : IEqualityComparer<IInventoryItem>
{
    public bool Equals(IInventoryItem x, IInventoryItem y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x == null || y == null) return false;
        return x.Id == y.Id;
    }

    public int GetHashCode(IInventoryItem obj)
    {
        return obj.Id?.GetHashCode() ?? 0;
    }
}
