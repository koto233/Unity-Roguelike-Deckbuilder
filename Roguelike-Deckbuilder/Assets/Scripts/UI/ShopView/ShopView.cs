using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitFramework;
using LitFramework.Asset;
using LitFramework.ObjectPool;
using LitFramework.UI.Core.Window;
using UnityEngine;

public partial class ShopView : UIWindow
{
    private AssetRef<GameObject> _cardItemRef;
    private AssetRef<GameObject> _shopCardItemRef;
    private AssetRef<GameObject> _relicItemRef;
    public event Action<int> OnClickRelic;
    public event Action<int> OnClickShopCard;
    public event Action OnClickContinue;
    public event Action OnClickRemove;
    public event Action<int> OnClickCardToRemove;
    public event Action OnClickConfirm;
    private ObjectPoolService _poolService;

    private List<ShopCardItem> _shopCardItems = new();
    private List<ShopRelicItem> _relicItems = new();
    private List<ClickableCardItem> _cardItems = new();

    protected override async UniTask OnOpenAsync()
    {
        var assetService = ServiceLocator.Get<IAssetService>();
        _cardItemRef = await assetService.LoadRefAsync<GameObject>(UIPath.ClickableCardItem);
        _shopCardItemRef = await assetService.LoadRefAsync<GameObject>(UIPath.ShopCardItem);
        _relicItemRef = await assetService.LoadRefAsync<GameObject>(UIPath.ShopRelicItem);
        InitObjectPools();
        await base.OnOpenAsync();
    }
    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    private void InitObjectPools()
    {
        _poolService = ServiceLocator.Get<ObjectPoolService>();
        _poolService.RegisterGameObjectPool<ShopCardItem>(_shopCardItemRef.Asset, initialPoolSize: 10);
        _poolService.RegisterGameObjectPool<ShopRelicItem>(_relicItemRef.Asset, initialPoolSize: 10);
        _poolService.RegisterGameObjectPool<ClickableCardItem>(_cardItemRef.Asset, initialPoolSize: 10);

    }
    private void SubscribeEvents()
    {
        b_ContinueButton.onClick.AddListener(() => OnClickContinue?.Invoke());
        b_RemoveButton.onClick.AddListener(() => OnClickRemove?.Invoke());
        b_ConfirmButton.onClick.AddListener(() => OnClickConfirm?.Invoke());
    }

    private void UnsubscribeEvents()
    {
        b_ContinueButton.onClick.RemoveAllListeners();
        b_RemoveButton.onClick.RemoveAllListeners();
        b_ConfirmButton.onClick.RemoveAllListeners();
    }

    public void RefreshCards(List<CardDisplayData> cardDataList)
    {
        ClearCardList();

        foreach (var data in cardDataList)
        {
            var item = _poolService.GetGameObject<ShopCardItem>();
            var shopCardItem = item.GetComponent<ShopCardItem>();
            shopCardItem.Init(data);
            item.transform.SetParent(b_CardsRoot, false);
            shopCardItem.OnClick += (id) => OnClickShopCard?.Invoke(id);
            _shopCardItems.Add(shopCardItem);
        }
    }
    public void RefreshRemoveCost(int cost)
    {
        b_RemovePriceText.SetText(cost.ToString());
    }
    public void RefreshRelics(List<RelicDisplayData> relicDataList)
    {
        ClearRelicList();

        foreach (var data in relicDataList)
        {
            var item = _poolService.GetGameObject<ShopRelicItem>();
            var shopRelicItem = item.GetComponent<ShopRelicItem>();
            shopRelicItem.Init(data);
            item.transform.SetParent(b_RelicsRoot, false);
            shopRelicItem.OnClick += (id) => OnClickRelic?.Invoke(id);
            _relicItems.Add(shopRelicItem);
        }
    }
    public void RefreshRemovePanel(List<CardDisplayData> datas)
    {
        foreach (var data in datas)
        {
            var item = _poolService.GetGameObject<ClickableCardItem>();
            var clickableCardItem = item.GetComponent<ClickableCardItem>();
            clickableCardItem.Init(data);
            item.transform.SetParent(b_DeckRoot, false);
            clickableCardItem.OnClick += (id) => OnClickCardToRemove?.Invoke(id);
            _cardItems.Add(clickableCardItem);
        }
    }
    public void ShowRemovePanel(List<CardDisplayData> datas)
    {
        RefreshRemovePanel(datas);
        b_RemovePanel.gameObject.SetActive(true);
    }
    public void ShowConfirmPanel(CardDisplayData data)
    {
        var item = _poolService.GetGameObject<ClickableCardItem>();
        var clickableCardItem = item.GetComponent<ClickableCardItem>();
        clickableCardItem.Init(data);
        item.transform.SetParent(b_CardRoot, false);
        b_ConfirmPanel.gameObject.SetActive(true);
    }
    public void HideRemovePanel()
    {
        b_RemovePanel.gameObject.SetActive(false);
        b_ConfirmPanel.gameObject.SetActive(false);
    }
    private void ClearCardList()
    {
        foreach (var item in _shopCardItems)
            _poolService.ReturnGameObject<ShopCardItem>(item.gameObject);
        _shopCardItems.Clear();
    }

    private void ClearRelicList()
    {
        foreach (var item in _cardItems)
            _poolService.ReturnGameObject<ShopRelicItem>(item.gameObject);
        _cardItems.Clear();
    }

    private void ClearAllItems()
    {
        ClearCardList();
        ClearRelicList();
    }
}

