using System;
using System.Collections;
using System.Collections.Generic;
using LitFramework;
using LitFramework.Asset;
using LitFramework.EventBus;
using LitFramework.UI.Core.Window;
using UnityEngine;

public partial class UIHandZone : MonoBehaviour
{
    [SerializeField]
    private FanLayout _fanLayout;
    [SerializeField]
    private Transform handContainer;
    private GameObject _cardPrefab;
    private List<UICardItem> _cardItems = new();
    private BattleContext _battleContext;
    public void Init(GameObject cardPrefab)
    {
        _cardPrefab = cardPrefab;
    }
    // void Update()
    // {
    //     _fanLayout.Refresh();
    // }
    public void RefreshHand(List<CardDisplayData> handCards, System.Action<Card, CharacterData> onCardPlay)
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
            uiCard.Init(card);
            _cardItems.Add(uiCard);
        }
        _fanLayout.Refresh();

    }
}