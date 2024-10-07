using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public static WorldManager instance;

    public List<CreatureStats> CreatureBases;
    public List<EggStats> EggBases;
    public List<FoodStats> FoodBases;
    public List<ToyStats> ToyBases;

    internal List<ActiveCreatureStats> activeCreatureStats = new List<ActiveCreatureStats>();
    internal List<ActiveItemStats> foodInventory = new List<ActiveItemStats>();
    internal List<ActiveItemStats> toysInventory = new List<ActiveItemStats>();

    internal bool MusicMuted;
    internal bool SfxMuted;

    internal int GoldAmount;
    public int level;

    public bool isTesting = false;
    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            if (isTesting)
            {
                GoldAmount = 30000;
                var allPossibleCreatureStats = CreatureBases;
                for (int i = 0; i < 10; i++)
                {
                    var randomCreatureIndex = UnityEngine.Random.Range(0, allPossibleCreatureStats.Count);
                    var creature = allPossibleCreatureStats.ElementAt(randomCreatureIndex);
                    activeCreatureStats.Add(new ActiveCreatureStats()
                    {
                        name = creature.CreatureName,
                        health = creature.HitPoints,
                        damage = creature.Attack,
                        block = creature.Defence,
                        hunger = creature.Hunger - UnityEngine.Random.Range(0, creature.Hunger),
                        entertainment = creature.Entertainment - UnityEngine.Random.Range(0, creature.Entertainment),
                        hygene = creature.Hygiene - UnityEngine.Random.Range(0, creature.Hygiene),
                        exhausted = false,
                        myType = creature.CreatureType
                    });
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    internal bool HasEnoughGold(int amount)
    {
        return GoldAmount >= amount;
    }

    internal void RemoveGold(int amount)
    {
        GoldAmount -= amount;
    }

    internal void AddGold(int amount)
    {
        GoldAmount += amount;
    }
}

[Serializable]
public class ActiveItemStats
{
    public string name;
}
