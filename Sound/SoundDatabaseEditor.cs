#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Core.SoundManager
{
    [CustomEditor(typeof(SoundDatabase))]
    public class SoundDatabaseEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Generate Sound & Music Enum"))
            {
                ((SoundDatabase)target).GenerateMusicEnum();
                ((SoundDatabase)target).GenerateSoundEnum();
                ((SoundDatabase)target).GenerateSoundListDic();
            }
        }
    }
}
#endif
