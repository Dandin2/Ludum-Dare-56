using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyInfo : ScriptableObject
{
    public string enemyName;
    public EnemyType type;
    public int health;
    public List<EnemyAttackInfo> myAttacks;
    public int defaultBlock;
    public GameObject prefab;
}
public class CreateEnemyInfo
{
    [MenuItem("Assets/Create Scriptable/Create Enemy Info")]
    public static void Create()
    {
        EnemyInfo ati = ScriptableObject.CreateInstance<EnemyInfo>();
        AssetDatabase.CreateAsset(ati, "Assets/ScriptableObjects/EnemyInfo/NewEnemyInfo.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = ati;
    }
}

