using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CombatEnemy : MonoBehaviour
{
    public CombatEnemy Instance;
    public EnemyInfo myInfo;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    public EnemyAttackInfo TakeTurn()
    {
        float sum = myInfo.myAttacks.Sum(x => x.ChanceToUseAttack);
        float rng = UnityEngine.Random.Range(0, sum);
        float cur = 0;
        foreach(EnemyAttackInfo eai in myInfo.myAttacks)
        {
            if (cur + eai.ChanceToUseAttack > rng)
                return eai;

            cur += eai.ChanceToUseAttack;
        }

        return myInfo.myAttacks.Last();
    }
}




public enum EnemyType
{
    None,
    Snail,
    Cat
}

