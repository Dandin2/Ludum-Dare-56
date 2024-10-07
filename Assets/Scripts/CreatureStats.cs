using System;
using UnityEditor;
using UnityEngine;

//public class CreateCreature
//{
//    [MenuItem("Assets/Create/Creature")]
//    public static void Create()
//    {
//        CreatureStats ati = ScriptableObject.CreateInstance<CreatureStats>();
//        AssetDatabase.CreateAsset(ati, "Assets/ScriptableObjects/Creatures/NewCreature.asset");
//        AssetDatabase.SaveAssets();

//        EditorUtility.FocusProjectWindow();

//        Selection.activeObject = ati;
//    }
//}

public class CreatureStats : ScriptableObject
{
    public CreatureType CreatureType;

    public string CreatureName;

    public int HitPoints;

    public int Defence;

    public int Attack;

    public int Hunger;

    public int Entertainment;

    public int Hygiene;

    public float Speed;

    public GameObject CreaturePrefab;
}
