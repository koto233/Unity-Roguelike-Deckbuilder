using System;
using System.Collections;
using System.Collections.Generic;
using LitFramework;
using LitFramework.Asset;
using LitFramework.EventBus;
using UnityEngine;

public class UIHandZone : MonoBehaviour
{
    public Transform handContainer;
    private GameObject _cardPrefab;
    private List<UICardItem> _cardItems = new();
    private BattleContext _battleContext;
    public void Init(GameObject cardPrefab)
    {
        _cardPrefab = cardPrefab;
    }

    public void RefreshHand(List<Card> handCards, Action<Card> onCardPlay)
    {
        // 清除现有
        foreach (var item in _cardItems)
            Destroy(item.gameObject);
        _cardItems.Clear();

        // 重新生成
        foreach (var card in handCards)
        {
            var go = Instantiate(_cardPrefab, handContainer);
            var uiCard = go.GetComponent<UICardItem>();
            uiCard.Refresh(card, onCardPlay);
            _cardItems.Add(uiCard);
        }
        // 自动布局（使用HorizontalLayoutGroup或GridLayoutGroup）
    }
}