using System;
using System.Collections;
using System.Collections.Generic;
using LitFramework.EventBus;
using UnityEngine;

public class UIBattlePresenter
{
    private UIBattleWindow _view;
    private IEventBinding<HpChangedEvent> m_HpChangedEventBinding;
    private IEventBinding<HandChangedEvent> m_HandChangedEventBinding;
    private IEventBinding<EnergyChangedEvent> m_EnergyChangedEventBinding;
    private BattleController _battleController;
    private EnemyData _currentTargetEnemy;
    private string _selectedCardId;
    public UIBattlePresenter(UIBattleWindow view, BattleController battleController)
    {
        _view = view;
        _battleController = battleController;
        m_HpChangedEventBinding = new EventBinding<HpChangedEvent>(OnHpChanged);
        m_HandChangedEventBinding = new EventBinding<HandChangedEvent>(OnHandChanged);
        m_EnergyChangedEventBinding = new EventBinding<EnergyChangedEvent>(OnEnergyChanged);
        EventBus<HandChangedEvent>.Subscribe(m_HandChangedEventBinding);
        EventBus<HpChangedEvent>.Subscribe(m_HpChangedEventBinding);
        EventBus<EnergyChangedEvent>.Subscribe(m_EnergyChangedEventBinding);
        var context = _battleController.Context;
        RefreshHp(context.Player.CurrentHp, context.Player.MaxHp);
        RefreshHand(context.Player.Hand);
        RefreshEnergy(context.Player.Energy, context.Player.MaxEnergy);
    }

    private void OnHandChanged(HandChangedEvent evt)
    {
        RefreshHand(evt.Cards);
    }

    private void OnHpChanged(HpChangedEvent evt)
    {
        RefreshHp(evt.NewHp, evt.characterData.MaxHp);
    }
    private void OnEnergyChanged(EnergyChangedEvent evt)
    {
        RefreshEnergy(evt.CurrentEnergy, evt.MaxEnergy);
    }
    private void RefreshHp(int currentHp, int maxHp)
    {
        _view.RefreshHp(currentHp, maxHp);
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
        _battleController.PlayCard(_selectedCardId, _currentTargetEnemy);
    }
    private void OnCancelCard()
    {

    }

    private void RefreshEnergy(int energy, int maxEnergy)
    {
        _view.RefreshEnergy(energy, maxEnergy);
    }


}
