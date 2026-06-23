using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LitFramework.EventBus;
using UnityEngine;

public class BattleController : IBattleController
{
    public BattleContext Context { get; private set; }

    private Action _onBattleEnd;   // 战斗结束回调（胜利/失败）

    public BattleController(BattleContext context, Action onBattleEnd)
    {
        Context = context;
        _onBattleEnd = onBattleEnd;
    }

    public void StartBattle()
    {
        Context.IsPlayerTurn = true;
        Context.CurrentTurn = 1;
        var cardLibrary = ModelContainer.Get<ICardLibrary>();
        for (int i = 0; i < 20; i++)
        {
            Context.Player.DrawPile.Add(cardLibrary.CreateRandomCard());
        }
        Context.Player.StartTurn();
        Context.Player.DrawCards(5);

    }

    public void PlayCard(string cardId)
    {
        var card = Context.Player.Hand.FirstOrDefault(c => c.Config.Id == cardId);
        Debug.Log("使用卡牌 " + card.Config.Name);
        if (Context.IsPlayerTurn)
        {
            // Context.Target = target;
            if (Context.Player.SpendEnergy(card.Config.Cost))
            {
                // 执行效果（传入目标）
                foreach (var effect in card.Effects)
                {
                    Debug.Log("执行效果 " + effect.GetType().Name);
                    effect.Execute(card, Context);
                }
            }
            Context.Player.Hand.Remove(card);
            Context.Player.DiscardPile.Add(card);
            EventBus<HandChangedEvent>.Publish(new HandChangedEvent() { Cards = Context.Player.Hand });
        }
        else
        {

        }
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
