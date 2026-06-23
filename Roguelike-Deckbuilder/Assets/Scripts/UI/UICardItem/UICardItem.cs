using System;
using System.Collections;
using System.Collections.Generic;
using LitFramework.UI.Core.Window;
using UnityEngine;
using UnityEngine.EventSystems;

public partial class UICardItem : UIBase, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CardDisplayData _displayData;
    private Vector2 _dragStartOffset;  // 开始拖拽时鼠标与卡牌的偏移
    private Vector2 _originalPos;
    private float _maxDragY = 200f;
    private Action _onPlay;
    private Action _onCancel;
    private Action<string> _onDragStart;
    private Action<EnemyData> _onCardDrag;
    private float _followSpeed = 10f;
    private EnemyData _Target;
    public void Init(CardDisplayData displayData, Action onPlay = null, Action onCancel = null, Action<string> onDragStart = null, Action<EnemyData> onCardDrag = null)
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
        _originalPos = b_UICardItemRect.anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
       b_UICardItemRect.parent as RectTransform,
       eventData.position,
       eventData.pressEventCamera,
       out Vector2 localMousePos
        );
        _dragStartOffset = b_UICardItemRect.anchoredPosition - localMousePos;
        _onDragStart?.Invoke(_displayData.CardId);

    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
               b_UICardItemRect.parent as RectTransform,
               eventData.position,
               eventData.pressEventCamera,
               out Vector2 localMousePos
           );
        Vector2 targetPos = localMousePos + _dragStartOffset;
        if (_displayData.NeedTarget)
        {
            targetPos.y = Mathf.Min(targetPos.y, _maxDragY);
            // targetPos.x = Mathf.Clamp(targetPos.x, -_maxDragY, _maxDragY);
            _Target = IsOverTarget(eventData);
            Debug.Log("获取目标" + _Target == null);
        }

        b_UICardItemRect.anchoredPosition = Vector2.Lerp(
        b_UICardItemRect.anchoredPosition,
        targetPos,
        _followSpeed * Time.deltaTime);
        _onCardDrag?.Invoke(_Target);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _onPlay?.Invoke();
        // // 检查是否拖拽到了目标区域
        // if (eventData.position.y < _maxDragY - 50)
        // {
        //     _onCancel?.Invoke();
        //     b_UICardItemRect.anchoredPosition = _originalPos;
        // }
        // else
        // {
        //     if (_displayData.NeedTarget && _Target == null)
        //     {
        //         _onCancel?.Invoke();
        //         b_UICardItemRect.anchoredPosition = _originalPos;
        //     }
        //     else
        //     {
        //         _onPlay?.Invoke();
        //     }
        // }
    }
    private EnemyData IsOverTarget(PointerEventData eventData)
    {
        int layerMask = 1 << LayerMask.NameToLayer("Target");
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            // ✅ 用 Tag 判断具体类型
            if (result.gameObject.CompareTag("Enemy"))
            {

                // ✅ 用 Tag 获取目标
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
