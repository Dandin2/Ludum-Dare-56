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
    public GameObject AirshipList;
    public GameObject ChefList;
    public GameObject FireList;
    public GameObject WaterList;


    public void SetOptions()
    {
        BaseList.gameObject.SetActive(true);
        SpecialList.gameObject.SetActive(false);

        //AttackButton.Set(() => { CombatManager.Instance.AttackEnemy();  SetActive(false); CombatManager.Instance.TextDisplay.HideMessage(); },
        //                 () => { CombatManager.Instance.TextDisplay.SetMessage("A basic attack that also restores some creatures.", true, null); },
        //                 () => { CombatManager.Instance.TextDisplay.HideMessage(); });
        SpecialButton.Set(() => { ShowMenu(false, true); CombatManager.Instance.TextDisplay.HideMessage(); },
                         () => { CombatManager.Instance.TextDisplay.SetMessage("Special moves using one type of creature.", true, null); },
                         () => { CombatManager.Instance.TextDisplay.HideMessage(); });
        //BlockButton.Set(() => { CombatManager.Instance.Block(); SetActive(false); CombatManager.Instance.TextDisplay.HideMessage(); },
        //                 () => { CombatManager.Instance.TextDisplay.SetMessage("Reduce enemy attack damage and restore many creatures.", true, null); },
        //                 () => { CombatManager.Instance.TextDisplay.HideMessage(); });
        UltimateButton.Set(() => { if (UltimateButton.active) { CombatManager.Instance.AttackEnemy(); UltimateButton.manualActive = false; SetActive(false); } },
                         () => { CombatManager.Instance.TextDisplay.SetMessage("Unleash your ultimate move! Stronger the more creatures you've restored.  One time per combat.", true, null); },
                         () => { CombatManager.Instance.TextDisplay.HideMessage(); });

        SetSpecialButtons();
    }

    public void SetSpecialButtons()
    {
        Air.Set(() => { ShowMenu(false, false, CreatureType.Air);  CombatManager.Instance.TextDisplay.HideMessage(); },
                () => { CombatManager.Instance.TextDisplay.SetMessage("Air based abilities.  Requires non-exhausted Air creatures.", true, null); },
                () => { CombatManager.Instance.TextDisplay.HideMessage(); });
        AirList.transform.Find("Ultimate Button").GetComponent<CombatButton>().Set(() => { ShowMenu(false, true); }, null, null);

        Fire.Set(() => { ShowMenu(false, false, CreatureType.Fire); CombatManager.Instance.TextDisplay.HideMessage(); },
                () => { CombatManager.Instance.TextDisplay.SetMessage("Fire based abilities.  Requirse non-exhausted Fire creatures.", true, null); },
                () => { CombatManager.Instance.TextDisplay.HideMessage(); });
        FireList.transform.Find("Ultimate Button").GetComponent<CombatButton>().Set(() => { ShowMenu(false, true); }, null, null);

        Water.Set(() => { ShowMenu(false, false, CreatureType.Water); CombatManager.Instance.TextDisplay.HideMessage(); },
                () => { CombatManager.Instance.TextDisplay.SetMessage("Water based abilities.  Requires non-exhausted Water creatures.", true, null); },
                () => { CombatManager.Instance.TextDisplay.HideMessage(); });
        WaterList.transform.Find("Ultimate Button").GetComponent<CombatButton>().Set(() => { ShowMenu(false, true); }, null, null);

        Chef.Set(() => { ShowMenu(false, false, CreatureType.Chef); CombatManager.Instance.TextDisplay.HideMessage(); },
                () => { CombatManager.Instance.TextDisplay.SetMessage("Chef based abilities.  Requires non-exhausted Chef creatures.", true, null); },
                () => { CombatManager.Instance.TextDisplay.HideMessage(); });
        ChefList.transform.Find("Ultimate Button").GetComponent<CombatButton>().Set(() => { ShowMenu(false, true); }, null, null);

        Airship.Set(() => { ShowMenu(false, false, CreatureType.Airship); CombatManager.Instance.TextDisplay.HideMessage(); },
                () => { CombatManager.Instance.TextDisplay.SetMessage("Airship based abilities.  Requires non-exhausted Airship creatures.", true, null); },
                () => { CombatManager.Instance.TextDisplay.HideMessage(); });
        AirshipList.transform.Find("Ultimate Button").GetComponent<CombatButton>().Set(() => { ShowMenu(false, true); }, null, null);

        Back.Set(() => { ShowMenu(true, false); CombatManager.Instance.TextDisplay.HideMessage(); },
                () => { CombatManager.Instance.TextDisplay.SetMessage("Go back to previous menu.", true, null); },
                () => { CombatManager.Instance.TextDisplay.HideMessage(); });
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
        ShowMenu(true, false);
    }

    public void ShowMenu(bool baseMenu, bool special, CreatureType specificMagicType = CreatureType.All)
    {
        if (baseMenu)
        {
            BaseList.gameObject.SetActive(true);
            SpecialList.gameObject.SetActive(false);
            SetMagicTypeListInactive(CreatureType.All);
        }
        else if(special)
        {
            BaseList.gameObject.SetActive(false);
            SpecialList.gameObject.SetActive(true);
            SetMagicTypeListInactive(CreatureType.All);
        }
        else if(specificMagicType != CreatureType.All)
        {
            BaseList.gameObject.SetActive(false);
            SpecialList.gameObject.SetActive(false);
            SetMagicTypeListInactive(specificMagicType);
        }
    }

    public void SetMagicTypeListInactive(CreatureType typeToKeepActive)
    {
        AirList.SetActive(typeToKeepActive == CreatureType.Air);
        AirshipList.SetActive(typeToKeepActive == CreatureType.Airship);
        ChefList.SetActive(typeToKeepActive == CreatureType.Chef);
        FireList.SetActive(typeToKeepActive == CreatureType.Fire);
        WaterList.SetActive(typeToKeepActive == CreatureType.Water);
    }
}
