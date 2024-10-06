using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSkillList : MonoBehaviour
{
    public CombatButton AttackButton;
    public CombatButton SpecialButton;
    public CombatButton BlockButton;
    public CombatButton UltimateButton;

    public void SetOptions()
    {
        AttackButton.Set(() => { CombatManager.Instance.AttackEnemy();  SetActive(false); });
        SpecialButton.Set(() => { CombatManager.Instance.PerformSpecialMove(); SetActive(false); });
        BlockButton.Set(() => { CombatManager.Instance.Block(); SetActive(false); });
        UltimateButton.Set(() => { CombatManager.Instance.Ultimate(); SetActive(false); });
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
