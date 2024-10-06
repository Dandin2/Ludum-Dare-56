using UnityEditor;
using UnityEngine;

public class CreateEgg
{
    [MenuItem("Assets/Create/Egg")]
    public static void Create()
    {
        EggStats ati = ScriptableObject.CreateInstance<EggStats>();
        AssetDatabase.CreateAsset(ati, "Assets/ScriptableObjects/Eggs/NewEgg.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = ati;
    }
}

public class EggStats : ScriptableObject
{
    public float MinHatchTime;

    public float MaxHatchTime;

    public CreatureType CreatureType;

    public GameObject EggPrefab;
}
