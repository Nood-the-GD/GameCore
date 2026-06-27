using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class Inventory
{
    private Dictionary<IInventoryItem, int> _inventory = new(new InventoryItemComparer());

    public Dictionary<IInventoryItem, int> ItemDict => _inventory;

    public void Add(IInventoryItem data, int amount = 1)
    {
        _inventory.TryGetValue(data, out int current);
        _inventory[data] = current + amount;
    }

    public bool TryRemove(IInventoryItem data, int amount)
    {
        if (!_inventory.ContainsKey(data) || _inventory[data] < amount)
        {
            Debug.LogError($"Inventory do not have {data} or amount is not enough");
            return false;
        }

        _inventory[data] -= amount;
        return true;
    }

    /// <summary>
    /// Remove a data with amount or until it reach 0
    /// </summary>
    /// <param name="data">key data to remove</param>
    /// <param name="amount">amount you want to remove</param>
    /// <param name="return">return the amount that inventory do not have enough</param>
    public void RemoveTo0(IInventoryItem data, out int @return, int amount = 1)
    {
        @return = amount;

        if (!_inventory.ContainsKey(data))
        {
            Debug.LogError($"Inventory do not have {data}");
            return;
        }
        else
        {
            if (_inventory[data] > amount)
            {
                _inventory[data] -= amount;
                @return = 0;
            }
            else
            {
                @return = amount - _inventory[data];
                _inventory.Remove(data);
            }
        }
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
}