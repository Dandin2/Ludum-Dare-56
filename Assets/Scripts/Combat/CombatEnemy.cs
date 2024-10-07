using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86;

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
        currentHP -= (dmg - (int)(dmg * block * 0.01f));
        HealthBar.UpdateHealth(myInfo.health, currentHP, 0, () =>
        {
            if (currentHP <= 0)
                CombatManager.Instance.Victory();
            else
                CombatManager.Instance.PlayerTurnDoneAnimating();
        });
        if (currentHP > 0)
            GetComponentInChildren<Animator>().SetTrigger("Hit");
        else
            GetComponentInChildren<Animator>().SetBool("Alive", false);
    }

    public void ReceiveEffect(SpecialSkillInfo ssi)
    {
        int totalDamage = 0;
        int heal = 0;
        int block = 0;
        float totalResistVuln = 0;
        foreach (MainEffect a in ssi.effects.Where(x => x.target == CombatTarget.Opponent))
        {
            float damage = a.damage * CombatPlayer.Instance.BaseDamageMod();
            float effectMod = 1;
            foreach (CreatureEffectModification b in a.mods)
            {
                effectMod += CombatPlayer.Instance.GetCreatureQuantity(b.type) * b.modPerActiveCreatureType * 0.01f;
            }
            damage = damage * effectMod;

            foreach (Resistance r in myInfo.strengths.Where(x => x.type == a.elementType))
            {
                damage = damage - (damage * r.resistPercent * 0.01f);
                totalResistVuln -= r.resistPercent;
            }

            foreach (Vulnerability v in myInfo.weaknesses.Where(x => x.type == a.elementType))
            {
                damage = damage + (damage * v.vulnPercent * 0.01f);
                totalResistVuln += v.vulnPercent;
            }

            totalDamage = (int)damage;
            heal += (int)(a.heal * effectMod);
            block += (int)(a.block * effectMod);
        }

        if (totalDamage > 0)
        {
            TakeDamage(totalDamage);
            if (totalResistVuln >= 20)
                CombatManager.Instance.TextDisplay.SetMessage("It's super effective!", false, null, 1);
            else if (totalResistVuln <= -20)
                CombatManager.Instance.TextDisplay.SetMessage("It's not very effective...", false, null, 1);
        }
        if (heal > 0)
            Heal(heal);
        if (block > 0)
            Block(block);


        if (ssi.OnEnemiesAnimation != null)
        {
            GameObject go = Instantiate(ssi.OnEnemiesAnimation);
            go.transform.parent = transform;
            go.transform.SetLocalPosition(0, 0, -10);
            if (totalDamage <= 0)
                go.GetComponent<OneTimeAnimation>()?.SetCompleteAction(() => { CombatManager.Instance.PlayerTurnDoneAnimating(); });
        }
        else if (totalDamage <= 0)
            CombatManager.Instance.PlayerTurnDoneAnimating();
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

    public EnemyAttackInfo ChooseSkillToUse()
    {
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

        return chosen;
    }
    public EnemyAttackInfo TakeTurn(EnemyAttackInfo chosen)
    {
        block = myInfo.defaultBlock;

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

        if (myInfo.type == EnemyType.Rabbit)
        {
            int rng = UnityEngine.Random.Range(0, 2);
            if (rng == 1)
                GetComponentInChildren<Animator>().SetTrigger("Attack");
            else
                GetComponentInChildren<Animator>().SetTrigger("Attack2");
        }
        else
        {
            //maybe just do this always?  So there's some sort of visual indicator they're doing something?
            GetComponentInChildren<Animator>().SetTrigger("Attack");
        }
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

[Serializable]
public class Resistance
{
    public CreatureType type;
    public float resistPercent;
}

[Serializable]
public class Vulnerability
{
    public CreatureType type;
    public float vulnPercent;
}