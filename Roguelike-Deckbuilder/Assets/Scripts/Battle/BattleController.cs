using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : IBattleController
{
    public BattleContext Context { get; private set; }

    private Action _onBattleEnd;   // 战斗结束回调（胜利/失败）

    public BattleController(BattleContext context, Action onBattleEnd)
    {
        Context = context;
        _onBattleEnd = onBattleEnd;
        StartBattle();
    }

    private void StartBattle()
    {
        Context.IsPlayerTurn = true;
        Context.CurrentTurn = 1;
    }

    public void PlayCard(Card card, CharacterData target = null)
    {
    }

    public void EndTurn()
    {

    }

    private void StartEnemyTurn()
    {

    }

    private void ExecuteNextEnemyAction(int index)
    {

    }

    private void EndEnemyTurn()
    {

    }

    private void StartNewPlayerTurn()
    {

    }

    private void RemoveDeadEnemies()
    {
        Context.Enemies.RemoveAll(e => e.CurrentHp <= 0);
        if (Context.Enemies.Count == 0)
            OnAllEnemiesDefeated();
    }

    public void OnPlayerDeath()
    {
        _onBattleEnd?.Invoke();  // 失败回调，状态机切换到游戏结束
    }

    public void OnAllEnemiesDefeated()
    {

    }
}
