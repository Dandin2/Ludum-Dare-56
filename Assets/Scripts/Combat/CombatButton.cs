using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatButton : MonoBehaviour
{
    public Sprite Unlit;
    public Sprite Highlight;
    public UIClick Click;
    public GameObject HighlightGO;

    public void Set(Action onClick, Action onHover, Action onUnHover)
    {
        Click.SetClickAction(() => { onClick?.Invoke(); GetComponentInChildren<Image>().sprite = Unlit; });
        Click.SetHoverAction(() => { SetHighlight(true); onHover?.Invoke(); });
        Click.SetUnhoverAction(() => { SetHighlight(false); onUnHover?.Invoke(); });
    }

    public void SetHighlight(bool highlight)
    {
        if(Highlight == null)
        {
            HighlightGO.SetActive(highlight);
        }
        else
        {
            if(highlight)
                GetComponentInChildren<Image>().sprite = Highlight;
            else
                GetComponentInChildren<Image>().sprite = Unlit;
        }
    }
}
