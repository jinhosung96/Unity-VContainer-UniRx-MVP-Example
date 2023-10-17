using System;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace Mine.Code.App.Model
{
    [Serializable]
    public class JellyFarmJsonDBModel : JsonDBModel<JellyFarmJsonDBModel>
    {
        public JArray JellyPresets => DB["JellyPreset"]["presets"] as JArray;
        public JToken Gelatin => DB["Currency"]["gelatin"];
        public JToken Gold => DB["Currency"]["gold"];
        public JArray Jellies => DB["Field"]["jellies"] as JArray;

        public JToken ApartmentLevel => DB["Plant"]["apartmentLevel"];
        public JToken ClickLevel => DB["Plant"]["clickLevel"];
        public JArray Apartment => DB["Upgrade"]["apartment"] as JArray;
        public JArray Click => DB["Upgrade"]["click"] as JArray;
    
        #region EditorOnly

#if UNITY_EDITOR
        [ContextMenu("Print Path")]
        public void PrintPath()
        {
            Debug.Log($"{Application.persistentDataPath}/{path}");
        }
    
        [MenuItem("DB/Reset")]
        public static void ResetDBEditor()
        {
            string path = $"{Application.persistentDataPath}/Json";
            foreach (string directory in Directory.GetDirectories(path))
            {
                try
                {
                    Directory.Delete(directory, true);
                }
                catch (IOException) 
                {
                    Directory.Delete(directory, true);
                }
                catch (UnauthorizedAccessException)
                {
                    Directory.Delete(directory, true);
                }
            }
            try
            {
                Directory.Delete(path, true);
            }
            catch (IOException) 
            {
                Directory.Delete(path, true);
            }
            catch (UnauthorizedAccessException)
            {
                Directory.Delete(path, true);
            }
        }
#endif

        #endregion
    }
}
