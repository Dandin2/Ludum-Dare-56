using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86;

public class CombatPlayer : MonoBehaviour
{
    public GameObject CreaturePrefab;
    public ParticleSystemForceField psff;

    public static CombatPlayer Instance;

    private List<CombatCreature> myCreatures = new List<CombatCreature>();

    private float minX;
    private float maxX;
    private float minY;
    private float maxY;

    private bool firstLoad;

    private int activeParticles;

    public float blockPercent;
    private float _ultimate;
    public float ultimate
    {
        get
        {
            return _ultimate;
        }
        set
        {
            _ultimate = value;
            CombatManager.Instance?.UpdateUltimateCharge(_ultimate);
        }
    }

    private List<CombatCreature> preppedCreatures = new List<CombatCreature>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            RectTransform bounds = (transform as RectTransform);
            minX = bounds.position.x - (bounds.rect.width * 0.5f) + .1f;
            maxX = bounds.position.x + (bounds.rect.width * 0.5f) - .1f;
            minY = bounds.position.y - (bounds.rect.height * 0.5f) + .1f;
            maxY = bounds.position.y + (bounds.rect.height * 0.5f) - .1f;
        }
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

    public void SetInitial()
    {
        foreach (ActiveCreatureStats acs in WorldManager.instance.activeCreatureStats)
        {
            GameObject go = Instantiate(CreaturePrefab);
            CombatCreature cc = go.GetComponent<CombatCreature>();
            myCreatures.Add(cc);
            cc.SetType(acs);
            cc.transform.parent = transform;
            cc.transform.position = new Vector3(minX + ((maxX - minX) * UnityEngine.Random.value),
                                                minY + ((maxY - minY) * UnityEngine.Random.value),
                                                transform.position.z - UnityEngine.Random.value);
            cc.GetComponent<ChaoticMover>().SetBounds(transform as RectTransform);
        }
    }

    public List<ActiveCreatureStats> GetCreatures()
    {
        return myCreatures.Select(x => x.myStats).ToList();
    }

    public int GetCreatureQuantity(CreatureType ct)
    {
        if (ct == CreatureType.All)
            return myCreatures.Where(x => !x.myStats.exhausted).Count();
        else
            return myCreatures.Where(x => !x.myStats.exhausted && x.myStats.myType == ct).Count();
    }

    public int CalculateUltimateDamage()
    {
        return (int)(10 + (90 * Mathf.Min(100, ultimate) * 0.01f));
    }

    public void ReceiveEnemyEffect(EnemyAttackInfo attackInfo)
    {
        int startCount = myCreatures.Count();
        if (attackInfo.OnEnemiesAnimation != null)
        {
            GameObject go = Instantiate(attackInfo.OnEnemiesAnimation);
            go.transform.parent = transform;
            go.transform.SetLocalPosition(0, 0, -10);
            go.GetComponent<OneTimeAnimation>()?.SetCompleteAction(() => { CombatManager.Instance.EnemyTurnDoneAnimating(); });
        }
        else
            CombatManager.Instance.EnemyTurnDoneAnimating();

        if (attackInfo.Damage > 0)
        {
            int numDamaging = 0;
            if (attackInfo.NumToDamage > 0)
                numDamaging = attackInfo.NumToDamage;
            else
                numDamaging = (int)(attackInfo.PercentToDamage * myCreatures.Count() * 0.01f);

            myCreatures.OrderBy(x => UnityEngine.Random.value).Take(numDamaging).ToList().ForEach(x =>
            {
                x.TakeDamage((int)(attackInfo.Damage * (100 - blockPercent) * (x.myStats.exhausted ? 1.2f : 1) * 0.01f));
            });

            myCreatures = myCreatures.Where(x => x.myStats.health > 0).ToList();
            int endCount = myCreatures.Count();
            if (endCount < startCount)
                CombatManager.Instance.lostCreatures = (startCount - endCount);
        }

        if (attackInfo.NumToExhaust > 0)
        {
            myCreatures.OrderBy(x => !x.myStats.exhausted).ThenBy(x => UnityEngine.Random.value).Take(attackInfo.NumToExhaust).ToList().ForEach(x => x.SetExhaust(true));
            CombatManager.Instance.exhaustedCreatures = attackInfo.NumToExhaust;
        }
        if (attackInfo.NumToReady > 0)
        {
            myCreatures.OrderBy(x => x.myStats.exhausted).ThenBy(x => UnityEngine.Random.value).Take(attackInfo.NumToReady).ToList().ForEach(x => x.SetExhaust(false));
            CombatManager.Instance.restoredCreatures = attackInfo.NumToReady;
        }

        blockPercent = 0;
        if (myCreatures.Count == 0)
            CombatManager.Instance.Defeat();
    }

    public void ReceivePlayerEffect(SpecialSkillInfo ssi)
    {
        //Exhaust creatures required to perform the effect
        if (ssi.requiredAmount > 0)
        {
            GetRandomAmountOfType(ssi.requiredAmount, ssi.requiredType, false, true).ForEach(x => x.SetExhaust(true));
            CombatManager.Instance.exhaustedCreatures += ssi.requiredAmount;
        }

        //Play any animation on self
        if (ssi.OnSelfAnimation != null)
        {
            GameObject go = Instantiate(ssi.OnSelfAnimation);
            go.transform.parent = transform;
            go.transform.SetLocalPosition(0, 0, -10);
            go.GetComponent<OneTimeAnimation>()?.SetCompleteAction(() => { CombatManager.Instance.PlayerTurnDoneAnimating(); });
        }
        else
            CombatManager.Instance.PlayerTurnDoneAnimating();

        //Find any effect that targets you and perform it.
        foreach (MainEffect main in ssi.effects.Where(x => x.target == CombatTarget.Self))
        {
            blockPercent += main.block;
            myCreatures.ForEach(x => x.TakeDamage(main.damage - main.heal));
        }

        //Find any effects that target your creatures and perform them
        foreach (CreatureEffect ce in ssi.effectsOnCreatures)
        {
            GetRandomAmountOfType(ce.quantity, ce.type, ce.ready, ce.exhaust).ForEach(x => x.ReceiveCreatureEffect(ce));
        }

        ultimate += ssi.ultimateMeterChange;

        if (myCreatures.Count == 0)
            CombatManager.Instance.Defeat();
    }

    public float BaseDamageMod()
    {
        return myCreatures.Sum(x => x.CalculateDamagePercent()) / myCreatures.Count();
    }

    public List<CombatCreature> GetRandomAmountOfType(int amount, CreatureType type, bool onlyExhausted = false, bool onlyNotExhausted = true)
    {
        List<CombatCreature> toReturn = myCreatures;
        if (onlyExhausted)
            toReturn = toReturn.Where(x => x.myStats.exhausted).ToList();
        else if(onlyNotExhausted)
            toReturn = toReturn.Where(x => !x.myStats.exhausted).ToList();

        if (type != CreatureType.All)
            toReturn = myCreatures.Where(x => x.myStats.myType == type).ToList();

        if (amount < toReturn.Count)
        {
            toReturn = toReturn.OrderBy(x => UnityEngine.Random.value).Take(amount).ToList();
        }

        return toReturn;
    }

    public bool HasRequiredCreatures(int amount, CreatureType ct)
    {
        if (ct == CreatureType.All)
            return myCreatures.Where(x => !x.myStats.exhausted).Count() >= amount;
        else
            return myCreatures.Where(x => !x.myStats.exhausted && x.myStats.myType == ct).Count() >= amount;
    }

    public void PreviewCreatures(int amount, CreatureType ct)
    {
        if (ct == CreatureType.All)
            preppedCreatures = myCreatures.Where(x => !x.myStats.exhausted).Take(amount).ToList();
        else
            preppedCreatures = myCreatures.Where(x => !x.myStats.exhausted && x.myStats.myType == ct).Take(amount).ToList();

        preppedCreatures.ForEach(x => x.SetPreview());
    }

    public void UnPreview()
    {
        if (preppedCreatures != null && preppedCreatures.Count > 0)
        {
            preppedCreatures.ForEach(x => x.UnPreview());
            preppedCreatures.Clear();
        }
    }


    public void Block()
    {
        blockPercent += 50;
        myCreatures.Where(x => x.myStats.exhausted).OrderBy(x => UnityEngine.Random.value).Take(10).ToList().ForEach(x => x.SetExhaust(false));
    }

    public void PlayAttackAnimation()
    {
        StartCoroutine(StopParticlesWithFF());
        foreach (CombatCreature cc in myCreatures)
        {
            cc.GetComponentInChildren<ParticleSystem>().Play();
            activeParticles = myCreatures.Count();
            cc.GetComponentInChildren<ParticleTriggerHandler>().SetInfoAndPlay(cc.myStats.myType, 1, cc.myColor != null ? cc.myColor : Color.red, new Color(), CombatEnemy.Instance.ParticleAbsorber,
                                                                        () => 
                                                                        {
                                                                            int dmg = CalculateUltimateDamage();
                                                                            CombatEnemy.Instance.TakeDamage(dmg); 
                                                                            CombatManager.Instance.lastUltimateDamage = dmg; 
                                                                        }, () => { ParticleCollided(); });
        }
        myCreatures.Where(x => x.myStats.exhausted).OrderBy(x => UnityEngine.Random.value).Take(5).ToList().ForEach(x => x.SetExhaust(false));

    }

    public void ParticleCollided()
    {
        activeParticles -= 1;
        if (activeParticles == 0)
        {
            CombatManager.Instance.DisplayUltimateResultMessage();
        }
    }

    private IEnumerator StopParticlesWithFF()
    {
        psff.drag = new ParticleSystem.MinMaxCurve() { constant = 1400 };
        yield return new WaitForSeconds(0.3f);
        psff.drag = new ParticleSystem.MinMaxCurve() { constant = 0 };
        CombatEnemy.Instance.psff.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        CombatEnemy.Instance.psff.gameObject.SetActive(false);
        yield break;
    }
}
