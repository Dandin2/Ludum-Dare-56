using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CombatButton : MonoBehaviour
{
    public Sprite Unlit;
    public Sprite Highlight;
    public UIClick Click;
    public GameObject HighlightGO;

    public SpecialSkillInfo SkillOnClick;

    public bool active = true;
    public bool? manualActive = null;
    private GameObject inactive;

    private void Awake()
    {
        //Set defaults
        if (SkillOnClick != null && Click.clickAction == null)
        {
            Click.SetClickAction(() => { if (active) { CombatManager.Instance.PerformPlayerAction(SkillOnClick); CombatPlayer.Instance.UnPreview(); } });
        }
        if (Click.hoverAction == null && Click.unHoverAction == null)
        {
            Click.SetHoverAction(() =>
            {
                if (SkillOnClick != null)
                    CombatManager.Instance.SetAbilityHoverText(SkillOnClick);
                if (active)
                    SetHighlight(true);
            });

            Click.SetUnhoverAction(() =>
            {
                if (SkillOnClick != null)
                {
                    CombatManager.Instance.TextDisplay.HideMessage();
                    CombatPlayer.Instance.UnPreview();
                }
                if (active)
                    SetHighlight(false);
            });
        }
    }

    private void OnEnable()
    {
        if (!manualActive.HasValue)
        {
            if (SkillOnClick != null && !CombatPlayer.Instance.HasRequiredCreatures(SkillOnClick.requiredAmount, SkillOnClick.requiredType))
                active = false;
            else
                active = true;
        }
        else
            active = manualActive.Value;

        if (!active)
        {
            if (inactive == null)
            {
                inactive = Instantiate(CombatManager.Instance.InactivePrefab);
                inactive.transform.SetParent(transform);
                inactive.transform.SetLocalPosition(0, 0, -1);
                inactive.transform.localScale = new Vector3(1, 1, 1);
            }
        }
        else if(inactive != null)
        {
            Destroy(inactive);
            inactive = null;
        }
    }


    public void Set(Action onClick, Action onHover, Action onUnHover)
    {
        Click.SetClickAction(() => { onClick?.Invoke(); GetComponentInChildren<Image>().sprite = Unlit; });
        Click.SetHoverAction(() => { SetHighlight(true); onHover?.Invoke(); });
        Click.SetUnhoverAction(() => { SetHighlight(false); onUnHover?.Invoke(); });
    }

    public void SetHighlight(bool highlight)
    {
        if (Highlight == null)
        {
            HighlightGO.SetActive(highlight);
        }
        else
        {
            if (highlight)
                GetComponentInChildren<Image>().sprite = Highlight;
            else
                GetComponentInChildren<Image>().sprite = Unlit;
        }
    }
}
