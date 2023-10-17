#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Mine.Code.Framework.Editor.ScriptTemplateModifier
{
    public sealed class ScriptTemplateProcessor : AssetModificationProcessor
    {
        public static void OnWillCreateAsset(string metaPath)
        {
            var suffixIndex = metaPath.LastIndexOf(".meta");
            if (suffixIndex < 0) return;

            var scriptPath = metaPath.Substring(0, suffixIndex);
            var directoryPath = Path.GetDirectoryName(scriptPath);
            var className = Path.GetFileNameWithoutExtension(scriptPath);
            var extname = Path.GetExtension(scriptPath);
            
            if (extname != ".cs") return;

            string templatePath = default;

            if (className.Contains("Context"))
            {
                if (className.Contains("Sheet")) templatePath = AssetDatabase.GUIDToAssetPath("b856b5b54f16f2e43abd3421aa7a741f");
                else if (className.Contains("Page")) templatePath = AssetDatabase.GUIDToAssetPath("ea76c5002baf9544587bbc93d5ddb9ce");
                else if(className.Contains("Modal")) templatePath = AssetDatabase.GUIDToAssetPath("4f88545e18c7b3a45a6137fa869becbb");
                else templatePath = AssetDatabase.GUIDToAssetPath("e49e02b5517aba148b70f7d44d5406be");
            }
            else if (className.Contains("Presenter")) templatePath = AssetDatabase.GUIDToAssetPath("b3613ac5dd2f4aa4c8cf6907ab6ebb0e");
            
            if(templatePath == default) return;
            
            string content = File.ReadAllText(templatePath);
            
            content = content.Replace("#NAMESPACE#", directoryPath.Replace("Assets\\", "").Replace("\\", "."));
            content = content.Replace("#SCRIPTNAME#", className);
            content = content.Replace("#NOTRIM#", "");

            if (scriptPath.StartsWith("Assets/")) scriptPath = scriptPath.Substring("Assets/".Length);

            var fullPath = Path.Combine(Application.dataPath, scriptPath);
            File.WriteAllText(fullPath, content);
            AssetDatabase.Refresh();
        }
    }
}
#endif