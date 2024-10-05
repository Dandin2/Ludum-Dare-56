using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    public void StartPlayerTurn()
    {

    }

    public void AttackEnemy()
    {

    }

    public void Block()
    {

    }

    public void PerformSpecialMove()
    {

    }

    public void Ultimate()
    {

    }
}
