﻿using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Core.Voxelworld
{
    public class VoxelTypeGenerator : MonoBehaviour
    {
        [DidReloadScripts]
        [MenuItem("UnityCore/Generated/Force refresh VoxelType.cs")]
        public static void ForceRefreshCode()
        {
            Directory.CreateDirectory(Application.dataPath + "/Generated/Core/Voxelworld");

            var voxelTypesDefinitions = AssetUtils.FindResourcePathsByType<VoxelTypesDefinition>();
            if (voxelTypesDefinitions.Length != 1) {
                Debug.LogError("None or multiple definition files found");
                return;
            }
            var assetPath = "Assets/Resources/" + voxelTypesDefinitions[0];
            var voxelTypesDefinition = AssetDatabase.LoadAssetAtPath<VoxelTypesDefinition>(assetPath);
            if (voxelTypesDefinition == null) {
                Debug.LogError("Failed to load VoxelTypesDefinition at '" + assetPath + "'");
                return;
            }

            GenerateVoxelType(voxelTypesDefinition);
            GenerateVoxelTypes(voxelTypesDefinition);
        }

        static void GenerateVoxelType(VoxelTypesDefinition voxelTypesDefinition)
        {
            var builder = new StringBuilder();
            builder.Append("// Generated by code. Do not edit.\r\n");
            builder.Append("\r\n");
            builder.Append("public enum VoxelType : byte\r\n");
            builder.Append("{\r\n");
            builder.Append("    None,\r\n");
            if (voxelTypesDefinition.types != null) {
                foreach (var type in voxelTypesDefinition.types) {
                    builder.Append("    " + type.name + ",\r\n");
                }
            }
            builder.Append("    COUNT\r\n");
            builder.Append("}\r\n");

            var scriptPath = Application.dataPath + "/Generated/Core/Voxelworld/VoxelType.gen.cs";
            File.WriteAllText(scriptPath, builder.ToString());
        }

        static void GenerateVoxelTypes(VoxelTypesDefinition voxelTypesDefinition)
        {
            var builder = new StringBuilder();
            builder.Append("// Generated by code. Do not edit.\r\n");
            builder.Append("\r\n");
            builder.Append("public static class VoxelTypes\r\n");
            builder.Append("{\r\n");

            builder.Append("    public static bool IsMergable(VoxelType type)\r\n");
            builder.Append("    {\r\n");
            builder.Append("        switch (type) {\r\n");
            if (voxelTypesDefinition.types != null) {
                foreach (var type in voxelTypesDefinition.types) {
                    if (!type.isNotMergable)
                        continue;

                    builder.Append("           case VoxelType." + type.name + ": return false;\r\n");
                }
            }
            builder.Append("            default: return true;\r\n");
            builder.Append("        }\r\n");
            builder.Append("    }\r\n");
            builder.Append("    \r\n");

            builder.Append("    public static bool IsTransparent(VoxelType type)\r\n");
            builder.Append("    {\r\n");
            builder.Append("        switch (type) {\r\n");
            if (voxelTypesDefinition.types != null) {
                foreach (var type in voxelTypesDefinition.types) {
                    if (!type.isTransparent)
                        continue;

                    builder.Append("           case VoxelType." + type.name + ": return true;\r\n");
                }
            }
            builder.Append("            default: return false;\r\n");
            builder.Append("        }\r\n");
            builder.Append("    }\r\n");

            builder.Append("}\r\n");

            var scriptPath = Application.dataPath + "/Generated/Core/Voxelworld/VoxelTypes.gen.cs";
            File.WriteAllText(scriptPath, builder.ToString());
        }
    }
}