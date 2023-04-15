using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using File = System.IO.File;

namespace Services.SaveServices
{
    public static class LocalSaveProvider
    {
        private static readonly string _pathJson = Application.persistentDataPath + "/SavesJson.json";

        public static SaveData LoadSave()
        {
            if (!File.Exists(_pathJson))
            {
                return null;
            }
            
            SaveData saveData = JsonConvert.DeserializeObject<SaveData>(
                File.ReadAllText(_pathJson), 
                new JsonSerializerSettings 
            { 
                TypeNameHandling = TypeNameHandling.Auto,
                NullValueHandling = NullValueHandling.Ignore,
            });

            return saveData;
        }

        public static void SaveObjectToJson(SaveData saveData)
        {
            JsonSerializer serializer = new JsonSerializer
            {
                TypeNameHandling = TypeNameHandling.Auto, 
                Formatting = Formatting.Indented
            };

            using (StreamWriter sw = new StreamWriter(_pathJson))
            {
                using (JsonWriter writer = new JsonTextWriter(sw)) 
                { 
                    serializer.Serialize(writer, saveData, typeof(SaveData)); 
                }
            }
        }
        
        public static void RemoveSaves()
        {
            File.Delete(_pathJson);
        }
    }
}
