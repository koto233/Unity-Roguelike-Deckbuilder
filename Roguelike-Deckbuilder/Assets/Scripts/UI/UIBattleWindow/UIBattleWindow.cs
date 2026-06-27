using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitFramework;
using LitFramework.Asset;
using LitFramework.ObjectPool;
using LitFramework.UI.Core.Window;
using UnityEngine;

public partial class UIBattleWindow : UIWindow
{
    private AssetRef<GameObject> _cardPrefabRef;
    private AssetRef<GameObject> _enemyPrefabRef;
    private AssetRef<GameObject> _playerPrefabRef;
    private UIPlayerItem _playerView;
    private List<UIEnemyItem> _enemyViews = new();
    private string _poolKey = "CardItem";
    private ObjectPoolService _poolService;
    /// <summary>
    /// 打开卡组，0是抽牌堆，1是弃牌堆
    /// </summary>
    public event Action<int> OnOpenPile;
    public event Action OnEndTurn;
    private List<UICardItem> _cardItems = new();
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
        _poolService = ServiceLocator.Get<ObjectPoolService>();
        _poolService.RegisterGameObjectPool(
            _poolKey,
            new GameObjectPool(_cardPrefabRef.Asset, initialPoolSize: 10)
        );
        _playerView = CreatePlayerView(battleContext.Player);
        _enemyViews = CreateEnemyViews(battleContext.Enemies);
        InitUI();
    }

    private void InitUI()
    {
        b_HandZone.Init(_poolKey, _poolService);
        b_ClosePileButton.onClick.AddListener(ClosePilePanel);
        ClosePilePanel();
        b_DrawPileBtn.onClick.AddListener(() => OnOpenPile(0));
        b_DiscardPileBtn.onClick.AddListener(() => OnOpenPile(1));
        b_EndTurnBtn.onClick.AddListener(() => OnEndTurn?.Invoke());
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
    public void ClearCardsInList()
    {
        foreach (var item in _cardItems)
        {
            item.gameObject.SetActive(false);
            _poolService.ReturnGameObject(_poolKey, item.gameObject);
        }
        _cardItems.Clear();
    }
    public void SpawnCardInList(CardDisplayData data)
    {

        var cardPrefab = _poolService.GetGameObject(_poolKey);
        cardPrefab.transform.SetParent(b_PilePanel.transform);
        var uiCard = cardPrefab.GetComponent<UICardItem>();
        uiCard.Init(data, null, null, null, null);
        _cardItems.Add(uiCard);
    }

    private UIPlayerItem CreatePlayerView(PlayerData data)
    {
        var go = Instantiate(_playerPrefabRef.Asset, b_PlayerRoot);
        var view = go.GetComponent<UIPlayerItem>();
        return view;
    }

    private List<UIEnemyItem> CreateEnemyViews(List<EnemyData> enemies)
    {
        var views = new List<UIEnemyItem>();
        foreach (var enemy in enemies)
        {
            var go = Instantiate(_enemyPrefabRef.Asset, b_EnemysRoot);
            var view = go.GetComponent<UIEnemyItem>();
            view.SetEnemy(enemy);
            // view.SetEnemyId(enemy.Id);
            views.Add(view);
        }
        return views;
    }
    private void OnDestroy()
    {
        _cardPrefabRef?.Dispose();
        _playerPrefabRef?.Dispose();
        _enemyPrefabRef?.Dispose();
    }
    public void RefreshHp(int currentHp, int maxHp, EntityType entityType)
    {
        if (entityType != EntityType.Player)
        {
            return;
        }
        b_HPText.SetText(currentHp + "/" + maxHp);
        b_HPSlider.value = (float)currentHp / maxHp;
        _playerView.UpdateHP(currentHp, maxHp);
    }
    public void RefreshEnergy(int energy, int maxEnergy)
    {
        b_EnergyText.SetText($"{energy}/{maxEnergy}");
    }
    public void RefreshHand(List<CardDisplayData> hand, Action onPlay = null, Action onCancel = null, Action<string> onDragStart = null, Action<EnemyData> onCardDrag = null)
    {
        b_HandZone.RefreshHand(hand, onPlay, onCancel, onDragStart, onCardDrag);
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
    public void HighlightTargets(List<string> validTargetIds)
    {
        // 高亮可用的目标（敌人）
        // foreach (Transform child in _enemyParent)
        // {
        //     var enemyView = child.GetComponent<UIEnemyItem>();
        //     enemyView.SetHighlight(validTargetIds.Contains(enemyView.EnemyId));
        // }
    }

    public void ClearHighlights()
    {
        // foreach (Transform child in _enemyParent)
        // {
        //     var enemyView = child.GetComponent<UIEnemyItem>();
        //     enemyView.SetHighlight(false);
        // }
    }

    public void ShowCardGhost(Vector2 position, Card data)
    {
        // 显示卡牌跟随鼠标的幻影
        // 简化版：直接移动卡牌本身，或者创建一个克隆体
    }

    public void HideCardGhost()
    {
        // 隐藏幻影
    }

    // ===== 拖拽回调（View 层触发） =====
    private void OnCardDragStart(string cardId)
    {
        // View 层纯粹通知 Presenter，不做业务判断
        // _onCardPlay?.Invoke(cardId); // 或者通过 EventBus 发送
    }

    private void OnCardDrag(string cardId, Vector2 position)
    {
        // 更新幻影位置
        // 检查是否悬停在目标上，更新高亮
    }

    private void OnCardDrop(string cardId)
    {
        // 通知 Presenter 卡牌被释放
    }

    private void OnCardCancel(string cardId)
    {
        // 取消使用卡牌
    }

    private void ClearCards()
    {
        // foreach (var item in _cardItems)
        // {
        //     Destroy(item.gameObject);
        // }
        // _cardItems.Clear();
    }
}
