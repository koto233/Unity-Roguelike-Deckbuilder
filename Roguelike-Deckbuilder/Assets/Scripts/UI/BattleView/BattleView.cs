using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using LitFramework;
using LitFramework.Asset;
using LitFramework.ObjectPool;
using LitFramework.UI.Core.Window;
using UnityEngine;

public partial class BattleView : UIWindow
{
    private AssetRef<GameObject> _cardPrefabRef;
    private AssetRef<GameObject> _cardItemPrefabRef;
    private AssetRef<GameObject> _flyPrefabRef;
    private AssetRef<GameObject> _floatingTextPrefabRef;
    private AssetRef<GameObject> _enemyPrefabRef;
    private AssetRef<GameObject> _playerPrefabRef;
    private PlayerItem _playerView;
    private Dictionary<int, EnemyItem> _enemyViews = new();
    private ObjectPoolService _poolService;
    public Transform DiscardPileTrans => b_DiscardPileBtn.transform;
    public Transform DrawPileTrans => b_DrawPileBtn.transform;
    public event Action<Card> OnCardPlayRequested;
    public event Action<Card, Vector2> OnCardDragStartRequested;
    public event Action<Card> OnCardDragEndRequested;
    public event Action<Enemy, Vector2> OnCardDragRequested;
    /// <summary>
    /// 打开卡组，0是抽牌堆，1是弃牌堆
    /// </summary>
    public event Action<int> OnOpenPile;
    public event Action OnEndTurn;
    private List<CardView> _cardDisplays = new();

    protected override async UniTask OnOpenAsync()
    {
        var assetService = ServiceLocator.Get<IAssetService>();

        _enemyPrefabRef = await assetService.LoadRefAsync<GameObject>(UIPath.EnemyItem);
        _playerPrefabRef = await assetService.LoadRefAsync<GameObject>(UIPath.PlayerItem);
        _cardPrefabRef = await assetService.LoadRefAsync<GameObject>(UIPath.HandCard);
        _cardItemPrefabRef = await assetService.LoadRefAsync<GameObject>(UIPath.CardItem);
        _flyPrefabRef = await assetService.LoadRefAsync<GameObject>(UIPath.CardFlyItem);
        _floatingTextPrefabRef = await assetService.LoadRefAsync<GameObject>(UIPath.FloatingTextItem);
        _poolService = ServiceLocator.Get<ObjectPoolService>();
        InitObjectPools();
        InitUI();
        SubscribeEvents();
    }

    private void InitObjectPools()
    {
        _poolService.RegisterGameObjectPool<HandCard>(_cardPrefabRef.Asset, initialPoolSize: 10);
        _poolService.RegisterGameObjectPool<CardView>(_cardItemPrefabRef.Asset, initialPoolSize: 10);
        _poolService.RegisterGameObjectPool<CardFlyFx>(_flyPrefabRef.Asset, initialPoolSize: 10);
        _poolService.RegisterGameObjectPool<FloatingTextItem>(_floatingTextPrefabRef.Asset, initialPoolSize: 10);
    }

    private void InitUI()
    {
        b_HandZone.Init(_poolService, this);
        b_ClosePileButton.onClick.AddListener(ClosePilePanel);
        ClosePilePanel();
        b_DrawPileBtn.onClick.AddListener(() => OnOpenPile(0));
        b_DiscardPileBtn.onClick.AddListener(() => OnOpenPile(1));
        b_EndTurnBtn.onClick.AddListener(() => OnEndTurn?.Invoke());
        b_BuffTooltip.Hide();
        b_IntentTooltip.Hide();
        CreatePlayerView();
        HideArrow();
    }

    private void SubscribeEvents()
    {
        b_HandZone.OnAnyCardPlay += (c) => OnCardPlayRequested?.Invoke(c);
        b_HandZone.OnAnyCardDragStart += (c, pos) => OnCardDragStartRequested?.Invoke(c, pos);
        b_HandZone.OnAnyCardDragEnd += (c) => OnCardDragEndRequested?.Invoke(c);
        b_HandZone.OnAnyCardDrag += (c, e) => OnCardDragRequested?.Invoke(c, e);
    }
    public void OpenPilePanel()
    {
        b_PilePanel.gameObject.SetActive(true);
        // 显示抽牌堆
    }
    private void ClosePilePanel()
    {
        b_PilePanel.gameObject.SetActive(false);
    }
    public void ClearCardsInPileUI()
    {
        foreach (var item in _cardDisplays)
        {
            item.gameObject.SetActive(false);
            _poolService.ReturnGameObject<CardView>(item.gameObject);
        }
        _cardDisplays.Clear();
    }
    public void SpawnCardInList(IReadOnlyList<Card> cards)
    {
        foreach (var data in cards)
        {
            var cardDisplayPrefab = _poolService.GetGameObject<CardView>();
            cardDisplayPrefab.transform.SetParent(b_PilePanel.transform);
            var uiCard = cardDisplayPrefab.GetComponent<CardView>();
            uiCard.Init(data);
            _cardDisplays.Add(uiCard);
        }
    }

    public void CreatePlayerView()
    {
        var go = Instantiate(_playerPrefabRef.Asset, b_PlayerRoot);
        _playerView = go.GetComponent<PlayerItem>();
    }

    public void CreateEnemyViews(List<Enemy> enemies)
    {
        foreach (var enemy in enemies)
        {
            var go = Instantiate(_enemyPrefabRef.Asset, b_EnemysRoot);
            var view = go.GetComponent<EnemyItem>();
            view.SetEnemy(enemy);
            _enemyViews[enemy.Id] = view;
        }
    }

    public EnemyItem GetEnemyView(int enemyId)
    {
        if (_enemyViews.TryGetValue(enemyId, out var view))
        {
            return view;
        }
        return null;
    }

    public void RefreshHp(int currentHp, int maxHp, EntityType entityType, int entityId)
    {
        if (entityType == EntityType.Player)
        {
            b_HPText.SetText(currentHp + "/" + maxHp);
            // b_HPSlider.value = (float)currentHp / maxHp;
            b_HPSlider.DOValue((float)currentHp / maxHp, 0.5f).SetEase(Ease.Linear);
            _playerView.UpdateHP(currentHp, maxHp);
        }
        else
        {
            if (_enemyViews.TryGetValue(entityId, out var enemyView))
            {
                enemyView.UpdateHP(currentHp, maxHp);
            }
        }

    }
    public void RefreshEnergy(int energy, int maxEnergy)
    {
        b_EnergyText.SetText($"{energy}/{maxEnergy}");
        b_HandZone.RefreshHandState(energy);
    }
    public void RefreshHand(IReadOnlyList<Card> hand, IReadOnlyList<Card> changedCards, ChangeType type)
    {
        b_HandZone.RefreshHand(hand, changedCards, type);
    }
    public void RefreshBlock(int block, EntityType entityType)
    {
        if (entityType != EntityType.Player)
        {
            return;
        }
        b_BlockText.SetText(block.ToString());
        b_BlockText.transform.parent.gameObject.SetActive(block > 0);
    }
    public void ResetCard()
    {
        b_HandZone.ResetCard();
    }
    public void RefreshDrawPileCount(int count)
    {
        b_DrawPileCount.SetText(count.ToString());
    }
    public void RefreshDiscardPileCount(int count)
    {
        b_DiscardPileCount.SetText(count.ToString());
    }


    // ===== View 层交互反馈 =====
    public void ShowArrow(Vector2 start, Vector2 end, Vector2 controlOffset)
    {
        b_TargetArrow.Show();
        b_TargetArrow.Init(start, end, controlOffset);
    }
    public void UpdateArrow(Vector2 end)
    {
        b_TargetArrow.UpdateArrow(end);
    }
    public void HideArrow()
    {
        b_TargetArrow.Hide();
    }
    public void ShowBuffTooltip(TooltipData data, Vector2 position)
    {
        Debug.Log($"ShowTooltip: {data} at {position}");
        b_BuffTooltip.Show(data, position);
    }
    public void ShowIntentToolTip(TooltipData data, Vector2 position)
    {
        b_IntentTooltip.Show(data, position);
    }
    public void HideAllTooltips()
    {
        b_BuffTooltip.Hide();
        b_IntentTooltip.Hide();
    }

    public void RefreshEnemyIntent(Enemy enemy, IntentConfig intentConfig)
    {
        if (_enemyViews.TryGetValue(enemy.Id, out var view))
        {
            view.RefreshIntent(intentConfig);
        }
    }

    public async UniTask ShowFloatingText(string text, Vector3 position, Color color, bool isCritical)
    {
        var item = _poolService.GetGameObject<FloatingTextItem>();
        item.transform.SetParent(transform);
        item.transform.position = position;
        var uiFloatingText = item.GetComponent<FloatingTextItem>();
        await uiFloatingText.PlayAsync(text, position, color, isCritical);
        if (item != null)
            _poolService.ReturnGameObject<FloatingTextItem>(item);
    }
    public Vector3 GetEntityPosition(EntityType entityType, int entityId)
    {
        if (entityType == EntityType.Player)
        {
            return _playerView.DamageTextPos;
        }
        else if (entityType == EntityType.Enemy)
        {
            if (_enemyViews.TryGetValue(entityId, out var enemyView))
                return enemyView.DamageTextPos;
        }
        return Vector3.zero;
    }
    void OnDisable()
    {
        b_BuffTooltip.Hide();
        b_IntentTooltip.Hide();
        b_HandZone.CancelAnimations();
        UnsubscribeEvents();
        ReleaseAssets();
    }

    private void UnsubscribeEvents()
    {
        b_ClosePileButton.onClick.RemoveAllListeners();
        b_DrawPileBtn.onClick.RemoveAllListeners();
        b_DiscardPileBtn.onClick.RemoveAllListeners();
        b_EndTurnBtn.onClick.RemoveAllListeners();
        OnOpenPile = null;
        OnEndTurn = null;
    }



    private void ReleaseAssets()
    {
        _poolService.RemovePool<HandCard>();
        _poolService.RemovePool<CardView>();
        _poolService.RemovePool<CardFlyFx>();
        _poolService.RemovePool<FloatingTextItem>();
        _cardPrefabRef?.Dispose();
        _cardItemPrefabRef?.Dispose();
        _flyPrefabRef?.Dispose();
        _floatingTextPrefabRef?.Dispose();
        _enemyPrefabRef?.Dispose();
        _playerPrefabRef?.Dispose();
    }
}
