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


    public UIBattlePresenter(UIBattleWindow view, BattleController battleController)
    {
        _view = view;
        _battleController = battleController;
        m_HpChangedEventBinding = new EventBinding<HpChangedEvent>(OnHpChanged);
        m_HandChangedEventBinding = new EventBinding<HandChangedEvent>(OnHandChanged);
        m_EnergyChangedEventBinding = new EventBinding<EnergyChangedEvent>(OnEnergyChanged);
        EventBus<HandChangedEvent>.Subscribe(m_HandChangedEventBinding);
        EventBus<HpChangedEvent>.Subscribe(m_HpChangedEventBinding);
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
        RefreshEnergy(evt.NewEnergy, _battleController.Context.Player.MaxEnergy);
    }
    private void RefreshHp(int currentHp, int maxHp)
    {
        _view.RefreshHp(currentHp, maxHp);
    }
    private void RefreshHand(List<Card> handCards)
    {
        _view.RefreshHand(handCards, OnPlayCard);
    }
    private void RefreshEnergy(int energy, int maxEnergy)
    {
        _view.RefreshEnergy(energy, maxEnergy);
    }
    private void OnPlayCard(Card card, CharacterData target = null)
    {
        _battleController.PlayCard(card, target);
    }

}
