using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LitFramework;
using LitFramework.Config;
using LitFramework.EventBus;
using LitFramework.FSM;
using UnityEngine;

public class BattleController
{
    public BattleContext Context { get; private set; }
    public StateMachine BattleFSM { get; private set; }
    private Action _onBattleEnd;   // 战斗结束回调（胜利/失败）

    public BattleController(BattleContext context, Action onBattleEnd)
    {
        Context = context;
        _onBattleEnd = onBattleEnd;
        // var mono = ServiceLocator.Get<MonoService>();
        // mono.AddUpdate(this);
        // mono.AddDestroyNotify(this);
    }

    public void StartBattle()
    {
        Context.IsPlayerTurn = true;
        Context.CurrentTurn = 1;
        var configService = ServiceLocator.Get<IConfigService>();
        var cardConfigTable = configService.GetTable<CardConfig>();

        for (int i = 0; i < 20; i++)
        {
            int randomId = UnityEngine.Random.Range(1, 11);
            var cardConfig = cardConfigTable.GetById(randomId) as CardConfig;
            var card = new Card(cardConfig);
            Context.Player.DrawPile.Add(card);
        }
        InitFsm();
        BattleFSM.ChangeState<PlayerTurnState>();
    }

    /// <summary>
    /// 初始化战斗状态机
    /// </summary> 
    private void InitFsm()
    {
        BattleFSM = new StateMachine();
        BattleFSM.RegisterState(new PlayerTurnState(this));
        BattleFSM.RegisterState(new EnemyTurnState(this));
        BattleFSM.RegisterState(new BattleEndState(this));
    }
    public bool PlayCard(int cardId, Enemy target = null)
    {
        Debug.Log("使用卡牌 " + cardId);
        var card = Context.Player.Hand.FirstOrDefault(c => c.Config.Id == cardId);

        Debug.Log("使用卡牌 " + card.Config.Name);
        if (Context.IsPlayerTurn)
        {
            if (card.NeedTarget && target == null)
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

            foreach (var effect in card.EffectsInstance)
            {
                // Debug.Log("执行效果 " + effect.GetType().Name);
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



    // public void StartPlayerTurn()
    // {
      

    // }
    // public void EndPlayerTurn()
    // {
    //     Context.Player.DiscardAllHand();
    // }

    // public void StartEnemyTurn()
    // {

    // }
    // public void EndEnemyTurn()
    // {
    //     BattleFSM.ChangeState<PlayerTurnState>();
    // }


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

    // public void Tick(float deltaTime)
    // {
    //     // _battleFSM.Update();
    // }
}
