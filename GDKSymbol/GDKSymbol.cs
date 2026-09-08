using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Core.GDKSymbol
{
    [CreateAssetMenu(fileName = "GDKSymbol", menuName = "RobotCafe/GDKSymbol")]
    public class GDKSymbol : ScriptableObject
    {
        [SerializedDictionary("Namespace", "In Build")]
        public SerializedDictionary<string, bool> Entries = new();
    }
}
    