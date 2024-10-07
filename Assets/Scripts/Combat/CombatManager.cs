using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;
    public CombatSkillList SkillList;
    public List<CombatEnemyInfo> Enemies;
    public CombatTextDisplay TextDisplay;
    public Fade Fader;

    public GameObject InactivePrefab;

    private bool hasLoaded;
    private bool _isPlayerTurn;
    [HideInInspector]
    public bool isPlayerTurn
    {
        get
        {
            return _isPlayerTurn;
        }
        set
        {
            _isPlayerTurn = value;
            SkillList.SetActive(_isPlayerTurn);
        }
    }
    private int doneAnimatingCount;
    private bool combatOver = false;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            SkillList.SetOptions();
        }
        else
            Destroy(gameObject);
    }

    public void RegisterInstanceCreated()
    {
        if (!hasLoaded) hasLoaded = true;
        else CombatStart();
    }

    public void CombatStart()
    {
        isPlayerTurn = false;
        CombatEnemyInfo chosen = Enemies[WorldManager.instance.level];
        CombatEnemy.Instance.SetEnemy(chosen.myInfo);
        CombatPlayer.Instance.SetInitial();
        Fader.FadeToClear(2, () =>
        {
            TextDisplay.SetMessage($"A {chosen.myInfo.enemyName} appeared!", false, () =>
            {
                TextDisplay.SetMessage(chosen.myInfo.combatIntroText, false, () => { StartPlayerTurn(); }, 2);
            }, 3);
        });
    }

    private IEnumerator WaitThenDo(float seconds, Action onComplete)
    {
        yield return new WaitForSeconds(seconds);
        onComplete?.Invoke();
        yield break;
    }
    public void StartPlayerTurn()
    {
        if (!combatOver)
            isPlayerTurn = true;
    }

    public void StartEnemyTurn()
    {
        if (!combatOver)
        {
            isPlayerTurn = false;
            EnemyAttackInfo eai = CombatEnemy.Instance.ChooseSkillToUse();
            TextDisplay.SetMessage($"{CombatEnemy.Instance.myInfo.enemyName} uses {eai.SkillName}!", false, () =>
            {
                CombatEnemy.Instance.TakeTurn(eai);
                CombatPlayer.Instance.ReceiveEnemyEffect(eai); //if eai doesn't do anything to you, this function will ignore the effect
            }, 2);
        }
    }

    public void EnemyTurnDoneAnimating()
    {
        doneAnimatingCount++;
        if (doneAnimatingCount > 1)
        {
            doneAnimatingCount = 0;
            StartCoroutine(WaitThenDo(1, () => { StartPlayerTurn(); }));
        }
    }

    public void PlayerTurnDoneAnimating()
    {
        doneAnimatingCount++;
        if (doneAnimatingCount > 1)
        {
            doneAnimatingCount = 0;
            StartCoroutine(WaitThenDo(1, () => { StartEnemyTurn(); }));
        }
    }

    public void AttackEnemy()
    {
        isPlayerTurn = false;
        CombatPlayer.Instance.PlayAttackAnimation();
    }

    public void Block()
    {
        isPlayerTurn = false;
        CombatPlayer.Instance.Block();
    }

    public void PerformPlayerAction(SpecialSkillInfo ssi)
    {
        isPlayerTurn = false;
        doneAnimatingCount = 0;
        TextDisplay.SetMessage(ssi.TextOnUse, false, () =>
        {
            CombatPlayer.Instance.ReceivePlayerEffect(ssi);
            CombatEnemy.Instance.ReceiveEffect(ssi);
        }, 1);
    }

    public void SetAbilityHoverText(SpecialSkillInfo ssi)
    {
        if (CombatPlayer.Instance.HasRequiredCreatures(ssi.requiredAmount, ssi.requiredType))
        {
            TextDisplay.SetMessage(ssi.TextOnHover, true, null);
            CombatPlayer.Instance.PreviewCreatures(ssi.requiredAmount, ssi.requiredType);
        }
        else
            TextDisplay.SetMessage($"Need more {ssi.requiredType.ToString()} creatures!", true, null);
    }

    public void Ultimate()
    {
        SkillList.SetActive(false);

    }

    public void Victory()
    {
        combatOver = true;
        int goldGain = 500 + (500 * WorldManager.instance.level);
        TextDisplay.SetMessage("You win!", false, () =>
        {
            TextDisplay.SetMessage($"You gain {goldGain} gold!", false, () =>
            {
                WorldManager.instance.activeCreatureStats = CombatPlayer.Instance.GetCreatures();
                WorldManager.instance.GoldAmount += goldGain;
                WorldManager.instance.level++;
                SceneManager.LoadScene(2);
            }, 3);
        }, 4);
    }

    public void Defeat()
    {
        combatOver = true;
        TextDisplay.SetMessage("You lose.....", false, () =>
        {
            Fader.FadeToBlack(2, () => { SceneManager.LoadScene(0); });
        }, 4);
    }
}

[Serializable]
public class CombatEnemyInfo
{
    public EnemyType myEnemyType;
    public EnemyInfo myInfo;
}