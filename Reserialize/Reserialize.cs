using System;
using UnityEngine;
using UnityEngine.UIElements;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Core.Reserialize
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ReserializeAttribute : Attribute { }

#if UNITY_EDITOR
    public static class ReserializeEditor
    {
        [InitializeOnLoadMethod]
        [MenuItem("Tools/Reserialize")]
        public static void Reserialize()
        {
            var scriptGUIDs = new List<string>();
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {

                var classesInAssembly = asm.GetTypes().Where(t => t.IsClass && t.IsDefined(typeof(ReserializeAttribute)));
                scriptGUIDs.AddRange(classesInAssembly.Select(cls => GetScriptGUID(cls)));
            }

            var scenes = GetAllScenesPath();
            var prefabPath = GetAllPrefabsPath();

            List<string> result = new();
            foreach (var guid in scriptGUIDs)
            {
                //File scenes
                var scene = GrepExtension.Grep(@"Assets/_Project", guid, "*.unity");

                //File prefab
                var asset = GrepExtension.Grep(@"Assets/_Project", guid, "*.asset");

                Debug.Log("guid: " + guid);
                result.AddRange(scene.Select(x => x.file));
                result.AddRange(asset.Select(x => x.file));
            }


            AssetDatabase.ForceReserializeAssets(result, ForceReserializeAssetsOptions.ReserializeAssets);
        }


        private static string GetScriptGUID(Type type)
        {
            var guids = AssetDatabase.FindAssets($"t:Script {type.Name}");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (Path.GetFileNameWithoutExtension(path) == type.Name)
                {
                    return guid;
                }
            }
            throw new Exception($"No script asset found matching type {type.FullName}");
        }
        private static List<string> GetAllPrefabsPath()
        {
            return AssetDatabase.FindAssets("t:Prefab").Select(guid => AssetDatabase.GUIDToAssetPath(guid)).ToList();
        }

        private static List<string> GetAllScenesPath()
        {
            return EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToList();
        }
    }

}
#endif