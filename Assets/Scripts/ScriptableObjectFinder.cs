using UnityEditor;
using UnityEngine;

public class ScriptableObjectFinder
{
    public static T FindScriptableObjectByName<T>(string name) where T : ScriptableObject
    {
        name = name.Replace(" ", "");
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name); // Find all ScriptableObjects of type T
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            T scriptableObject = AssetDatabase.LoadAssetAtPath<T>(path);
            if (scriptableObject != null && scriptableObject.name == name)
            {
                return scriptableObject; // Return the ScriptableObject with the matching name
            }
        }

        return null; // No matching ScriptableObject found
    }
}
