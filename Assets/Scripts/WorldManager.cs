using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public static WorldManager instance;

    internal List<ActiveCreatureStats> activeCreatureStats = new List<ActiveCreatureStats>();

    internal int GoldAmount;

    public bool isTesting = false;
    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if(isTesting)
        {
            GoldAmount = 100000;
            var allPossibleCreatureStats = ScriptableObjectFinder.GetAllCreatureStats();
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
                    hunger = creature.Hunger,
                    entertainment = creature.Entertainment,
                    hygene = creature.Hygiene,
                    exhausted = false,
                    myType = creature.CreatureType
                });
            }
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
