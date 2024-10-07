using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpecialSkillInfo : ScriptableObject
{
    public string SkillName;
    public string TextOnHover;
    public string TextOnUse;
    public CreatureType requiredType;
    public int requiredAmount;

    public List<MainEffect> effects;
    public List<CreatureEffect> effectsOnCreatures;

    public int ultimateMeterChange;

    public GameObject OnEnemiesAnimation;
    public GameObject OnSelfAnimation;
}

//These effects happen only to specific creature types, and do not count as the main effect of what you're using
[Serializable]
public class CreatureEffect
{
    public CreatureType type;
    public int quantity;
    public float quantityPercent;
    public int turns;

    public bool exhaust;
    public bool ready;

    public int heal;

    public int hungerChange;
    public int hygeneChange;
    public int happinessChange;

    public int damageMod;
    public int hpMod;
    public int blockMod;

    public int takeDamage;
}

//This will modify whatever the main effect is of the skill you're using by a certain amount
[Serializable]
public class CreatureEffectModification
{
    public CreatureType type;
    public float modPerActiveCreatureType;
}

[Serializable]
public class MainEffect
{
    public CreatureType elementType;
    public CombatTarget target;
    public int damage;
    public int heal;
    public int block;

    public List<CreatureEffectModification> mods;
}

public enum CombatTarget
{
    Self,
    Opponent
}


//public class CreateSpecialSkillInfo
//{
//    [MenuItem("Assets/Create Scriptable/SpecialSkillInfo")]
//    public static void Create()
//    {
//        SpecialSkillInfo ati = ScriptableObject.CreateInstance<SpecialSkillInfo>();
//        AssetDatabase.CreateAsset(ati, "Assets/ScriptableObjects/PlayerSkills/NewSpecialSkillInfo.asset");
//        AssetDatabase.SaveAssets();

//        EditorUtility.FocusProjectWindow();

//        Selection.activeObject = ati;
//    }
//}