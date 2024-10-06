using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public static WorldManager instance;

    internal List<ActiveCreatureStats> activeCreatureStats;

    internal int GoldAmount;
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
