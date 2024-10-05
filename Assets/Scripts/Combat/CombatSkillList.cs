using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSkillList : MonoBehaviour
{
    public UIClick AttackButton;
    public UIClick SpecialButton;
    public UIClick BlockButton;
    public UIClick UltimateButton;

    public void SetOptions()
    {
        AttackButton.SetClickAction(() => { CombatManager.Instance.AttackEnemy(); });
        SpecialButton.SetClickAction(() => { CombatManager.Instance.PerformSpecialMove(); });
        BlockButton.SetClickAction(() => { CombatManager.Instance.Block(); });
        UltimateButton.SetClickAction(() => { CombatManager.Instance.Ultimate(); });
    }
}
