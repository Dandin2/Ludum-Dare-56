using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyAttackInfo : ScriptableObject
{
    //todo: better logic for determining attacks/attack patterns?
    public float ChanceToUseAttack;

    public int WindUpTurns = 0;
    public string WindUpMessage;
    public int Damage;
    public int NumToDamage;
    public float PercentToDamage;
    //public List<CreatureType> PriorityTargets = new List<CreatureType>();
    public int NumToExhaust;
    public int NumToReady;//in case you want em to be nice.

    public int SelfHealAmount;
    public int SelfBlockAmount;

    public GameObject OnEnemiesAnimation;
    public GameObject OnSelfAnimation;
}

public class CreateEnemyAttackInfo
{
    [MenuItem("Assets/Create Scriptable/EnemyAttackInfo")]
    public static void Create()
    {
        EnemyAttackInfo ati = ScriptableObject.CreateInstance<EnemyAttackInfo>();
        AssetDatabase.CreateAsset(ati, "Assets/ScriptableObjects/EnemyInfo/NewEnemyAttackInfo.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = ati;
    }
}