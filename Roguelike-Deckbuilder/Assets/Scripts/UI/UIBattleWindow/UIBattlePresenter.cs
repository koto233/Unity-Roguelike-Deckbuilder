using System;
using System.Collections;
using System.Collections.Generic;
using LitFramework.EventBus;
using UnityEngine;

public class UIBattlePresenter
{
    private UIBattleWindow _view;
    private IEventBinding<HpChangedEvent> _HpChangedEventBinding;
    private IEventBinding<HandChangedEvent> _HandChangedEventBinding;
    private IEventBinding<EnergyChangedEvent> _EnergyChangedEventBinding;
    private IEventBinding<BlockChangedEvent> _BlockChangedEventBinding;
    private IEventBinding<PlayerMaxHpChangedEvent> _PlayerMaxHpChangedEventBinding;

    private BattleController _battleController;
    private EnemyData _currentTargetEnemy;
    private string _selectedCardId;
    public UIBattlePresenter(UIBattleWindow view, BattleController battleController)
    {
        _view = view;
        _battleController = battleController;
        var context = _battleController.Context;
        SubscribeEvents();
        RefreshHp(context.Player.CurrentHp, context.Player.MaxHp, EntityType.Player);
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
        RefreshHp(evt.NewHp, evt.MaxHp, evt.EntityType);
    }
    private void OnEnergyChanged(EnergyChangedEvent evt)
    {
        RefreshEnergy(evt.CurrentEnergy, evt.MaxEnergy);
    }
    private void RefreshHp(int currentHp, int maxHp, EntityType entityType)
    {
        _view.RefreshHp(currentHp, maxHp, entityType);
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
            // Color rarityColor = GetRarityColor(card.Config.Rarity);
            string desc = string.Format(card.Config.Description, card.Config.Effects[0].Value);
            displayList.Add(new CardDisplayData
            {
                CardId = card.Config.Id,
                Name = card.Config.Name,
                Cost = card.CurrentCost,
                CostColor = costColor,
                Description = desc,
                // Icon = card.Config.Icon,
                // RarityColor = rarityColor,
                NeedTarget = card.Config.Effects[0].Target == "Enemy",
                IsPlayable = canPlay,
                IsHighlighted = false
            });
        }
        _view.RefreshHand(displayList, OnPlayCard, OnCancelCard, OnDragStart, OnDragCard);
    }


    private void OnDragStart(string cardId)
    {
        Debug.Log("OnDragStart:" + cardId);
        _selectedCardId = cardId;
    }
    private void OnDragCard(EnemyData enemy)
    {
        _currentTargetEnemy = enemy;
        Debug.Log("OnDragCard:" + enemy == null);
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


}
