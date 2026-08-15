using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using LitFramework;
using LitFramework.Config;
using LitFramework.EventBus;
using LitFramework.ObjectPool;
using UnityEngine;

public class BattlePresenter : IDisposable
{
    private UIBattle _view;
    private BattleController _battleController;
    private Enemy _currentTargetEnemy;
    private Card _selectedCard;
    public BattlePresenter(UIBattle view, BattleController battleController)
    {
        _view = view;
        _battleController = battleController;
        var context = _battleController.Context;
        SubscribeEvents();
        RefreshAllHp();
        RefreshHand(context.Player.Hand, context.Player.Hand, ChangeType.Add);
        RefreshBlock(context.Player.Block, EntityType.Player);
        RefreshEnergy(context.Player.Energy, context.Player.MaxEnergy);
        RefreshDrawPileCount(context.Player.DrawPileCount);
        RefreshDiscardPileCount(context.Player.DiscardPileCount);
    }


    private void SubscribeEvents()
    {
        EventBus<HandChangedEvent>.Subscribe(OnHandChanged);
        EventBus<HpChangedEvent>.Subscribe(OnHpChanged);
        EventBus<EnergyChangedEvent>.Subscribe(OnEnergyChanged);
        EventBus<BlockChangedEvent>.Subscribe(OnBlockChanged);
        EventBus<PlayerMaxHpChangedEvent>.Subscribe(OnPlayerMaxHpChanged);
        EventBus<BuffAppliedEvent>.Subscribe(OnBuffApplied);
        EventBus<BuffRemovedEvent>.Subscribe(OnBuffRemoved);
        EventBus<BuffStacksChangedEvent>.Subscribe(OnBuffStacksChanged);
        EventBus<TooltipShowEvent>.Subscribe(OnHoverEvent);
        EventBus<IntentEvent>.Subscribe(OnEnemyIntentChanged);
        EventBus<FloatingTextEvent>.Subscribe(OnFloatingTextEvent);
        EventBus<DrawPileChangedEvent>.Subscribe(OnDrawPileChanged);
        EventBus<DiscardPileChangedEvent>.Subscribe(OnDiscardPileChanged);
        _view.OnOpenPile += OpenPile;
        _view.OnEndTurn += EndTurn;
        _view.OnCardDragStartRequested += OnDragStart;
        _view.OnCardDragRequested += OnDragCard;
        _view.OnCardDragEndRequested += OnDragEnd;
        _view.OnCardPlayRequested += OnCardPlay;

    }



    private void UnSubscribeEvents()
    {
        EventBus<HandChangedEvent>.Unsubscribe(OnHandChanged);
        EventBus<HpChangedEvent>.Unsubscribe(OnHpChanged);
        EventBus<EnergyChangedEvent>.Unsubscribe(OnEnergyChanged);
        EventBus<BlockChangedEvent>.Unsubscribe(OnBlockChanged);
        EventBus<PlayerMaxHpChangedEvent>.Unsubscribe(OnPlayerMaxHpChanged);
        EventBus<BuffAppliedEvent>.Unsubscribe(OnBuffApplied);
        EventBus<BuffRemovedEvent>.Unsubscribe(OnBuffRemoved);
        EventBus<BuffStacksChangedEvent>.Unsubscribe(OnBuffStacksChanged);
        EventBus<TooltipShowEvent>.Unsubscribe(OnHoverEvent);
        EventBus<IntentEvent>.Unsubscribe(OnEnemyIntentChanged);
        EventBus<FloatingTextEvent>.Unsubscribe(OnFloatingTextEvent);
        EventBus<DrawPileChangedEvent>.Unsubscribe(OnDrawPileChanged);
        EventBus<DiscardPileChangedEvent>.Unsubscribe(OnDiscardPileChanged);
        _view.OnOpenPile -= OpenPile;
        _view.OnEndTurn -= EndTurn;
        _view.OnCardDragStartRequested -= OnDragStart;
        _view.OnCardDragRequested -= OnDragCard;
        _view.OnCardDragEndRequested -= OnDragEnd;
        _view.OnCardPlayRequested -= OnCardPlay;
    }


    private void EndTurn()
    {
        _battleController.EndPlayerTurn();
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
        RefreshHand(evt.Cards, evt.ChangedCards, evt.Type);
    }
    private void OnHpChanged(HpChangedEvent evt)
    {
        RefreshHp(evt.NewHp, evt.MaxHp, evt.EntityType, evt.EntityId);
    }
    private void OnEnergyChanged(EnergyChangedEvent evt)
    {
        RefreshEnergy(evt.CurrentEnergy, evt.MaxEnergy);

    }
    private void OnDiscardPileChanged(DiscardPileChangedEvent @event)
    {
        RefreshDiscardPileCount(@event.CurrentCount);
    }

    private void OnDrawPileChanged(DrawPileChangedEvent @event)
    {
        RefreshDrawPileCount(@event.CurrentCount);
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
    private void RefreshHand(IReadOnlyList<Card> handCards, IReadOnlyList<Card> changedCards, ChangeType type)
    {
        _battleController.UpdateCardUsability();
        _view.RefreshHand(handCards, changedCards, type);
    }

    private void RefreshDrawPileCount(int count)
    {
        _view.RefreshDrawPileCount(count);
    }

    private void RefreshDiscardPileCount(int count)
    {
        _view.RefreshDiscardPileCount(count);
    }

    private void OnDragStart(Card card, Vector2 position)
    {
        // Debug.Log("OnDragStart:" + cardId);
        _selectedCard = card;
        if (card.NeedTarget)
        {
            _view.ShowArrow(position, position, Vector2.up * 150);
        }
    }
    private void OnDragCard(Enemy enemy, Vector2 position)
    {
        _currentTargetEnemy = enemy;
        _view.UpdateArrow(position);
        // Debug.Log("OnDragCard:" + enemy == null);
    }


    private void OnCardPlay(Card card)
    {
        if (_selectedCard != card)
        {
            Debug.LogError("拖拽的卡牌不一致");
            return;
        }
        if (!_battleController.PlayCard(_selectedCard, _currentTargetEnemy))
        {
            _view.ResetCard();
        }
    }
    private void OnDragEnd(Card card)
    {
        _view.HideArrow();
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
        var pile = _battleController.GetPile(index);
        _view.ClearCardsInPileUI();
        _view.SpawnCardInList(pile);
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

    private void OnFloatingTextEvent(FloatingTextEvent evt)
    {
        Vector3 position = _view.GetEntityPosition(evt.EntityType, evt.EntityId);
        _view.ShowFloatingText(evt.Text, position, evt.Color, evt.IsCritical).Forget();
    }

}
