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
    private AssetRef<GameObject> _relicItemRef;
    public event Action<int> OnClickRelic;
    public event Action<int> OnClickShopCard;
    public event Action OnClickContinue;
    public event Action OnClickRemove;
    public event Action<int> OnClickCardToRemove;
    public event Action OnClickConfirm;
    private ObjectPoolService _poolService;
    private List<ShopCardItem> _currentCardItems = new();
    private List<ShopRelicItem> _currentRelicItems = new();

    protected override async UniTask OnOpenAsync()
    {
        var assetService = ServiceLocator.Get<IAssetService>();
        _cardItemRef = await assetService.LoadRefAsync<GameObject>(UIPath.ShopCardItem);
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
        _poolService.RegisterGameObjectPool<ShopCardItem>(_cardItemRef.Asset, initialPoolSize: 10);
        _poolService.RegisterGameObjectPool<ShopRelicItem>(_relicItemRef.Asset, initialPoolSize: 10);

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
    public void CreateCardList()
    {
        ClearCardList();

        // foreach (var data in shopCards)
        // {
        //     var item = _poolService.Get<ShopCardItem>();
        //     item.Init(data.CardConfig);
        //     item.transform.SetParent(b_CardsRoot, false);

        //     // 点击事件转发（View 不管业务，只转发）
        //     int id = data.CardConfig.Id;
        //     item.OnClick += (id) => OnClickShopCard?.Invoke(id);

        //     _currentCardItems.Add(item);
        // }
        // 生成示范
        //   var card = _poolService.Get<ShopCardItem>();
        // card.transform.SetParent(b_CardsRoot, false);
    }
    public void CreateRelicList()
    {
        ClearRelicList();

        // foreach (var data in shopRelics)
        // {
        //     var item = _poolService.Get<ShopRelicItem>();
        //     item.Init(data.RelicConfig);
        //     item.SetPrice(data.Price);
        //     item.transform.SetParent(b_RelicItemsRoot, false);

        //     int id = data.RelicConfig.Id;
        //     item.OnClick += () => OnClickRelic?.Invoke(id);

        //     _currentRelicItems.Add(item);
        // }
        // 生成示范
        // var relic = _poolService.Get<ShopRelicItem>();
        // relic.transform.SetParent(b_RelicItemsRoot, false);
    }
    public void ShowRemovePanel()
    {
        b_RemovePanel.gameObject.SetActive(true);
    }
    public void ShowConfirmPanel()
    {
        b_ConfirmPanel.gameObject.SetActive(true);
    }
    private void ClearCardList()
    {
        foreach (var item in _currentCardItems)
            _poolService.RegisterGameObjectPool<ShopCardItem>(item.gameObject);
        _currentCardItems.Clear();
    }

    private void ClearRelicList()
    {
        foreach (var item in _currentRelicItems)
            _poolService.RegisterGameObjectPool<ShopRelicItem>(item.gameObject);
        _currentRelicItems.Clear();
    }

    private void ClearAllItems()
    {
        ClearCardList();
        ClearRelicList();
    }
}
