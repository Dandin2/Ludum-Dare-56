using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image NoHealth;
    public Image CurrentHealth;
    public Image DamagedHealth;
    public Text Text;
    public GameObject BlockGO;
    public Text BlockText;

    [HideInInspector]
    public int curHP;
    private int maxHP;
    private int block;
    private Coroutine AnimateCoroutine = null;
    private Action animatingComplete;

    public void UpdateHealth(int max, int cur, int blk, Action onDoneAnimating)
    {
        animatingComplete = onDoneAnimating;
        maxHP = max;

        if (cur < curHP)
            curHP = cur;// Math.Max(0, cur);
        else if (cur > curHP)
            curHP = Math.Min(maxHP, cur);

        block = blk;
        BlockGO.SetActive(block > 0);
        BlockText.text = block.ToString();

        SetText();
        if (AnimateCoroutine != null)
        {
            StopAllCoroutines();
            AnimateCoroutine = StartCoroutine(Animate(true));
        }
        else
            AnimateCoroutine = StartCoroutine(Animate(false));
    }

    public void SetInitial(int current, int max, int blk)
    {
        curHP = current;
        maxHP = max;
        block = blk;
        CurrentHealth.transform.SetSize((float)Math.Max(0, curHP) / maxHP * ((NoHealth.transform as RectTransform).sizeDelta.x));
        DamagedHealth.transform.SetSize((float)Math.Max(0, curHP) / maxHP * ((NoHealth.transform as RectTransform).sizeDelta.x));
        SetText();
    }

    public void SetText()
    {
        Text.text = curHP + " / " + maxHP;
    }

    private IEnumerator Animate(bool alreadyAnimating)
    {
        CurrentHealth.transform.SetSize((float)Math.Max(0, curHP) / maxHP * ((NoHealth.transform as RectTransform).sizeDelta.x));
        float cur = (CurrentHealth.transform as RectTransform).sizeDelta.x;
        float dmg = (DamagedHealth.transform as RectTransform).sizeDelta.x;
        float diff = dmg - cur;
        if (dmg < cur)
        {
            DamagedHealth.transform.SetSize(cur);
        }
        else
        {
            if (!alreadyAnimating)
                yield return new WaitForSeconds(1);

            while (dmg > cur)
            {
                dmg -= diff * Time.deltaTime * 0.75f;
                DamagedHealth.transform.SetSize(dmg);
                yield return new WaitForEndOfFrame();
            }
            DamagedHealth.transform.SetSize(cur);
        }

        AnimateCoroutine = null;
        animatingComplete?.Invoke();
        yield break;
    }
}
