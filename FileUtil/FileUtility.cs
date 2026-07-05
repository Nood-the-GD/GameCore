using System.IO;
using UnityEngine;

namespace Core.FileUtil
{
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

        public static string GetDataParentPath(string relativePath)
        {
            return Path.Combine(Directory.GetParent(Application.dataPath).FullName, relativePath);
        }

        public static void WriteToPath(string relativePath, string content)
        {
            File.WriteAllText(GetWriteablePath(relativePath), content);
        }

        public static string ReadTextFromPath(string relativePath)
        {
            return File.ReadAllText(GetWriteablePath(relativePath));
        }

        public static byte[] ReadBytesFromPath(string relativePath)
        {
            return File.ReadAllBytes(GetWriteablePath(relativePath));
        }
    }
}