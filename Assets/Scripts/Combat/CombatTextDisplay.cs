using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatTextDisplay : MonoBehaviour
{
    public Text Text;
    public Action OnMessageShowComplete;

    public void SetMessage(string text, bool removeOnClick, Action endAction, float duration = 0)
    {
        Text.text = text;
        OnMessageShowComplete = endAction;
        gameObject.SetActive(true);
        StartCoroutine(WaitThenDisappear(2));
    }

    private IEnumerator WaitThenDisappear(float duration)
    {
        if (duration > 0)
        {
            yield return new WaitForSeconds(duration);
        }
        else
        {
            //wait for a click?
        }
        OnMessageShowComplete?.Invoke();
        //todo: maybe fade out
        gameObject.SetActive(false);
    }
}
