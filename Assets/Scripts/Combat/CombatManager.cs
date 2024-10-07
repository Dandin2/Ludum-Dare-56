using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;
    public CombatSkillList SkillList;
    public List<CombatEnemyInfo> Enemies;
    public CombatTextDisplay TextDisplay;

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
        CombatEnemy.Instance.SetEnemy(Enemies.Where(x => x.myEnemyType == EnemyType.Skull).FirstOrDefault().myInfo);
        CombatPlayer.Instance.SetInitial();

        StartPlayerTurn();
    }

    private IEnumerator WaitThenDo(float seconds, Action onComplete)
    {
        yield return new WaitForSeconds(seconds);
        onComplete?.Invoke();
        yield break;
    }
    public void StartPlayerTurn()
    {
        isPlayerTurn = true;
    }

    public void StartEnemyTurn()
    {
        isPlayerTurn = false;
        EnemyAttackInfo eai = CombatEnemy.Instance.ChooseSkillToUse();
        TextDisplay.SetMessage($"{CombatEnemy.Instance.myInfo.enemyName} uses {eai.SkillName}!", false, () =>
        {
            CombatEnemy.Instance.TakeTurn(eai);
            CombatPlayer.Instance.ReceiveEnemyEffect(eai); //if eai doesn't do anything to you, this function will ignore the effect
        }, 2);
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

    }

    public void PerformPlayerAction(SpecialSkillInfo ssi)
    {
        isPlayerTurn = false;
        doneAnimatingCount = 0;
        CombatPlayer.Instance.ReceivePlayerEffect(ssi);
        CombatEnemy.Instance.ReceiveEffect(ssi);
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
}

[Serializable]
public class CombatEnemyInfo
{
    public EnemyType myEnemyType;
    public EnemyInfo myInfo;
}