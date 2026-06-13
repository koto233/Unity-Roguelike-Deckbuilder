using System.Collections;
using System.Collections.Generic;
using LitFramework.UI.Core.Window;
using UnityEngine;
using UnityEngine.EventSystems;

public partial class UICardItem : UIWindow, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Card _card;
    private BattleContext _context;
    private System.Action<Card> _onPlayCard;


    public void RefreshCard(Card card, BattleContext context, System.Action<Card> onPlay)
    {
        _card = card;
        _context = context;
        _onPlayCard = onPlay;
        b_CostText.SetText(card.CurrentCost.ToString());
        b_NameText.SetText(card.Config.Name);
        // ... 刷新UI显示（费用、名称等）
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        // // 只有费用足够时才允许拖拽（否则直接return）
        // if (_context.Player.Energy < _card.CurrentCost) return;

        // // 记录原始状态
        // originalPos = rectTransform.anchoredPosition;
        // originalParent = transform.parent;

        // // 创建拖拽克隆体
        // dragGhost = Instantiate(dragPrefab != null ? dragPrefab : gameObject, canvas.transform);
        // var ghostRect = dragGhost.GetComponent<RectTransform>();
        // ghostRect.anchoredPosition = eventData.position;
        // var ghostCanvasGroup = dragGhost.GetComponent<CanvasGroup>();
        // if (ghostCanvasGroup == null) ghostCanvasGroup = dragGhost.AddComponent<CanvasGroup>();
        // ghostCanvasGroup.alpha = 0.7f;
        // ghostCanvasGroup.blocksRaycasts = false;  // 让克隆体不阻挡射线

        // // 原卡牌半透明且不再响应射线（避免二次拖拽）
        // canvasGroup.alpha = 0.5f;
        // canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // if (dragGhost == null) return;
        // dragGhost.GetComponent<RectTransform>().anchoredPosition = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //  // 清理克隆体
        // if (dragGhost != null) Destroy(dragGhost);

        // // 恢复原卡牌
        // canvasGroup.alpha = 1f;
        // canvasGroup.blocksRaycasts = true;

        // // 如果拖拽开始时因为能量不足而返回，此时直接结束
        // if (_context.Player.Energy < _card.CurrentCost) return;

        // // 检测释放点下方的目标
        // var results = new List<RaycastResult>();
        // EventSystem.current.RaycastAll(eventData, results);

        // Character target = null;
        // foreach (var hit in results)
        // {
        //     // 假设敌人身上挂有 EnemyCharacter 组件并实现了 ICharacter 接口
        //     var enemy = hit.gameObject.GetComponent<EnemyCharacter>();
        //     if (enemy != null)
        //     {
        //         target = enemy.Character;
        //         break;
        //     }
    }

    // // 根据卡牌目标类型判断是否有效
    // bool isValid = false;
    // if (_card.Config.TargetType == "Enemy" && target != null)
    //     isValid = true;
    // else if (_card.Config.TargetType == "Self")
    //     isValid = true;   // 目标为自己，无需检测敌人

    // if (isValid)
    // {
    //     _context.CurrentTarget = target;   // 设置到上下文中
    //     _onPlayCard?.Invoke(_card);
    // }
    // else
    // {
    //     // 无效拖拽：可播放提示音效或UI动画（卡牌飞回）
    // }
    // }
    protected override void OnShowInternal(object param)
    {

    }
}
