using System;
using System.Collections.Generic;
using System.IO;
using Mine.Code.Framework.Extension;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Mine.Code.App.Model
{
    [Serializable]
    public class JsonDBModel<T> where T : JsonDBModel<T>
    {
        [SerializeField] protected string path;
        [SerializeField] TextAsset[] jsons;
        public Dictionary<string, JObject> DB { get; private set; }

        public void SaveDB()
        {
            DB.ForEach(kvp => { File.WriteAllText($"{Application.persistentDataPath}/{path}/{kvp.Key}.json", kvp.Value.ToString()); });
        }

        public void LoadDB(params string[] keys)
        {
            if (!Directory.Exists($"{Application.persistentDataPath}/{path}")) InitDB();
            
            DB = new Dictionary<string, JObject>();
            keys.ForEach(key =>
            {
                string json = File.ReadAllText($"{Application.persistentDataPath}/{path}/{key}.json");
                if (!string.IsNullOrEmpty(json))
                {
                    DB[key] = JObject.Parse(json);
                }
            });
        }

        void InitDB()
        {
            Directory.CreateDirectory($"{Application.persistentDataPath}/{this.path}");
            jsons.ForEach(json =>
            {
                string key = json.name;
                string path = $"{Application.persistentDataPath}/{this.path}/{key}.json";
                
                File.WriteAllText(path, json.text);
            });
        }
    }
}