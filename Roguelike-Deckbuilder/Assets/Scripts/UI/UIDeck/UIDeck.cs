using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitFramework;
using LitFramework.Asset;
using LitFramework.Config;
using LitFramework.ObjectPool;
using LitFramework.UI.Core.Window;
using UnityEngine;

public partial class UIDeck : UIWindow
{
    public event System.Action OnClickBack;
    private AssetRef<GameObject> _cardDisplayPrefab;
    private ObjectPoolService _poolService;
    private List<UICardDisplay> _cardDisplays = new();
    void OnEnable()
    {
        b_BackButton.onClick.AddListener(() => OnClickBack?.Invoke());
    }
    protected override async UniTask OnOpenAsync()
    {
        var assetService = ServiceLocator.Get<IAssetService>();
        _cardDisplayPrefab = await assetService.LoadRefAsync<GameObject>("Assets/Res/UI/Dynamic/UICardDisplay.prefab");
        _poolService = ServiceLocator.Get<ObjectPoolService>();

        InitObjectPools();
        await base.OnOpenAsync();

    }

    void OnDisable()
    {
        b_BackButton.onClick.RemoveAllListeners();
    }

    public void SpawnCardInList(IReadOnlyList<int> cards)
    {
        if (_cardDisplays.Count > 0)
        {
            ClearCardsInPileUI();
        }
        var configService = ServiceLocator.Get<IConfigService>();
        var cardConfigTable = configService.GetTable<CardConfig>();
        foreach (var id in cards)
        {
            var cardDisplayPrefab = _poolService.GetGameObject<UICardDisplay>();
            cardDisplayPrefab.transform.SetParent(b_Content.transform);
            var uiCard = cardDisplayPrefab.GetComponent<UICardDisplay>();
            var cardConfig = cardConfigTable.Get(id);
            var card = new Card(cardConfig);
            uiCard.Init(card);
            _cardDisplays.Add(uiCard);
        }
    }

    public void ClearCardsInPileUI()
    {
        foreach (var item in _cardDisplays)
        {
            item.gameObject.SetActive(false);
            _poolService.ReturnGameObject<UICardDisplay>(item.gameObject);
        }
        _cardDisplays.Clear();
    }
    private void InitObjectPools()
    {
        _poolService.RegisterGameObjectPool<UICardDisplay>(_cardDisplayPrefab.Asset, initialPoolSize: 10);
    }

}
