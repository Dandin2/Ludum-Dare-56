using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatCreature : MonoBehaviour
{
    [HideInInspector]
    public ActiveCreatureStats myStats;


    public Color myColor;
    public GameObject Preview;
    public GameObject Exhaust;

    //Delete this when we get actual sprites
    public void SetType(CreatureType type)
    {
        myStats.myType = type;
        myStats.damage = 7;
        myStats.health = 10;
        if (GetComponent<SpriteRenderer>() != null)
        {
            if (type == CreatureType.Air)
                GetComponent<SpriteRenderer>().color = Color.cyan;
            if (type == CreatureType.Airship)
                GetComponent<SpriteRenderer>().color = Color.gray;
            if (type == CreatureType.Fire)
                GetComponent<SpriteRenderer>().color = Color.red;
            if (type == CreatureType.Chef)
                GetComponent<SpriteRenderer>().color = Color.green;
            if (type == CreatureType.Water)
                GetComponent<SpriteRenderer>().color = Color.blue;

            myColor = GetComponent<SpriteRenderer>().color;
            //Just in case we want to have exhaustion pass from the other mode.
            if (myStats.exhausted)
            {
                myStats.exhausted = false;
                SetExhaust(myStats.exhausted);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        myStats.health -= damage;

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
    public int health;
    public int damage;
    public int block;
    public int hunger;
    public int hygene;
    public int entertainment;
    public CreatureType myType;
    public bool exhausted;
}
