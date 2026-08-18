using UnityEngine;

[CreateAssetMenu(fileName = "InventorySO", menuName = "InventorySO", order = 0)]
public class InventorySO : ScriptableObject
{
    public Inventory inventory = new();
}