using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatPlayer : MonoBehaviour
{
    public CombatPlayer Instance;
    //public List<Creature> myCreatures;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
}
