#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Debug = Mine.Code.Framework.Util.Debug.Debug;

namespace Mine.Code.Framework.Editor.CopySelectFileGUID
{
    public class CopySelectFileGuid : EditorWindow
    {
        [MenuItem("Assets/GUID Copy")]
        private static void GetSelectedAssetGuid()
        {
            if (Selection.assetGUIDs.Length > 0)
            {
                string selectedGUID = Selection.assetGUIDs[0];
                Debug.Log($"Selected asset GUID : {selectedGUID}");
                TextEditor textEditor = new UnityEngine.TextEditor();
                textEditor.text = selectedGUID;
                textEditor.SelectAll();
                textEditor.Copy();
            }
            else
            {
                Debug.LogWarning("No asset selected");
            }
        }
    }
}
#endif