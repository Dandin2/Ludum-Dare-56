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
        StartCoroutine(WaitThenDisappear(duration));
    }

    public void HideMessage()
    {
        gameObject.SetActive(false);
    }

    private IEnumerator WaitThenDisappear(float duration)
    {
        if (duration > 0)
        {
            yield return new WaitForSeconds(duration);
        }
        else
        {
            //At the moment, just wait for a HideMessageCall
            yield return new WaitForSeconds(100000);
        }
        //todo: maybe fade out
        gameObject.SetActive(false);
        OnMessageShowComplete?.Invoke();
    }
}
