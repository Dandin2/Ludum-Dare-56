using UnityEditor;
using UnityEngine;

public class CreateToy
{
    [MenuItem("Assets/Create/Toy")]
    public static void Create()
    {
        ToyStats ati = ScriptableObject.CreateInstance<ToyStats>();
        AssetDatabase.CreateAsset(ati, "Assets/ScriptableObjects/Toys/NewToy.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = ati;
    }
}

public class ToyStats : ScriptableObject
{
    public int EntertainmentRestore;

    public float AttractionRadius;

    public int NumberOfUses;

    public string Description;

    public GameObject ToyPrefab;
}
