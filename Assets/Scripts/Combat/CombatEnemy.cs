using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CombatEnemy : MonoBehaviour
{
    public HealthBar HealthBar;
    public ParticleSystemForceField psff;
    public Component ParticleAbsorber;

    public static CombatEnemy Instance;
    [HideInInspector]
    public EnemyInfo myInfo;
    [HideInInspector]
    public GameObject EnemyPrefab;

    private int currentHP;
    private bool firstLoad;

    public int block = 0;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if (!firstLoad && CombatManager.Instance != null)
        {
            firstLoad = true;
            CombatManager.Instance.RegisterInstanceCreated();
        }

    }

    public void SetEnemy(EnemyInfo info)
    {
        myInfo = info;
        currentHP = myInfo.health;
        HealthBar.SetInitial(myInfo.health, myInfo.health, 0);
        GameObject go = Instantiate(myInfo.prefab);
        go.transform.SetParent(transform);
        go.transform.SetLocalPosition(0, -0.5f, -1);
        go.transform.localScale = new Vector3(1, 1, 1);
        go.GetComponent<RunOnAnimationDone>().SetWhenDone(() => { AttackDone(); });
    }

    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        HealthBar.UpdateHealth(myInfo.health, currentHP, 0, () => { CombatManager.Instance.PlayerTurnDoneAnimating(); });
        if (currentHP > 0)
            GetComponentInChildren<Animator>().SetTrigger("Hit");
        else
            GetComponentInChildren<Animator>().SetBool("Alive", false);
    }

    public void Heal(int hp)
    {
        currentHP = Math.Min(myInfo.health, currentHP + hp);
        HealthBar.UpdateHealth(myInfo.health, currentHP, 0, null);
    }

    public void Block(int percent)
    {
        block += percent;
    }

    public EnemyAttackInfo TakeTurn()
    {
        block = myInfo.defaultBlock;

        float sum = myInfo.myAttacks.Sum(x => x.ChanceToUseAttack);
        float rng = UnityEngine.Random.Range(0, sum);
        float cur = 0;
        EnemyAttackInfo chosen = null;
        foreach (EnemyAttackInfo eai in myInfo.myAttacks)
        {
            if (cur + eai.ChanceToUseAttack > rng)
            {
                chosen = eai;
                break;
            }

            cur += eai.ChanceToUseAttack;
        }
        if (chosen == null)
            chosen = myInfo.myAttacks.Last();

        if (chosen.OnSelfAnimation != null)
        {
            GameObject go = Instantiate(chosen.OnSelfAnimation);
            go.transform.parent = transform;
            go.transform.SetLocalPosition(0, 0, -10);
        }

        if (chosen.SelfHealAmount > 0)
            Heal(chosen.SelfHealAmount);
        if (chosen.SelfBlockAmount > 0)
            Block(chosen.SelfBlockAmount);

        //maybe just do this always?  So there's some sort of visual indicator they're doing something?
        GetComponentInChildren<Animator>().SetTrigger("Attack");
        return chosen;
    }

    public void AttackDone()
    {
        CombatManager.Instance.EnemyTurnDoneAnimating();
    }
}




public enum EnemyType
{
    None,
    EyePlant,
    Skull,
    BlueNimkip,
    PinkNimkip,
    Rabbit,
    Spaceship
}

