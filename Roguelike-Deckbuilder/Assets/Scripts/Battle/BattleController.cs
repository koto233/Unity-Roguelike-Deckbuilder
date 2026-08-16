using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using LitFramework;
using LitFramework.Config;
using LitFramework.EventBus;
using LitFramework.FSM;
using UnityEngine;

public class BattleController
{
    public BattleContext Context { get; private set; }
    public StateMachine BattleFSM { get; private set; }
    private Action _onBattleEnd;
    private List<int> _startDeck = new List<int>()
    {
        1,1,1,1,1,3,3,3,3,5
    };
    public BattleController(BattleContext context, Action onBattleEnd)
    {
        Context = context;
        _onBattleEnd = onBattleEnd;
    }

    public void StartBattle()
    {
        Context.IsPlayerTurn = true;
        Context.CurrentTurn = 1;
        var configService = ServiceLocator.Get<IConfigService>();
        var cardConfigTable = configService.GetTable<CardConfig>();

        foreach (var id in _startDeck)
        {
            var cardConfig = cardConfigTable.Get(id);
            var card = new Card(cardConfig);
            Context.Player.AddCardToDrawPile(card);
        }
        Context.Player.ShuffleDrawDeck();
        InitFsm();
        EventBus<DiedEvent>.Subscribe(OnCharacterDied);
        BattleFSM.ChangeState<PlayerTurnState>();
    }

    /// <summary>
    /// 初始化战斗状态机
    /// </summary> 
    private void InitFsm()
    {
        BattleFSM = new StateMachine();
        BattleFSM.RegisterState(new PlayerTurnState(this, BattleFSM));
        BattleFSM.RegisterState(new EnemyTurnState(this, BattleFSM));
        BattleFSM.RegisterState(new BattleEndState(this, BattleFSM));
    }
    public bool PlayCard(Card card, Enemy target = null)
    {
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
            Context.Player.RemoveCardFromHand(card);
            Context.Player.AddCardToDiscardPile(card);
            EventBus<HandChangedEvent>.Publish(new HandChangedEvent()
            {
                ChangedCards = new List<Card> { card },
                Cards = Context.Player.Hand,
                Type = ChangeType.Refresh
            });
            Debug.Log($"卡牌效果{card.EffectsInstance.Count} ");
            foreach (var effect in card.EffectsInstance)
            {
                if (effect == null)
                {
                    Debug.LogError("效果实例为空");
                    continue;
                }
                Debug.Log("执行效果 " + effect.GetType().Name);
                effect.Execute(card, Context);
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    public async UniTask<TurnResult> ExecuteEnemyTurnAsync()
    {
        foreach (var enemy in Context.Enemies)
        {
            if (enemy.CurrentHp <= 0) continue;

            // 1. 决策
            enemy.DetermineIntent(Context);

            // 2. 发布意图事件（UI更新）
            // EventBus.Publish(new EnemyIntentDeterminedEvent(enemy));

            // 3. 视觉延迟
            await UniTask.Delay(300);

            // 4. 执行
            enemy.ExecuteIntent(Context);

            // 5. 间隔
            await UniTask.Delay(500);
        }

        if (Context.Player.CurrentHp <= 0)
        {
            BattleFSM.ChangeState<BattleEndState>();
            return TurnResult.PlayerLose;
        }

        Context.Enemies.RemoveAll(e => e.CurrentHp <= 0);

        foreach (var enemy in Context.Enemies)
            enemy.OnTurnEnd();
        if (Context.Enemies.Count == 0)
        {
            return TurnResult.PlayerWin;
        }
        return TurnResult.Continue;
    }
    public void StartPlayerTurn()
    {
        Context.Player.DrawCardInTurnStart(BattleRules.InitialHandSize);
        Context.Player.ResetEnergy();

    }
    public void EndPlayerTurn()
    {
        Context.Player.DiscardAllHand();
        BattleFSM.ChangeState<EnemyTurnState>();
    }

    public void UpdateCardUsability()
    {
        foreach (var card in Context.Player.Hand)
        {
            card.CanUse = Context.Player.Energy >= card.CurrentCost;
        }
    }

    public IReadOnlyList<Card> GetPile(int pileType)
    {
        return pileType == 0 ? Context.Player.DrawPile : Context.Player.DiscardPile;
    }

    public void StartEnemyTurn()
    {

    }
    public void EndEnemyTurn()
    {
        BattleFSM.ChangeState<PlayerTurnState>();
    }

    private void OnCharacterDied(DiedEvent evt)
    {
        if (evt.EntityType == EntityType.Player)
        {
            EventBus<DiedEvent>.Unsubscribe(OnCharacterDied);
            BattleFSM.ChangeState<BattleEndState>();
            return;
        }
        if (evt.EntityType == EntityType.Enemy)
        {
            Context.Enemies.Remove(evt.Character as Enemy);
            if (Context.Enemies.Count == 0)
            {
                EventBus<DiedEvent>.Unsubscribe(OnCharacterDied);
                BattleFSM.ChangeState<BattleEndState>();
            }
        }
    }
    public void EndBattle()
    {
        _onBattleEnd?.Invoke();
    }
    public void OnAllEnemiesDefeated()
    {

    }

    // public void Tick(float deltaTime)
    // {
    //     // _battleFSM.Update();
    // }
}
