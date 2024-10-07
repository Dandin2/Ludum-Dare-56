using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
    public void FadeToClear(float time, Action endAction)
    {
        StartCoroutine(Fadee(true, time, endAction));
    }

    public void FadeToBlack(float time, Action endAction)
    {
        StartCoroutine(Fadee(false, time, endAction));
    }

    private IEnumerator Fadee(bool toClear, float seconds, Action onComplete)
    {
        float cur = 0;
        while (cur < seconds)
        {
            gameObject.SetColor(a: toClear ? 1 - (cur / seconds) : (cur / seconds));
            cur += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        gameObject.SetColor(a: toClear ? 0 : 1);
        onComplete?.Invoke();
        yield break;
    }

}
