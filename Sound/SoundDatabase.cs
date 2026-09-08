using System.Text;
using AYellowpaper.SerializedCollections;
using Core.FileUtil;
using UnityEngine;
using System.IO;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Core.SoundManager
{
    [CreateAssetMenu(fileName = "SoundDatabase", menuName = "SoundDatabase", order = 0)]
    public class SoundDatabase : ScriptableObject
    {
        public SerializedDictionary<string, List<AudioClip>> soundListDic = new();
        public SerializedDictionary<string, AudioClip> soundClipDic = new();
        public SerializedDictionary<string, AudioClip> musicCipDic = new();

        [ContextMenu("GenerateSoundEnum")]
        internal void GenerateSoundEnum()
        {
#if UNITY_EDITOR
            var path = FileUtility.GetFilePath(nameof(SoundDatabase), FileType.Script);
            var finalPath = Path.Combine(Path.GetDirectoryName(path), "SoundEnum.cs");
            if (File.Exists(finalPath))
            {
                File.Delete(finalPath);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("public enum SoundEnum \n");
            sb.Append("{\n");
            foreach (var key in soundClipDic.Keys)
            {
                sb.Append("\t" + key + ",\n");
            }
            sb.Append("}");

            FileUtility.WriteToRelativePath(finalPath, sb.ToString());

            AssetDatabase.Refresh();

            Debug.Log($"Generated SoundEnum in {finalPath}");
#endif
        }

        [ContextMenu("GenerateMusicEnum")]
        internal void GenerateMusicEnum()
        {
#if UNITY_EDITOR
            var path = FileUtility.GetFilePath(nameof(SoundDatabase), FileType.Script);
            var finalPath = Path.Combine(Path.GetDirectoryName(path), "MusicEnum.cs");
            if (File.Exists(finalPath))
            {
                File.Delete(finalPath);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("public enum MusicEnum \n");
            sb.Append("{\n");
            foreach (var key in musicCipDic.Keys)
            {
                sb.Append("\t" + key + ",\n");
            }
            sb.Append("}");

            FileUtility.WriteToRelativePath(finalPath, sb.ToString());

            AssetDatabase.Refresh();

            Debug.Log($"Generated MusicEnum in {finalPath}");
#endif
        }

        [ContextMenu("GenerateSoundListDic")]
        internal void GenerateSoundListDic()
        {
#if UNITY_EDITOR
            var path = FileUtility.GetFilePath(nameof(SoundDatabase), FileType.Script);
            var finalPath = Path.Combine(Path.GetDirectoryName(path), "SoundListEnum.cs");
            if (File.Exists(finalPath))
            {
                File.Delete(finalPath);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("public enum SoundListEnum \n");
            sb.Append("{\n");
            foreach (var key in soundListDic.Keys)
            {
                sb.Append("\t" + key + ",\n");
            }
            sb.Append("}");

            FileUtility.WriteToRelativePath(finalPath, sb.ToString());

            AssetDatabase.Refresh();

            Debug.Log($"Generated SoundListEnum in {finalPath}");
#endif
        }
    }
}