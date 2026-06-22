using System;
using System.Collections;
using System.Collections.Generic;
using LitFramework.UI.Core.Window;
using UnityEngine;
using UnityEngine.EventSystems;

public partial class UICardItem : UIBase, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CardDisplayData _displayData;
    private Action<string> _onPlay;
    private Action<string> _onCancel;
    private Action<string> _onDragStart;
    private Action<string, Vector2> _onCardDrag;
    public void Init(CardDisplayData displayData, Action<string> onPlay = null, Action<string> onCancel = null, Action<string> onDragStart = null, Action<string, Vector2> onCardDrag = null)
    {
        _onPlay = onPlay;
        _onCancel = onCancel;
        _onDragStart = onDragStart;
        _onCardDrag = onCardDrag;
        RefreshUI(displayData);
    }
    public void RefreshUI(CardDisplayData data)
    {
        _displayData = data;
        b_CostText.SetText(data.Cost.ToString());
        b_NameText.SetText(data.Name);
        b_DescText.SetText(data.Description);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // if (!_card.IsPlayable)
        // {
        //     eventData.pointerDrag = null; // 禁止拖拽
        //     return;
        // }
        // _canvasGroup.blocksRaycasts = false;
        _onDragStart?.Invoke(_displayData.CardId);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
        _onCardDrag?.Invoke(_displayData.CardId, eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 检查是否拖拽到了目标区域
        CharacterData target = IsOverTarget(eventData);
        if (target != null)
        {
            _onPlay?.Invoke(_displayData.CardId);
        }
        else
        {
            _onCancel?.Invoke(_displayData.CardId);
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
