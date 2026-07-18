using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using LitFramework;
using LitFramework.Asset;
using LitFramework.ObjectPool;
using LitFramework.UI.Core.Window;
using UnityEngine;

public partial class UIBattleWindow : UIWindow
{
    private AssetRef<GameObject> _cardPrefabRef;
    private AssetRef<GameObject> _cardDisplayPrefabRef;
    private AssetRef<GameObject> _flyPrefabRef;
    private AssetRef<GameObject> _floatingTextPrefabRef;
    private AssetRef<GameObject> _enemyPrefabRef;
    private AssetRef<GameObject> _playerPrefabRef;
    private UIPlayerItem _playerView;
    private Dictionary<int, UIEnemyItem> _enemyViews = new();
    private ObjectPoolService _poolService;
    public Transform DiscardPileTrans => b_DiscardPileBtn.transform;
    public Transform DrawPileTrans => b_DrawPileBtn.transform;
    /// <summary>
    /// 打开卡组，0是抽牌堆，1是弃牌堆
    /// </summary>
    public event Action<int> OnOpenPile;
    public event Action OnEndTurn;
    private List<UICardDisplay> _cardDisplays = new();
    protected override async UniTask OnOpenAsync(object param)
    {
        var battleContext = param as BattleContext;
        if (battleContext == null)
        {
            Debug.LogError("参数类型错误，需要 BattleContext");
            return;
        }
        var assetService = ServiceLocator.Get<IAssetService>();
        _enemyPrefabRef = await assetService.LoadRefAsync<GameObject>("Assets/Res/UI/UIEnemyItem.prefab");
        _playerPrefabRef = await assetService.LoadRefAsync<GameObject>("Assets/Res/UI/UIPlayerItem.prefab");
        _cardPrefabRef = await assetService.LoadRefAsync<GameObject>("Assets/Res/UI/UICardItem.prefab");
        _cardDisplayPrefabRef = await assetService.LoadRefAsync<GameObject>("Assets/Res/UI/UICardDisplay.prefab");
        _flyPrefabRef = await assetService.LoadRefAsync<GameObject>("Assets/Res/UI/CardFlyFx.prefab");
        _floatingTextPrefabRef = await assetService.LoadRefAsync<GameObject>("Assets/Res/UI/UIFloatingText.prefab");
        _poolService = ServiceLocator.Get<ObjectPoolService>();
        InitObjectPools();
        _playerView = CreatePlayerView(battleContext.Player);
        CreateEnemyViews(battleContext.Enemies);
        InitUI();
    }

    private void InitObjectPools()
    {
        _poolService.RegisterGameObjectPool<UICardItem>(_cardPrefabRef.Asset, initialPoolSize: 10);
        _poolService.RegisterGameObjectPool<UICardDisplay>(_cardDisplayPrefabRef.Asset, initialPoolSize: 10);
        _poolService.RegisterGameObjectPool<CardFlyFx>(_flyPrefabRef.Asset, initialPoolSize: 10);
        _poolService.RegisterGameObjectPool<UIFloatingTextItem>(_floatingTextPrefabRef.Asset, initialPoolSize: 10);
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
            _poolService.ReturnGameObject<UICardDisplay>(item.gameObject);
        }
        _cardDisplays.Clear();
    }
    public void SpawnCardInList(List<Card> cards)
    {
        foreach (var data in cards)
        {
            var cardDisplayPrefab = _poolService.GetGameObject<UICardDisplay>();
            cardDisplayPrefab.transform.SetParent(b_PilePanel.transform);
            var uiCard = cardDisplayPrefab.GetComponent<UICardDisplay>();
            uiCard.Init(data);
            _cardDisplays.Add(uiCard);
        }
    }

    private UIPlayerItem CreatePlayerView(Player data)
    {
        var go = Instantiate(_playerPrefabRef.Asset, b_PlayerRoot);
        var view = go.GetComponent<UIPlayerItem>();
        return view;
    }

    private void CreateEnemyViews(List<Enemy> enemies)
    {
        foreach (var enemy in enemies)
        {
            var go = Instantiate(_enemyPrefabRef.Asset, b_EnemysRoot);
            var view = go.GetComponent<UIEnemyItem>();
            view.SetEnemy(enemy);
            _enemyViews[enemy.Id] = view;
        }
    }

    public UIEnemyItem GetEnemyView(int enemyId)
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
    public void RefreshHand(List<Card> hand, List<Card> changedCards, ChangeType type, Action onPlay = null, Action onCancel = null, Action<int> onDragStart = null, Action<Enemy> onCardDrag = null)
    {
        b_HandZone.RefreshHand(hand, changedCards, type, onPlay, onCancel, onDragStart, onCardDrag);
    }
    public void RefreshBlock(int block, EntityType entityType)
    {
        if (entityType != EntityType.Player)
        {
            return;
        }
        b_BlockText.SetText(block.ToString());
    }
    public void ResetCard()
    {
        b_HandZone.ResetCard();
    }

    // ===== View 层交互反馈 =====

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
        var item = _poolService.GetGameObject<UIFloatingTextItem>();
        item.transform.SetParent(transform);
        item.transform.position = position;
        var uiFloatingText = item.GetComponent<UIFloatingTextItem>();
        await uiFloatingText.PlayAsync(text, position, color, isCritical);
        _poolService.ReturnGameObject<UIFloatingTextItem>(item);
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
    protected override void OnClose()
    {
        base.OnClose();
        b_BuffTooltip.Hide();
        b_IntentTooltip.Hide();
        b_HandZone.CancelAnimations();

    }
}
