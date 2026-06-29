using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LitFramework;
using LitFramework.Config;
using LitFramework.EventBus;
using UnityEngine;

public class UIBattlePresenter : IDisposable
{
    private UIBattleWindow _view;
    private IEventBinding<HpChangedEvent> _HpChangedEventBinding;
    private IEventBinding<HandChangedEvent> _HandChangedEventBinding;
    private IEventBinding<EnergyChangedEvent> _EnergyChangedEventBinding;
    private IEventBinding<BlockChangedEvent> _BlockChangedEventBinding;
    private IEventBinding<PlayerMaxHpChangedEvent> _PlayerMaxHpChangedEventBinding;
    private BattleController _battleController;
    private EnemyData _currentTargetEnemy;
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
        _HpChangedEventBinding = new EventBinding<HpChangedEvent>(OnHpChanged);
        _HandChangedEventBinding = new EventBinding<HandChangedEvent>(OnHandChanged);
        _EnergyChangedEventBinding = new EventBinding<EnergyChangedEvent>(OnEnergyChanged);
        _BlockChangedEventBinding = new EventBinding<BlockChangedEvent>(OnBlockChanged);
        _PlayerMaxHpChangedEventBinding = new EventBinding<PlayerMaxHpChangedEvent>(OnPlayerMaxHpChanged);
        EventBus<HandChangedEvent>.Subscribe(_HandChangedEventBinding);
        EventBus<HpChangedEvent>.Subscribe(_HpChangedEventBinding);
        EventBus<EnergyChangedEvent>.Subscribe(_EnergyChangedEventBinding);
        EventBus<BlockChangedEvent>.Subscribe(_BlockChangedEventBinding);
        EventBus<PlayerMaxHpChangedEvent>.Subscribe(_PlayerMaxHpChangedEventBinding);
        _view.OnOpenPile += OpenPile;
        _view.OnEndTurn += EndTurn;
    }


    private void UnSubscribeEvents()
    {
        EventBus<HandChangedEvent>.Unsubscribe(_HandChangedEventBinding);
        EventBus<HpChangedEvent>.Unsubscribe(_HpChangedEventBinding);
        EventBus<EnergyChangedEvent>.Unsubscribe(_EnergyChangedEventBinding);
        EventBus<BlockChangedEvent>.Unsubscribe(_BlockChangedEventBinding);
        EventBus<PlayerMaxHpChangedEvent>.Unsubscribe(_PlayerMaxHpChangedEventBinding);
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
                CanInteract = true,
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
    private void OnDragCard(EnemyData enemy)
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
                CanInteract = false,
                // Icon = card.Config.Icon,
                // RarityColor = GetRarityColor(card.Config.Rarity),
                NeedTarget = card.NeedTarget,
            };
            _view.SpawnCardInList(display);
        }
        _view.OpenPilePanel();
    }

    public void Dispose()
    {
        UnSubscribeEvents();
    }
}
