using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LitFramework;
using LitFramework.Config;
using LitFramework.EventBus;
using UnityEngine;

public class UIBattlePresenter : IDisposable
{
    private UIBattleWindow _view;
    private IEventBinding<HpChangedEvent> _hpChangedEventBinding;
    private IEventBinding<HandChangedEvent> _handChangedEventBinding;
    private IEventBinding<EnergyChangedEvent> _energyChangedEventBinding;
    private IEventBinding<BlockChangedEvent> _blockChangedEventBinding;
    private IEventBinding<PlayerMaxHpChangedEvent> _playerMaxHpChangedEventBinding;
    private IEventBinding<BuffAppliedEvent> _buffAppliedEventBinding;
    private IEventBinding<BuffRemovedEvent> _buffRemovedEventBinding;
    private IEventBinding<BuffStacksChangedEvent> _buffStacksChangedEventBinding;
    private IEventBinding<TooltipShowEvent> _hoverEventBinding;
    private EventBinding<IntentEvent> _onIntentChanged;
    private BattleController _battleController;
    private Enemy _currentTargetEnemy;
    private int _selectedCardId;
    public UIBattlePresenter(UIBattleWindow view, BattleController battleController)
    {
        _view = view;
        _battleController = battleController;
        var context = _battleController.Context;
        SubscribeEvents();
        RefreshAllHp();
        RefreshHand(context.Player.Hand);
        RefreshEnergy(context.Player.Energy, context.Player.MaxEnergy);
        RefreshBlock(context.Player.Block, EntityType.Player);
    }


    private void SubscribeEvents()
    {
        _hpChangedEventBinding = new EventBinding<HpChangedEvent>(OnHpChanged);
        _handChangedEventBinding = new EventBinding<HandChangedEvent>(OnHandChanged);
        _energyChangedEventBinding = new EventBinding<EnergyChangedEvent>(OnEnergyChanged);
        _blockChangedEventBinding = new EventBinding<BlockChangedEvent>(OnBlockChanged);
        _playerMaxHpChangedEventBinding = new EventBinding<PlayerMaxHpChangedEvent>(OnPlayerMaxHpChanged);
        _buffAppliedEventBinding = new EventBinding<BuffAppliedEvent>(OnBuffApplied);
        _buffRemovedEventBinding = new EventBinding<BuffRemovedEvent>(OnBuffRemoved);
        _buffStacksChangedEventBinding = new EventBinding<BuffStacksChangedEvent>(OnBuffStacksChanged);
        _hoverEventBinding = new EventBinding<TooltipShowEvent>(OnHoverEvent);
        _onIntentChanged = new EventBinding<IntentEvent>(OnEnemyIntentChanged);
        EventBus<HandChangedEvent>.Subscribe(_handChangedEventBinding);
        EventBus<HpChangedEvent>.Subscribe(_hpChangedEventBinding);
        EventBus<EnergyChangedEvent>.Subscribe(_energyChangedEventBinding);
        EventBus<BlockChangedEvent>.Subscribe(_blockChangedEventBinding);
        EventBus<PlayerMaxHpChangedEvent>.Subscribe(_playerMaxHpChangedEventBinding);
        EventBus<BuffAppliedEvent>.Subscribe(_buffAppliedEventBinding);
        EventBus<BuffRemovedEvent>.Subscribe(_buffRemovedEventBinding);
        EventBus<BuffStacksChangedEvent>.Subscribe(_buffStacksChangedEventBinding);
        EventBus<TooltipShowEvent>.Subscribe(_hoverEventBinding);
        EventBus<IntentEvent>.Subscribe(_onIntentChanged);
        _view.OnOpenPile += OpenPile;
        _view.OnEndTurn += EndTurn;
    }



    private void UnSubscribeEvents()
    {
        EventBus<HandChangedEvent>.Unsubscribe(_handChangedEventBinding);
        EventBus<HpChangedEvent>.Unsubscribe(_hpChangedEventBinding);
        EventBus<EnergyChangedEvent>.Unsubscribe(_energyChangedEventBinding);
        EventBus<BlockChangedEvent>.Unsubscribe(_blockChangedEventBinding);
        EventBus<PlayerMaxHpChangedEvent>.Unsubscribe(_playerMaxHpChangedEventBinding);
        EventBus<BuffAppliedEvent>.Unsubscribe(_buffAppliedEventBinding);
        EventBus<BuffRemovedEvent>.Unsubscribe(_buffRemovedEventBinding);
        EventBus<BuffStacksChangedEvent>.Unsubscribe(_buffStacksChangedEventBinding);
        EventBus<TooltipShowEvent>.Unsubscribe(_hoverEventBinding);
        EventBus<IntentEvent>.Unsubscribe(_onIntentChanged);
        _view.OnOpenPile -= OpenPile;
        _view.OnEndTurn -= EndTurn;
    }
    private void EndTurn()
    {
        _battleController.BattleFSM.ChangeState<EnemyTurnState>();
    }


    private void OnPlayerMaxHpChanged(PlayerMaxHpChangedEvent evt)
    {

    }

    private void OnBlockChanged(BlockChangedEvent evt)
    {
        RefreshBlock(evt.NewBlock, evt.EntityType);
    }

    private void OnHandChanged(HandChangedEvent evt)
    {
        RefreshHand(evt.Cards);
    }
    private void OnHpChanged(HpChangedEvent evt)
    {
        RefreshHp(evt.NewHp, evt.MaxHp, evt.EntityType, evt.EntityId);
    }
    private void OnEnergyChanged(EnergyChangedEvent evt)
    {
        RefreshEnergy(evt.CurrentEnergy, evt.MaxEnergy);

    }

    private void RefreshAllHp()
    {
        var context = _battleController.Context;
        RefreshHp(context.Player.CurrentHp, context.Player.MaxHp, EntityType.Player, -1);
        foreach (var enemy in context.Enemies)
        {
            RefreshHp(enemy.CurrentHp, enemy.MaxHp, EntityType.Enemy, enemy.Id);
        }

    }
    private void RefreshHp(int currentHp, int maxHp, EntityType entityType, int entityId)
    {
        _view.RefreshHp(currentHp, maxHp, entityType, entityId);
    }

    private void RefreshBlock(int block, EntityType entityType)
    {
        _view.RefreshBlock(block, entityType);
    }
    private void RefreshHand(List<Card> handCards)
    {
        var displayList = new List<CardDisplayData>();
        foreach (var card in handCards)
        {
            // bool canPlay = _battleController.CanPlayCard(card);          // 外部判断
            bool canPlay = true;
            Color costColor = canPlay ? Color.white : Color.red;
            displayList.Add(new CardDisplayData
            {
                CardId = card.Config.Id,
                Name = card.Config.Name,
                Cost = card.CurrentCost,
                CostColor = costColor,
                Description = card.Description,
                CanUse = _battleController.Context.Player.Energy >= card.CurrentCost,
                // Icon = card.Config.Icon,
                // RarityColor = rarityColor,
                NeedTarget = card.NeedTarget,
                IsPlayable = canPlay,
                IsHighlighted = false
            });
        }
        _view.RefreshHand(displayList, OnPlayCard, OnCancelCard, OnDragStart, OnDragCard);
    }


    private void OnDragStart(int cardId)
    {
        // Debug.Log("OnDragStart:" + cardId);
        _selectedCardId = cardId;
    }
    private void OnDragCard(Enemy enemy)
    {
        _currentTargetEnemy = enemy;
        // Debug.Log("OnDragCard:" + enemy == null);
    }
    private void OnPlayCard()
    {
        if (!_battleController.PlayCard(_selectedCardId, _currentTargetEnemy))
        {
            _view.ResetCard();
        }
    }
    private void OnCancelCard()
    {

    }

    private void RefreshEnergy(int energy, int maxEnergy)
    {
        _view.RefreshEnergy(energy, maxEnergy);
    }

    /// <summary>
    /// 打开牌组 0 : 抽牌组 1 : 弃牌组 
    /// </summary>
    /// <param name="index"></param>
    private void OpenPile(int index)
    {
        List<Card> drawPile = null;
        switch (index)
        {
            case 0:
                drawPile = _battleController.Context.Player.DrawPile;
                break;
            case 1:
                drawPile = _battleController.Context.Player.DiscardPile;
                break;
            default:
                break;
        }
        if (drawPile == null) return;
        _view.ClearCardsInList();
        for (int i = 0; i < drawPile.Count; i++)
        {
            var card = drawPile[i];
            CardDisplayData display = new CardDisplayData
            {
                CardId = card.Config.Id,
                Name = card.Config.Name,
                Cost = card.CurrentCost,
                Description = card.Description,
                CanUse = _battleController.Context.Player.Energy >= card.CurrentCost,
                // Icon = card.Config.Icon,
                // RarityColor = GetRarityColor(card.Config.Rarity),
                NeedTarget = card.NeedTarget,
            };
            Debug.Log($"打开牌组: {display.Name} 消耗: {display.Cost} 当前能量: {_battleController.Context.Player.Energy} 可用: {display.CanUse}");
            _view.SpawnCardInList(display);
        }
        _view.OpenPilePanel();
    }
    private void OnBuffApplied(BuffAppliedEvent evt)
    {
        RefreshOwnerBuffs(evt.Owner);
    }

    private void OnBuffRemoved(BuffRemovedEvent evt)
    {
        RefreshOwnerBuffs(evt.Owner);
    }

    private void OnBuffStacksChanged(BuffStacksChangedEvent evt)
    {
        // 可以只更新单个 Slot，但简单起见全量刷新
        RefreshOwnerBuffs(evt.Owner);
    }

    private void RefreshOwnerBuffs(CharacterBase owner)
    {
        if (owner is Player)
        {
            // var playerView = _view.GetPlayerView();
            // playerView.RefreshBuffs(owner.BuffManager.AllBuffs.ToList());
        }
        else if (owner is Enemy enemy)
        {
            var enemyView = _view.GetEnemyView(enemy.Id);
            enemyView?.RefreshBuffs(owner.BuffManager.AllBuffs.ToList());
        }
    }
    private void OnHoverEvent(TooltipShowEvent evt)
    {
        if (evt.IsHovering)
        {
            switch (evt.Type)
            {
                case TooltipType.Buff:
                    {
                        _view.ShowBuffTooltip(evt.Data, evt.Position);
                        break;
                    }
                case TooltipType.Intent:
                    {
                        _view.ShowIntentToolTip(evt.Data, evt.Position);
                        break;
                    }
            }
        }
        else
        {
            _view.HideAllTooltips();
        }

    }
    private void OnEnemyIntentChanged(IntentEvent evt)
    {
        _view.RefreshEnemyIntent(evt.Enemy, evt.IntentConfig);
    }
    public void Dispose()
    {
        UnSubscribeEvents();
    }
}
