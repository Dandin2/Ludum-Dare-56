using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatCreature : MonoBehaviour
{
    [HideInInspector]
    public ActiveCreatureStats myStats;


    public Color myColor;
    public GameObject Preview;
    public GameObject Exhaust;

    private int damageMod;
    private int blockMod;

    //Delete this when we get actual sprites
    public void SetType(ActiveCreatureStats stats)
    {
        myStats = stats;

        if (myStats.myType == CreatureType.Air)
            gameObject.SetColor(0.52f, 0.87f, 0.72f);
        if (myStats.myType == CreatureType.Airship)
            gameObject.SetColor(0.72f, 0.52f, 0.87f);
        if (myStats.myType == CreatureType.Fire)
            gameObject.SetColor(.87f, .52f, .52f);
        if (myStats.myType == CreatureType.Chef)
            gameObject.SetColor(.87f, .78f, 0.52f);
        if (myStats.myType == CreatureType.Water)
            gameObject.SetColor(.52f, .67f, .87f);

        myColor = GetComponent<SpriteRenderer>() != null ? GetComponent<SpriteRenderer>().color : GetComponent<Image>().color;
        //Just in case we want to have exhaustion pass from the other mode.
        if (myStats.exhausted)
        {
            myStats.exhausted = false;
            SetExhaust(myStats.exhausted);
        }
    }

    public void TakeDamage(int damage)
    {
        myStats.health -= damage;
        myStats.health = Math.Min(myStats.health, myStats.maxHealth);

        if (GetComponent<SpriteRenderer>() != null)
        {
            if (damage > 0)
                StartCoroutine(FlashRed());
            if (damage < 0)
                StartCoroutine(FlashGreen());
        }
    }

    public void ReceiveCreatureEffect(CreatureEffect ce)
    {
        if (ce.heal > 0 || ce.takeDamage > 0)
            TakeDamage(ce.takeDamage - ce.heal);

        myStats.hunger += ce.hungerChange;
        myStats.hygene += ce.hygeneChange;
        myStats.entertainment += ce.happinessChange;

        damageMod += ce.damageMod;

        if (ce.ready)
        {
            if (myStats.exhausted)
            {
                CombatPlayer.Instance.ultimate = CombatPlayer.Instance.ultimate + 2;
                CombatManager.Instance.restoredCreatures++;
            }

            SetExhaust(false);
        }
        else if (ce.exhaust)
        {
            if (!myStats.exhausted)
                CombatManager.Instance.exhaustedCreatures++;

            SetExhaust(true);
        }
    }

    public float CalculateDamagePercent()
    {
        float dmgModPercent = (myStats.damage + damageMod) / myStats.damage;
        float dmg = dmgModPercent * (myStats.hunger < 50 ? 0.9f : 1) * (myStats.entertainment < 50 ? 0.9f : 1) * (myStats.hygene < 50 ? 0.9f : 1) * (myStats.exhausted ? 0.5f : 1);
        damageMod = 0;
        return dmg;
    }

    public void SetPreview()
    {
        Preview.SetActive(true);
    }
    public void UnPreview()
    {
        Preview.SetActive(false);
    }

    private IEnumerator FlashRed()
    {
        GetComponent<SpriteRenderer>().color = Color.white;
        yield return new WaitForSeconds(0.6f);
        if (myStats.health <= 0)
            Destroy(gameObject);
        else
            GetComponent<SpriteRenderer>().color = myColor;
        yield break;
    }
    private IEnumerator FlashGreen()
    {
        GetComponent<SpriteRenderer>().color = new Color(0, 1, 0);
        yield return new WaitForSeconds(0.6f);
        if (myStats.health <= 0)
            Destroy(gameObject);
        else
            GetComponent<SpriteRenderer>().color = myColor;
        yield break;
    }

    public void SetExhaust(bool isExhausted)
    {
        Exhaust.SetActive(isExhausted);
        if (isExhausted == myStats.exhausted)
            return;

        myStats.exhausted = isExhausted;
    }
}

[Serializable]
public class ActiveCreatureStats
{
    public string name;
    public int maxHealth;
    public int health;
    public int damage;
    public int block;
    public int hunger;
    public int hygene;
    public int entertainment;
    public CreatureType myType;
    public bool exhausted;

    public ActiveCreatureStats()
    {

    }
    public ActiveCreatureStats(CreatureStats c)
    {
        name = c.name;
        maxHealth = c.HitPoints;
        health = c.HitPoints;
        damage = c.Attack;
        block = c.Defence;
        hunger = c.Hunger;
        hygene = c.Hygiene;
        entertainment = c.Entertainment;
        myType = c.CreatureType;
        exhausted = false;
    }
}
