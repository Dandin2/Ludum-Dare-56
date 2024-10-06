using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSkillList : MonoBehaviour
{
    public GameObject BaseList;
    public CombatButton AttackButton;
    public CombatButton SpecialButton;
    public CombatButton BlockButton;
    public CombatButton UltimateButton;


    public GameObject SpecialList;
    public CombatButton Air;
    public CombatButton Fire;
    public CombatButton Water;
    public CombatButton Chef;
    public CombatButton Airship;
    public CombatButton Back;

    public GameObject AirList;
    


    public void SetOptions()
    {
        BaseList.gameObject.SetActive(true);
        SpecialList.gameObject.SetActive(false);

        AttackButton.Set(() => { CombatManager.Instance.AttackEnemy();  SetActive(false); CombatManager.Instance.TextDisplay.HideMessage(); },
                         () => { CombatManager.Instance.TextDisplay.SetMessage("A basic attack that also restores some creatures.", true, null); },
                         () => { CombatManager.Instance.TextDisplay.HideMessage(); });
        SpecialButton.Set(() => { CombatManager.Instance.PerformSpecialMove(); SetActive(false); CombatManager.Instance.TextDisplay.HideMessage(); },
                         () => { CombatManager.Instance.TextDisplay.SetMessage("Special moves using one type of creature.", true, null); },
                         () => { CombatManager.Instance.TextDisplay.HideMessage(); });
        BlockButton.Set(() => { CombatManager.Instance.Block(); SetActive(false); CombatManager.Instance.TextDisplay.HideMessage(); },
                         () => { CombatManager.Instance.TextDisplay.SetMessage("Reduce enemy attack damage and restore many creatures.", true, null); },
                         () => { CombatManager.Instance.TextDisplay.HideMessage(); });
        UltimateButton.Set(() => { CombatManager.Instance.Ultimate(); SetActive(false); CombatManager.Instance.TextDisplay.HideMessage(); },
                         () => { CombatManager.Instance.TextDisplay.SetMessage("Unleash all your built up energy!  One time per combat.", true, null); },
                         () => { CombatManager.Instance.TextDisplay.HideMessage(); });

        SetSpecialButtons();
    }

    public void SetSpecialButtons()
    {
        Air.Set(() => {  SetActive(false); CombatManager.Instance.TextDisplay.HideMessage(); },
                () => { CombatManager.Instance.TextDisplay.SetMessage("Air based abilities.  Require non-exhausted Air creatures to use.", true, null); },
                () => { CombatManager.Instance.TextDisplay.HideMessage(); });
        Fire.Set(() => { SetActive(false); CombatManager.Instance.TextDisplay.HideMessage(); },
                () => { CombatManager.Instance.TextDisplay.SetMessage("Fire based abilities.  Require non-exhausted Fire creatures to use.", true, null); },
                () => { CombatManager.Instance.TextDisplay.HideMessage(); });
        Water.Set(() => { SetActive(false); CombatManager.Instance.TextDisplay.HideMessage(); },
                () => { CombatManager.Instance.TextDisplay.SetMessage("Water based abilities.  Require non-exhausted Water creatures to use.", true, null); },
                () => { CombatManager.Instance.TextDisplay.HideMessage(); });
        Chef.Set(() => { SetActive(false); CombatManager.Instance.TextDisplay.HideMessage(); },
                () => { CombatManager.Instance.TextDisplay.SetMessage("Chef based abilities.  Require non-exhausted Chef creatures to use.", true, null); },
                () => { CombatManager.Instance.TextDisplay.HideMessage(); });
        Airship.Set(() => { SetActive(false); CombatManager.Instance.TextDisplay.HideMessage(); },
                () => { CombatManager.Instance.TextDisplay.SetMessage("Airship based abilities.  Require non-exhausted Airship creatures to use.", true, null); },
                () => { CombatManager.Instance.TextDisplay.HideMessage(); });
        Back.Set(() => { SetActive(false); CombatManager.Instance.TextDisplay.HideMessage(); },
                () => { CombatManager.Instance.TextDisplay.SetMessage("Go back to previous menu.", true, null); },
                () => { CombatManager.Instance.TextDisplay.HideMessage(); });
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
        ShowMenu(true, false, false);
    }

    public void ShowMenu(bool baseMenu, bool special, bool specialSpecific)
    {
        if (baseMenu)
        {
            BaseList.gameObject.SetActive(true);
            SpecialList.gameObject.SetActive(false);
        }
        else if(special)
        {
            BaseList.gameObject.SetActive(false);
            SpecialList.gameObject.SetActive(true);
        }
    }
}
