using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitFramework;
using LitFramework.Asset;
using LitFramework.Config;
using LitFramework.ObjectPool;
using LitFramework.UI.Core.Window;
using UnityEngine;

public partial class DeckView : UIWindow
{
    public event System.Action OnClickBack;
    private AssetRef<GameObject> _cardItemPrefab;
    private ObjectPoolService _poolService;
    private List<CardItem> _cardDisplays = new();
    void OnEnable()
    {
        b_BackButton.onClick.AddListener(() => OnClickBack?.Invoke());
    }
    protected override async UniTask OnOpenAsync()
    {
        var assetService = ServiceLocator.Get<IAssetService>();
        _cardItemPrefab = await assetService.LoadRefAsync<GameObject>(UIPath.CardItem);
        _poolService = ServiceLocator.Get<ObjectPoolService>();

        InitObjectPools();
        await base.OnOpenAsync();

    }

    void OnDisable()
    {
        b_BackButton.onClick.RemoveAllListeners();
    }

    public void SpawnCardInList(IReadOnlyList<CardDisplayData> cards)
    {
        if (_cardDisplays.Count > 0)
        {
            ClearCardsInPileUI();
        }
        foreach (var card in cards)
        {
            var cardDisplayPrefab = _poolService.GetGameObject<CardItem>();
            cardDisplayPrefab.transform.SetParent(b_Content.transform);
            var uiCard = cardDisplayPrefab.GetComponent<CardItem>();
            uiCard.Init(card);
            _cardDisplays.Add(uiCard);
        }
    }

    public void ClearCardsInPileUI()
    {
        foreach (var item in _cardDisplays)
        {
            item.gameObject.SetActive(false);
            _poolService.ReturnGameObject<CardItem>(item.gameObject);
        }
        _cardDisplays.Clear();
    }
    private void InitObjectPools()
    {
        _poolService.RegisterGameObjectPool<CardItem>(_cardItemPrefab.Asset, initialPoolSize: 10);
    }

}
