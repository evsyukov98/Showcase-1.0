using UnityEditor;
using UnityEngine;

namespace Services.SaveServices.Editor
{
    public class CustomMenuItem : MonoBehaviour
    {
        [MenuItem("Tools/Saves/Delete Saves")]
        public static void DeleteSaves()
        {
            LocalSaveProvider.RemoveSaves();
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("Saves deleted");
        }
    }
}
