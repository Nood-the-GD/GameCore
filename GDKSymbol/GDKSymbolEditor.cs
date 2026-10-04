#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Core.GDKSymbol
{
    [CustomEditor(typeof(GDKSymbol))]
    public class GDKSymbolEditor : Editor
    {
        private const string GeneratedFolder = "Assets/_Project/_Asset/Scripts/Core/GDKSymbol/Generated";
        private const string GeneratedFileSuffix = ".GDKDebug.cs";
        // Project-owned list of namespaces, outside the Core submodule so each project keeps its own.
        private const string ManifestPath = "Assets/gdksymbol.csc";

        private static readonly string[] WrappedMethods = { "Log", "LogWarning", "LogError" };

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Apply"))
                Apply((GDKSymbol)target);
        }

        [InitializeOnLoadMethod]
        private static void RegenerateFromManifest()
        {
            // Core's own SoundManager needs Module_Sound, so a fresh project starts with it.
            if (!File.Exists(ManifestPath))
                WriteManifest(new List<string> { "Module_Sound" });

            if (GenerateFiles(ReadManifest()))
                EditorApplication.delayCall += AssetDatabase.Refresh;
        }

        private static void Apply(GDKSymbol symbol)
        {
            var namespaces = symbol.Entries.Keys.Where(n => !string.IsNullOrWhiteSpace(n)).Select(n => n.Trim()).ToList();
            WriteManifest(namespaces);
            GenerateFiles(namespaces);
            AssetDatabase.Refresh();
            SyncDefines(symbol);
        }

        private static List<string> ReadManifest()
        {
            return File.ReadAllLines(ManifestPath)
                .Select(line => line.Trim())
                .Where(line => line.Length > 0 && !line.StartsWith("#"))
                .Distinct()
                .ToList();
        }

        private static void WriteManifest(List<string> namespaces)
        {
            var sb = new StringBuilder();
            sb.AppendLine("# GDKSymbol namespaces for this project. Written by GDKSymbol Apply; read on every Editor reload.");
            foreach (var nameSpace in namespaces)
                sb.AppendLine(nameSpace);
            File.WriteAllText(ManifestPath, sb.ToString());
        }

        // Returns true when any file was written or deleted, so callers only refresh when needed.
        private static bool GenerateFiles(List<string> namespaces)
        {
            Directory.CreateDirectory(GeneratedFolder);

            var changed = false;
            var expectedFiles = new HashSet<string>();
            foreach (var nameSpace in namespaces)
            {
                var fileName = Sanitize(nameSpace) + GeneratedFileSuffix;
                expectedFiles.Add(fileName);

                var path = Path.Combine(GeneratedFolder, fileName);
                var script = GenerateScript(nameSpace);
                if (File.Exists(path) && File.ReadAllText(path) == script) continue;

                File.WriteAllText(path, script);
                changed = true;
            }

            foreach (var existingFile in Directory.GetFiles(GeneratedFolder, "*" + GeneratedFileSuffix))
            {
                if (expectedFiles.Contains(Path.GetFileName(existingFile))) continue;

                File.Delete(existingFile);
                if (File.Exists(existingFile + ".meta"))
                    File.Delete(existingFile + ".meta");
                changed = true;
            }

            return changed;
        }

        // Entries drive the scripting define symbols: an enabled entry adds its define so the
        // matching [Conditional] logs compile in, a disabled/removed entry strips them from the build.
        private static void SyncDefines(GDKSymbol symbol)
        {
            var managed = new HashSet<string>();
            var enabled = new HashSet<string>();
            foreach (var (nameSpace, inBuild) in symbol.Entries)
            {
                if (string.IsNullOrWhiteSpace(nameSpace)) continue;

                var define = Sanitize(nameSpace);
                managed.Add(define);
                if (inBuild) enabled.Add(define);
            }

            foreach (var buildTarget in AllBuildTargets())
            {
                // Some NamedBuildTarget fields (e.g. deprecated platforms) are rejected by PlayerSettings.
                try
                {
                    var defines = PlayerSettings.GetScriptingDefineSymbols(buildTarget)
                        .Split(';', StringSplitOptions.RemoveEmptyEntries)
                        .Where(d => !managed.Contains(d))
                        .Concat(enabled)
                        .Distinct()
                        .ToArray();

                    PlayerSettings.SetScriptingDefineSymbols(buildTarget, defines);
                }
                catch (ArgumentException)
                {
                }
            }
        }

        private static IEnumerable<NamedBuildTarget> AllBuildTargets()
        {
            var seen = new HashSet<string>();
            foreach (var field in typeof(NamedBuildTarget).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (field.FieldType != typeof(NamedBuildTarget)) continue;

                var buildTarget = (NamedBuildTarget)field.GetValue(null);
                if (string.IsNullOrEmpty(buildTarget.TargetName) || buildTarget.TargetName == "Unknown") continue;

                if (seen.Add(buildTarget.TargetName))
                    yield return buildTarget;
            }
        }

        private static string Sanitize(string nameSpace) => nameSpace.Replace(".", "_");

        private static string GenerateScript(string nameSpace)
        {
            var define = Sanitize(nameSpace);
            var sb = new StringBuilder();
            sb.AppendLine("// Auto-generated by GDKSymbolEditor. Do not edit by hand.");
            sb.Append("namespace GDKSymbol.").AppendLine(nameSpace);
            sb.AppendLine("{");
            sb.AppendLine("    public static class Debug");
            sb.AppendLine("    {");
            foreach (var method in WrappedMethods)
            {
                sb.Append("        [System.Diagnostics.Conditional(\"").Append(define).AppendLine("\")]");
                sb.Append("        public static void ").Append(method)
                    .Append("(object message) { UnityEngine.Debug.").Append(method)
                    .Append("($\"[").Append(nameSpace).Append("] {message}\"); }").AppendLine();
            }
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
#endif
}
