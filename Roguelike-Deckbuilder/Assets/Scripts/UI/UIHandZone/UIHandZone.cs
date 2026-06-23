using System;
using System.Collections;
using System.Collections.Generic;
using LitFramework;
using LitFramework.Asset;
using LitFramework.EventBus;
using LitFramework.ObjectPool;
using LitFramework.UI.Core.Window;
using UnityEngine;

public partial class UIHandZone : MonoBehaviour
{
    [SerializeField]
    private FanLayout _fanLayout;
    [SerializeField]
    private Transform handContainer;
    private List<UICardItem> _cardItems = new();
    private BattleContext _battleContext;
    private string _poolKey = "CardItem";
    private ObjectPoolService _poolService;
    public void Init(string poolKey, ObjectPoolService poolService)
    {
        _poolKey = poolKey;
        _poolService = poolService;
    }
    // void Update()
    // {
    //     _fanLayout.Refresh();
    // }
    public void RefreshHand(List<CardDisplayData> handCards, Action<string> onCardPlay)
    {
        Debug.Log("刷新手牌");
        // 清除现有
        foreach (var item in _cardItems)
        {
            item.gameObject.SetActive(false);
            _poolService.ReturnGameObject(_poolKey, item.gameObject);
        }
        _cardItems.Clear();

        // 重新生成
        foreach (var card in handCards)
        {
            var go = _poolService.GetGameObject(_poolKey);
            go.SetActive(true);
            go.transform.SetParent(handContainer);
            var uiCard = go.GetComponent<UICardItem>();
            uiCard.Init(card, onCardPlay);
            _cardItems.Add(uiCard);
        }
        _fanLayout.Refresh();

    }
}