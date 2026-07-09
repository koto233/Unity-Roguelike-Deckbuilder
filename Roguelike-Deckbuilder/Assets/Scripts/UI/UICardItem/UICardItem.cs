using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using LitFramework.UI.Core.Window;
using UnityEngine;
using UnityEngine.EventSystems;

public partial class UICardItem : UIBase, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private CardDisplayData _displayData;
    private Vector2 _dragStartOffset;  // 开始拖拽时鼠标与卡牌的偏移
    private Vector2 _originalPos;
    private float _maxDragY = 200f;
    private Quaternion _originalRotation;
    private Action _onPlay;
    private Action _onCancel;
    private Action<int> _onDragStart;
    private Action<Enemy> _onCardDrag;
    private float _followSpeed = 10f;
    private Enemy _Target;
    private bool _isDrag = false;
    private bool _canUse = true;
    public void Init(CardDisplayData displayData, Action onPlay = null, Action onCancel = null, Action<int> onDragStart = null, Action<Enemy> onCardDrag = null)
    {
        _onPlay = onPlay;
        _onCancel = onCancel;
        _onDragStart = onDragStart;
        _onCardDrag = onCardDrag;

        RefreshUI(displayData);
    }
    public void RefreshUI(CardDisplayData data)
    {
        Debug.Log($"传入的 data.CanUse = {data.CanUse}, data 哈希 = {data.GetHashCode()}");
        _displayData = data;
        _canUse = data.CanUse;
        b_CostText.SetText(data.Cost.ToString());
        Color targetColor = _canUse ? Color.black : Color.red;
        b_CostText.color = targetColor;
        Debug.Log("卡牌状态刷新" + _canUse + " 卡牌消耗" + _displayData.Cost);
        b_NameText.SetText(data.Name);
        b_DescText.SetText(data.Description);
    }
    public void RefreshState(int currentEnergy)
    {
        _canUse = currentEnergy >= _displayData.Cost;
        Debug.Log("卡牌状态刷新" + _canUse + " 当前能量" + currentEnergy + " 卡牌消耗" + _displayData.Cost);
        Color targetColor = _canUse ? Color.black : Color.red;
        // b_CostText.DOColor(targetColor, 0.5f);
        b_CostText.color = targetColor;
        Debug.Log("卡牌状态刷新" + targetColor);
        transform.position = _canUse ? transform.position : new Vector3(transform.position.x, transform.position.y - 10f, transform.position.z);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!_canUse) return;
        _isDrag = true;
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
        if (!_canUse)
        {
            targetPos.y = Mathf.Min(targetPos.y, _maxDragY);
            return;
        }
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
        if (!_canUse)
        {
            return;
        }
        // if (_displayData.NeedTarget && eventData.position.y < _maxDragY - 50)
        // {

        // }

        _onPlay?.Invoke();
        // // 检查是否拖拽到了目标区域
        // 
        // {
        //     _onCancel?.Invoke();
        //     b_UICardItemRect.anchoredPosition = _originalPos;
        // }
        // else
        // {
        //    
        //     else
        //     {
        //         _onPlay?.Invoke();
        //     }
        // }
    }
    private Enemy IsOverTarget(PointerEventData eventData)
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
    public void ResetCard()
    {
        b_UICardItemRect.anchoredPosition = _originalPos;

    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        if (_isDrag || !_canUse) return;
        _originalPos = b_UICardItemRect.anchoredPosition;
        _originalRotation = b_UICardItemRect.localRotation;
        var parentRect = b_UICardItemRect.parent as RectTransform;
        var localBottom = parentRect.rect.yMin;
        float cardHeight = b_UICardItemRect.rect.height * 1.2f;
        // Debug.Log($"cardHeight{cardHeight}+localBottom{localBottom}");
        float targetY = localBottom + cardHeight / 2f;
        // Debug.Log($"targetY{targetY} {localBottom} {cardHeight / 2f}");
        // 杀死旧的退出动画
        // _exitTween?.Kill();
        transform.localScale = Vector3.one * 1.2f;
        transform.localPosition = new Vector3(transform.localPosition.x, targetY, transform.localPosition.z);
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_isDrag || !_canUse) return;
        transform.DOScale(1f, 0.2f);
        transform.DORotateQuaternion(_originalRotation, 0.2f);
        transform.DOLocalMove(new Vector3(transform.localPosition.x, _originalPos.y, transform.localPosition.z), 0.2f);
        // ResetCard();
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
