using System;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Core.FileUtil
{
    public enum FileType
    {
        Script,
        Prefab,
        ScriptableObject,
    }

    public class FileUtility
    {
        /// <summary>
        /// Get writeable path 
        /// </summary>
        /// <param name="relativePath">E.g: Folder/Folder/FileName.txt</param>
        /// <returns></returns>
        public static string GetWriteablePath(string relativePath)
        {
#if UNITY_EDITOR
            return Path.Combine(Directory.GetParent(Application.dataPath).FullName, "WriteablePath", relativePath);
#else
            return Path.Combine(Application.persistentDataPath, relativePath);
#endif
        }

        public static string GetFilePath(string fileName, FileType type)
        {
#if UNITY_EDITOR
            var assets = AssetDatabase.FindAssetGUIDs($"t:{type.ToString()} {fileName}");
            var path = AssetDatabase.GUIDToAssetPath(assets[0]);
            return path;
#endif
            return "";
        }

        public static string GetDataParentPath(string relativePath)
        {
            return Path.Combine(Directory.GetParent(Application.dataPath).FullName, relativePath);
        }

        /// <summary>
        /// This method will write to RelativePath start with Assets/
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="content"></param>
        public static void WriteToRelativePath(string relativePath, string content)
        {
            var dataPathFolder = Path.GetDirectoryName(Application.dataPath);
            var finalPath = Path.Combine(dataPathFolder, relativePath);
            var dir = Path.GetDirectoryName(finalPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            File.WriteAllText(relativePath, content);
        }

        /// <summary>
        /// This method will write to WriteablePath with relativePath combine
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="content"></param>
        public static void WriteToPath(string relativePath, string content)
        {
            var fullPath = GetWriteablePath(relativePath);
            var dir = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            File.WriteAllText(fullPath, content);
        }

        public static bool IsFileExist(string relativePath)
        {
            var fullPath = GetWriteablePath(relativePath);
            if (File.Exists(fullPath))
            {
                return true;
            }
            return false;
        }

        public static string ReadTextFromPath(string relativePath)
        {
            if (File.Exists(GetWriteablePath(relativePath)))
            {
                var fullPath = GetWriteablePath(relativePath);
                return File.ReadAllText(fullPath);
            }
            return String.Empty;
        }

        public static byte[] ReadBytesFromPath(string relativePath)
        {
            return File.ReadAllBytes(GetWriteablePath(relativePath));
        }
    }
}