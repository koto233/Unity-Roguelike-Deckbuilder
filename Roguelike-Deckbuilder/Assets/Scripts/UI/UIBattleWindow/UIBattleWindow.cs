using System;
using System.Collections;
using System.Collections.Generic;
using LitFramework.UI.Core.Window;
using UnityEngine;

public partial class UIBattleWindow : UIWindow
{
    public void Init(GameObject cardPrefab)
    {
        b_HandZone.Init(cardPrefab);
    }
    public override void OnOpen(object args)
    {
        base.OnOpen(args);
    }
    protected override void OnShowInternal(object param)
    {

    }

    public void RefreshHp(int currentHp, int maxHp)
    {
        b_HPText.SetText(currentHp + "/" + maxHp);
        b_HPSlider.value = currentHp / maxHp;

    }
    public void RefreshEnergy(int energy) { /* 更新能量显示 */ }
    public void RefreshHand(List<Card> hand, Action<Card> onCardPlay)
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
