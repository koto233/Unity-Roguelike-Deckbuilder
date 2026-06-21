using System;
using System.Collections;
using System.Collections.Generic;
using LitFramework.UI.Core.Window;
using UnityEngine;
using UnityEngine.EventSystems;

public partial class UICardItem : UIBase, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Card _card;
    private System.Action<Card, CharacterData> _onPlayCard;
    private Action<Card> _onCardCancel;
    private Action<string> _onCardDragStart;
    private Action<string, Vector2> _onCardDrag;
    public void Refresh(Card card, System.Action<Card, CharacterData> onPlay)
    {
        _card = card;
        _onPlayCard = onPlay;
        b_CostText.SetText(card.CurrentCost.ToString());
        b_NameText.SetText(card.Config.Name);
        string desc = string.Format(card.Config.Description, card.Config.Effects[0].Value);
        b_DescText.SetText(desc);
        // ... 刷新UI显示（费用、名称等）
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        // if (!_card.IsPlayable)
        // {
        //     eventData.pointerDrag = null; // 禁止拖拽
        //     return;
        // }
        // _canvasGroup.blocksRaycasts = false;
        _onCardDragStart?.Invoke(_card.Config.ID);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
        _onCardDrag?.Invoke(_card.Config.ID, eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 检查是否拖拽到了目标区域
        CharacterData target = IsOverTarget(eventData);
        if (target != null)
        {
            _onPlayCard?.Invoke(_card, target);
        }
        else
        {
            _onCardCancel?.Invoke(_card);
        }
    }
    private CharacterData IsOverTarget(PointerEventData eventData)
    {
        int layerMask = 1 << LayerMask.NameToLayer("Target");
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            // ✅ 用 Tag 判断具体类型
            if (result.gameObject.CompareTag("Enemy"))
            {
                return result.gameObject.GetComponent<UIEnemyItem>().Enemy;
            }
        }
        // 判断鼠标位置是否在目标区域（由 View 层的碰撞检测负责）
        return null;
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

}
