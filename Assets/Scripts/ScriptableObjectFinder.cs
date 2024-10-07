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

    public static List<FoodStats> GetAllFoodStats()
    {
        var foods = new List<FoodStats>();
        string[] guids = AssetDatabase.FindAssets("t:FoodStats");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            FoodStats food = AssetDatabase.LoadAssetAtPath<FoodStats>(path);
            if (food != null)
            {
                foods.Add(food);
            }
        }

        return foods;
    }

    public static List<ToyStats> GetAllToyStats()
    {
        var toys = new List<ToyStats>();
        string[] guids = AssetDatabase.FindAssets("t:ToyStats");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ToyStats toy = AssetDatabase.LoadAssetAtPath<ToyStats>(path);
            if (toy != null)
            {
                toys.Add(toy);
            }
        }

        return toys;
    }

    public static List<EggStats> GetAllEggStats()
    {
        var eggs = new List<EggStats>();
        string[] guids = AssetDatabase.FindAssets("t:EggStats");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            EggStats egg = AssetDatabase.LoadAssetAtPath<EggStats>(path);
            if (egg != null)
            {
                eggs.Add(egg);
            }
        }

        return eggs;
    }
}
