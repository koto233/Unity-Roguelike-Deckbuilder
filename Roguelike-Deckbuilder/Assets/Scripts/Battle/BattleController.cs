using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LitFramework;
using LitFramework.EventBus;
using LitFramework.FSM;
using UnityEngine;

public class BattleController : ITickable
{
    public BattleContext Context { get; private set; }
    private StateMachine _battleFSM;
    private Action _onBattleEnd;   // 战斗结束回调（胜利/失败）

    public BattleController(BattleContext context, Action onBattleEnd)
    {
        Context = context;
        _onBattleEnd = onBattleEnd;
        var mono = ServiceLocator.Get<MonoService>();
        mono.AddUpdate(this);
        // mono.AddDestroyNotify(this);
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

        InitFsm();
    }

    /// <summary>
    /// 初始化战斗状态机
    /// </summary> 
    private void InitFsm()
    {
        _battleFSM = new StateMachine();
        _battleFSM.RegisterState(new PlayerTurnState(_battleFSM));
        _battleFSM.RegisterState(new EnemyTurnState(_battleFSM));
        _battleFSM.RegisterState(new BattleEndState(_battleFSM));
    }
    public bool PlayCard(string cardId, EnemyData target = null)
    {
        Debug.Log("使用卡牌 " + cardId);
        var card = Context.Player.Hand.FirstOrDefault(c => c.Config.Id == cardId);

        Debug.Log("使用卡牌 " + card.Config.Name);
        if (Context.IsPlayerTurn)
        {
            if (card.Config.Effects[0].Target == "Enemy" && target == null)
            {
                Debug.LogError("请选择目标");
                return false;
            }
            Context.Target = target;
            if (!Context.Player.SpendEnergy(card.Config.Cost))
            {
                Debug.LogError("能量不足");
                return false;

            }
            foreach (var effect in card.Effects)
            {
                Debug.Log("执行效果 " + effect.GetType().Name);
                effect.Execute(card, Context);
            }
            Context.Player.Hand.Remove(card);
            Context.Player.DiscardPile.Add(card);
            EventBus<HandChangedEvent>.Publish(new HandChangedEvent() { Cards = Context.Player.Hand });
            return true;
        }
        else
        {
            return false;
        }
    }



    public void StartPlayerTurn()
    {
        Context.Player.StartTurn();
        Context.Player.DrawCards(5);
    }
    public void EndPlayerTurn()
    {

    }

    private void StartEnemyTurn()
    {

    }
    private void EndEnemyTurn()
    {

    }
    private void ExecuteNextEnemyAction(int index)
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

    public void Tick(float deltaTime)
    {
        _battleFSM.Update();
    }
}
