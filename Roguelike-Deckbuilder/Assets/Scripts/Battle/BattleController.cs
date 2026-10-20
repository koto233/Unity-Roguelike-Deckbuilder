using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using LitFramework;
using LitFramework.Config;
using LitFramework.EventBus;
using LitFramework.FSM;
using LitFramework.UI.Core.Service;
using UnityEngine;

public class BattleController
{
    private NotificationService _noticeService;
    public BattleContext Context { get; private set; }
    public StateMachine BattleFSM { get; private set; }
    /// <summary>
    /// 创建战斗数据（Context、牌库、事件订阅）。
    /// 此时不启动状态机，因为 UI 还没创建，提前决定敌人意图会丢失 IntentEvent。
    /// </summary>
    public void CreateBattle(BattleStartParams args)
    {
        Context = BattleContextFactory.Create(args);
        Context.IsPlayerTurn = false;
        Context.CurrentTurn = 0;
        _noticeService = ServiceLocator.Get<NotificationService>();
        var configService = ServiceLocator.Get<IConfigService>();
        var cardConfigTable = configService.GetTable<CardConfig>();
        var deck = ServiceLocator.Get<PlayerDataService>().DeckCardIds;
        foreach (var id in deck)
        {
            var cardConfig = cardConfigTable.Get(id);
            var card = new Card(cardConfig);
            Context.Player.AddCardToDrawPile(card);
        }

        Context.Player.ShuffleDrawDeck();
        EventBus<DiedEvent>.Subscribe(OnCharacterDied);
    }

    /// <summary>
    /// UI（BattleView、EnemyItem 及其资源）全部就绪后调用，启动状态机进入第一个玩家回合。
    /// 此时 StartPlayerTurn 中的 EnemiesDetermineAction 才能把意图正确推送到界面。
    /// </summary>
    public void BeginBattle()
    {
        Context.IsPlayerTurn = true;
        // Context 和战斗 UI 都已就绪，此时通知遗物等系统应用战斗级修正
        EventBus<BattleStartedEvent>.Publish(new BattleStartedEvent { Context = Context });
        InitFsm();
        BattleFSM.ChangeState<PlayerTurnState>();
    }
    public void EndBattle()
    {
        EventBus<DiedEvent>.Unsubscribe(OnCharacterDied);
        Context = null;
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
            Context.Attacker = Context.Player;
            Context.Target = target;
            if (!Context.Player.SpendEnergy(card.Config.Cost))
            {
                Debug.LogError("能量不足");
                _noticeService.ShowToast("能量不足").Forget();
                return false;
            }
            Context.Player.RemoveCardFromHand(card);
            if (!card.Config.HasTrait(CardTrait.Exhaust))
            {
                Context.Player.AddCardToDiscardPile(card);
            }
            EventBus<CardEvent>.Publish(new CardEvent() { Trait = card.Config.Traits });
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

    public void EnemiesDetermineAction()
    {
        foreach (var enemy in Context.Enemies)
        {
            enemy.DetermineAction();
        }
    }
    public async UniTask<TurnResult> ExecuteEnemyTurnAsync()
    {
        // 敌人回合开始：清格挡、结算回合开始效果
        foreach (var enemy in Context.Enemies.ToList())
        {
            enemy.OnTurnStart();
        }

        foreach (var enemy in Context.Enemies.ToList())
        {
            if (enemy.CurrentHp <= 0) continue;
            // 3. 视觉延迟
            await UniTask.Delay(300);

            // 4. 执行
            enemy.ExecuteIntent();

            // 5. 间隔
            await UniTask.Delay(500);
        }

        if (Context.Player.CurrentHp <= 0)
        {
            ServiceLocator.Get<UIService>().OpenAsync<GameOverView>().Forget();
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
        // 玩家回合开始：清格挡、结算待执行效果和 Buff 回合开始逻辑
        Context.Player.OnTurnStart();
        Context.Player.DrawCardInTurnStart(BattleRules.InitialHandSize);
        Context.Player.ResetEnergy();
        EnemiesDetermineAction();
    }
    public void EndPlayerTurn()
    {
        // UI 加载期间状态机还没启动，忽略结束回合请求
        if (BattleFSM == null)
        {
            return;
        }
        if (BattleFSM.CurrentState is not PlayerTurnState)
        {
            Debug.LogError("当前状态不是玩家回合，无法结束玩家回合");
            return;
        }
        Context.Player.OnTurnEnd();
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
        foreach (var enemy in Context.Enemies)
        {
            enemy.OnTurnEnd();
        }
        BattleFSM.ChangeState<PlayerTurnState>();
    }

    private void OnCharacterDied(DiedEvent evt)
    {
        if (evt.EntityType == EntityType.Player)
        {
            EventBus<DiedEvent>.Unsubscribe(OnCharacterDied);
            ServiceLocator.Get<UIService>().OpenAsync<GameOverView>().Forget();
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
    public Enemy GetEnemy(int enemyId)
    {
        return Context.Enemies.FirstOrDefault(e => e.InstanceId == enemyId);
    }
    public void OnAllEnemiesDefeated()
    {

    }

}
