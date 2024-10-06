using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatButton : MonoBehaviour
{
    public GameObject Highlight;
    public UIClick Click;

    public void Set(Action onClick)
    {
        Click.SetClickAction(onClick);
        Click.SetHoverAction(() => { Highlight.SetActive(true); });
        Click.SetUnhoverAction(() => { Highlight.SetActive(false); });
    }
}
