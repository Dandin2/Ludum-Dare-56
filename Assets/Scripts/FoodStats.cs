using UnityEditor;
using UnityEngine;

public class CreateFood
{
    [MenuItem("Assets/Create/Food")]
    public static void Create()
    {
        FoodStats ati = ScriptableObject.CreateInstance<FoodStats>();
        AssetDatabase.CreateAsset(ati, "Assets/ScriptableObjects/Foods/NewFood.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = ati;
    }
}

public class FoodStats : ScriptableObject
{
    public int HungerRestore;

    public float AttractionRadius;

    public int NumberOfUses;

    public int Cost;

    public Sprite ShopImage;

    public string Description;

    public GameObject FoodPrefab;
}
