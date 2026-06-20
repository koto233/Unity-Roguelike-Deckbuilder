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
    private BattleController _battleController;

    
    public UIBattlePresenter(UIBattleWindow view, BattleController battleController)
    {
        _view = view;
        _battleController = battleController;
        m_HpChangedEventBinding = new EventBinding<HpChangedEvent>(OnHpChanged);
        m_HandChangedEventBinding = new EventBinding<HandChangedEvent>(OnHandChanged);
        EventBus<HandChangedEvent>.Subscribe(m_HandChangedEventBinding);
        EventBus<HpChangedEvent>.Subscribe(m_HpChangedEventBinding);
        RefreshHp(_battleController.Context.Player.CurrentHp, _battleController.Context.Player.MaxHp);
        RefreshHand(_battleController.Context.Player.Hand);
    }

    private void OnHandChanged(HandChangedEvent evt)
    {
        RefreshHand(evt.Cards);
    }

    private void OnHpChanged(HpChangedEvent evt)
    {
        RefreshHp(evt.NewHp, evt.characterData.MaxHp);
    }
    private void RefreshHp(int currentHp, int maxHp)
    {
        _view.RefreshHp(currentHp, maxHp);
    }
    private void RefreshHand(List<Card> handCards)
    {
        _view.RefreshHand(handCards, OnPlayCard);
    }

    private void OnPlayCard(Card card)
    {

    }

}
