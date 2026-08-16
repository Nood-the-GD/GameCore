using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "GDKSymbol", menuName = "RobotCafe/GDKSymbol")]
public class GDKSymbol : ScriptableObject
{
    [SerializedDictionary("Namespace", "Is Active")]
    public SerializedDictionary<string, bool> Entries = new();
}
    