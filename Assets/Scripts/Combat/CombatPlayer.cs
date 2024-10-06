using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        for (int i = 0; i < 80; i++)
        {
            GameObject go = Instantiate(CreaturePrefab);
            CombatCreature cc = go.GetComponent<CombatCreature>();
            myCreatures.Add(cc);
            cc.SetType((CreatureType)UnityEngine.Random.Range(0, 5));
            cc.transform.parent = transform;
            cc.transform.position = new Vector3(minX + ((maxX - minX) * UnityEngine.Random.value),
                                                minY + ((maxY - minY) * UnityEngine.Random.value),
                                                transform.position.z - UnityEngine.Random.value);
            cc.GetComponent<ChaoticMover>().SetBounds(transform as RectTransform);
        }
    }

    public int CalculateDamage()
    {
        return myCreatures.Sum(x => x.myStats.damage) / myCreatures.Count;
    }

    public void ReceiveEnemyEffect(EnemyAttackInfo attackInfo)
    {
        if(attackInfo.OnEnemiesAnimation != null)
        {
            GameObject go = Instantiate(attackInfo.OnEnemiesAnimation);
            go.transform.parent = transform;
            go.transform.SetLocalPosition(0, 0, -10);
            go.GetComponent<OneTimeAnimation>()?.SetCompleteAction(() => { CombatManager.Instance.EnemyTurnDoneAnimating(); });
        }
        else
            CombatManager.Instance.EnemyTurnDoneAnimating();

        if(attackInfo.Damage > 0)
        {
            int numDamaging = 0;
            if (attackInfo.NumToDamage > 0)
                numDamaging = attackInfo.NumToDamage;
            else
                numDamaging = (int)(attackInfo.PercentToDamage * myCreatures.Count());

            myCreatures.OrderBy(x => UnityEngine.Random.value).Take(numDamaging).ToList().ForEach(x => 
            { 
                x.TakeDamage(attackInfo.Damage);
            });

            myCreatures = myCreatures.Where(x => x.myStats.health > 0).ToList();
        }

        if(attackInfo.NumToExhaust > 0)
        {
            myCreatures.OrderBy(x => x.myStats.exhausted).ThenBy(x => UnityEngine.Random.value).Take(attackInfo.NumToExhaust).ToList().ForEach(x => x.myStats.exhausted = true);
        }
        if (attackInfo.NumToReady > 0)
        {
            myCreatures.OrderBy(x => x.myStats.exhausted).ThenBy(x => UnityEngine.Random.value).Take(attackInfo.NumToExhaust).ToList().ForEach(x => x.myStats.exhausted = true);
        }
    }





    public void PlayAttackAnimation()
    {
        StartCoroutine(StopParticlesWithFF());
        foreach(CombatCreature cc in myCreatures)
        {
            cc.GetComponentInChildren<ParticleSystem>().Play();
            activeParticles = myCreatures.Count();
            cc.GetComponentInChildren<ParticleTriggerHandler>().SetInfoAndPlay(cc.myStats.myType, 1, cc.myColor != null ? cc.myColor : Color.red, new Color(), CombatEnemy.Instance.ParticleAbsorber, 
                                                                               () => { CombatEnemy.Instance.TakeDamage(CalculateDamage()); }, () => { ParticleCollided(); });
        }
    }

    public void ParticleCollided()
    {
        activeParticles -= 1;
        if(activeParticles == 0)
        {
            CombatManager.Instance.PlayerTurnDoneAnimating();
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
