using System.Linq;
using Mine.Code.Framework.Manager.UINavigator.Runtime;
using Mine.Code.Framework.Manager.UINavigator.Runtime.Sheet;
using UnityEditor;
using UnityEngine;

namespace Mine.Code.Framework.Manager.UINavigator.Editor
{
    [CustomEditor(typeof(SheetContainer))]
    public class SheetContainerEditor : UIContainerEditor
    {
        #region Fields

        SerializedProperty instantiateType;
        SerializedProperty registerSheetsByPrefab;
        SerializedProperty registerSheetsByAddressable;
        SerializedProperty hasDefault;
        readonly string[] toggleArray = { "On", "Off" };

        #endregion

        #region Properties
        
        SheetContainer Target => target as SheetContainer;
        protected override string[] PropertyToExclude() => base.PropertyToExclude().Concat(new[]
        {
            $"<{nameof(SheetContainer.InstantiateType)}>k__BackingField", 
            $"<{nameof(SheetContainer.RegisterSheetsByPrefab)}>k__BackingField",
#if ADDRESSABLE_SUPPORT
            $"<{nameof(SheetContainer.RegisterSheetsByAddressable)}>k__BackingField",
#endif
            $"<{nameof(SheetContainer.HasDefault)}>k__BackingField"
        }).ToArray();

        #endregion

        #region Unity Lifecycle

        protected override void OnEnable()
        {
            base.OnEnable();
            instantiateType = serializedObject.FindProperty($"<{nameof(SheetContainer.InstantiateType)}>k__BackingField");
            registerSheetsByPrefab = serializedObject.FindProperty($"<{nameof(SheetContainer.RegisterSheetsByPrefab)}>k__BackingField");
#if ADDRESSABLE_SUPPORT
            registerSheetsByAddressable = serializedObject.FindProperty($"<{nameof(SheetContainer.RegisterSheetsByAddressable)}>k__BackingField");
#endif
            hasDefault = serializedObject.FindProperty($"<{nameof(SheetContainer.HasDefault)}>k__BackingField");
        }

        #endregion

        #region GUI Process

        protected override void AdditionalGUIProcess()
        {
            var area = EditorGUILayout.BeginVertical();
            {
                GUI.Box(area, GUIContent.none);
                DrawTitleField("Initialize Setting");
                EditorGUI.indentLevel++;
                {
                    EditorGUILayout.PropertyField(instantiateType, GUIContent.none);
                    switch (Target.InstantiateType)
                    {
                        case InstantiateType.InstantiateByPrefab:
                            EditorGUILayout.PropertyField(registerSheetsByPrefab);
                            break;
#if ADDRESSABLE_SUPPORT
                        case InstantiateType.InstantiateByAddressable:
                            EditorGUILayout.PropertyField(registerSheetsByAddressable);
                            break;
#endif
                    }
                    
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.PrefixLabel(new GUIContent("Has Default"));
                        var select = GUILayout.Toolbar(hasDefault.boolValue ? 0 : 1, toggleArray);
                        hasDefault.boolValue = select == 0;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.Space(9);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
        }

        #endregion
    }
}