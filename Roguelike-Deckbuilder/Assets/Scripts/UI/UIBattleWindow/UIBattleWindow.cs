using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitFramework;
using LitFramework.Asset;
using LitFramework.UI.Core.Window;
using UnityEngine;

public partial class UIBattleWindow : UIWindow
{
    private AssetRef<GameObject> _cardPrefabRef;
    private AssetRef<GameObject> _enemyPrefabRef;
    private AssetRef<GameObject> _playerPrefabRef;
    private UIPlayerItem _playerView;
    private List<UIEnemyItem> _enemyViews = new();
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
        b_HandZone.Init(_cardPrefabRef.Asset);
        _playerView = CreatePlayerView(battleContext.Player);
        _enemyViews = CreateEnemyViews(battleContext.Enemies);
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
    public void RefreshHp(int currentHp, int maxHp)
    {
        b_HPText.SetText(currentHp + "/" + maxHp);
        b_HPSlider.value = currentHp / maxHp;
        _playerView.UpdateHP(currentHp, maxHp);
    }
    public void RefreshEnergy(int energy, int maxEnergy)
    {
        b_EnergyText.SetText($"{energy}/{maxEnergy}");
    }
    public void RefreshHand(List<CardDisplayData> hand, System.Action<Card, CharacterData> onCardPlay)
    {
        b_HandZone.RefreshHand(hand, onCardPlay);
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
