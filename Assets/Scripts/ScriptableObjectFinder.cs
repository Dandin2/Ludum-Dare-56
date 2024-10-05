using System.Collections.Generic;
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

    public static List<CreatureStats> GetAllCreatureStats()
    {
        var creatures = new List<CreatureStats>();
        string[] guids = AssetDatabase.FindAssets("t:CreatureStats");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            CreatureStats creature = AssetDatabase.LoadAssetAtPath<CreatureStats>(path);
            if (creature != null)
            {
                creatures.Add(creature);
            }
        }

        return creatures;
    }
}
