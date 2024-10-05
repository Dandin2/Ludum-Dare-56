using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatCreature : MonoBehaviour
{
    [HideInInspector]
    public ActiveCreatureStats myStats;

    //Delete this when we get actual sprites
    public void SetType(CreatureType type)
    {
        myStats.myType = type;
        if (GetComponent<SpriteRenderer>() != null)
        {
            if (type == CreatureType.Air)
                GetComponent<SpriteRenderer>().color = Color.cyan;
            if (type == CreatureType.Earth)
                GetComponent<SpriteRenderer>().color = Color.gray;
            if (type == CreatureType.Fire)
                GetComponent<SpriteRenderer>().color = Color.red;
            if (type == CreatureType.Plant)
                GetComponent<SpriteRenderer>().color = Color.green;
            if (type == CreatureType.Water)
                GetComponent<SpriteRenderer>().color = Color.blue;
        }
    }
}

[Serializable]
public class ActiveCreatureStats
{
    public int health;
    public int damage;
    public int block;
    public int specialDamage;
    public int specialContribution;
    public int hunger;
    public int hygene;
    public int entertainment;
    public CreatureType myType;
}
