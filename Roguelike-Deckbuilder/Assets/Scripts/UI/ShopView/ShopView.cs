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

    public void CreateCardList(List<CardDisplayData> cardDataList)
    {
        ClearCardList();

        foreach (var data in cardDataList)
        {
            var item = _poolService.Get<ShopCardItem>();
            item.Init(data);
            item.transform.SetParent(b_CardsRoot, false);
            int id = data.Id;
            item.OnClick += (id) => OnClickShopCard?.Invoke(id);
            _currentCardItems.Add(item);
        }
    }

    public void CreateRelicList(List<RelicDisplayData> relicDataList)
    {
        ClearRelicList();

        foreach (var data in relicDataList)
        {
            var item = _poolService.Get<ShopRelicItem>();
            item.Init(data);
            item.transform.SetParent(b_RelicsRoot, false);
            int id = data.Id;
            item.OnClick += (id) => OnClickRelic?.Invoke(id);
            _currentRelicItems.Add(item);
        }
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
            _poolService.ReturnGameObject<ShopCardItem>(item.gameObject);
        _currentCardItems.Clear();
    }

    private void ClearRelicList()
    {
        foreach (var item in _currentRelicItems)
            _poolService.ReturnGameObject<ShopRelicItem>(item.gameObject);
        _currentRelicItems.Clear();
    }

    private void ClearAllItems()
    {
        ClearCardList();
        ClearRelicList();
    }
}
