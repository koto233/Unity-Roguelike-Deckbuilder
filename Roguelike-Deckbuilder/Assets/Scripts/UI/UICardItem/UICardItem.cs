using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using LitFramework.UI.Core.Window;
using UnityEngine;
using UnityEngine.EventSystems;

public partial class UICardItem : UIBase, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private Card _card;
    public int InstanceId => _card.InstanceId;
    private Transform _cardDetailTrans;
    private Vector2 _dragStartOffset;  // 开始拖拽时鼠标与卡牌的偏移
    private Vector2 _originalPos;
    private float _selectPositionY;  // 拖拽时鼠标与卡牌的偏移
    private float _maxDragY = 200f;
    private Quaternion _originalRotation;
    private Transform _originalParent;
    private int _originalSiblingIndex;
    private Action _onPlay;
    private Action _onCancel;
    private Action<int> _onDragStart;
    private Action<Enemy> _onCardDrag;
    private Enemy _Target;
    private bool _canUse = true;
    private Tween _flyTween; // 用于管理飞行补间，防止冲突
    public void Init(Card card, Transform cardDetailTrans, Action onPlay = null, Action onCancel = null, Action<int> onDragStart = null, Action<Enemy> onCardDrag = null)
    {
        _onPlay = onPlay;
        _onCancel = onCancel;
        _onDragStart = onDragStart;
        _onCardDrag = onCardDrag;
        _cardDetailTrans = cardDetailTrans;
        RefreshUI(card);
    }

    public void OnLayoutComplete()
    {
        _originalPos = b_UICardItemRect.localPosition;
        _originalRotation = b_UICardItemRect.localRotation;
        _originalParent = transform.parent;
        _originalSiblingIndex = transform.GetSiblingIndex();
        var parentRect = b_UICardItemRect.parent as RectTransform;
        var localBottom = parentRect.rect.yMin;
        float cardHeight = b_UICardItemRect.rect.height * 1.6f;
        _selectPositionY = localBottom + cardHeight / 2f;
        _maxDragY = _originalPos.y + cardHeight / 2f;
    }

    public void RefreshUI(Card card)
    {
        _card = card;
        _canUse = card.CanUse;
        b_CostText.SetText(card.Config.Cost.ToString());
        Color targetColor = _canUse ? Color.black : Color.red;
        b_CostText.color = targetColor;
        b_NameText.SetText(card.Config.Name);
        b_DescText.SetText(card.Description);
    }
    public void RefreshState(int currentEnergy)
    {
        _canUse = currentEnergy >= _card.Config.Cost;
        Color targetColor = _canUse ? Color.black : Color.red;
        // b_CostText.DOColor(targetColor, 0.5f);
        b_CostText.color = targetColor;
        transform.position = _canUse ? transform.position : new Vector3(transform.position.x, transform.position.y - 10f, transform.position.z);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!_canUse) return;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
       b_UICardItemRect.parent as RectTransform,
       eventData.position,
       eventData.pressEventCamera,
       out Vector2 localMousePos
        );
        _dragStartOffset = b_UICardItemRect.anchoredPosition - localMousePos;
        _onDragStart?.Invoke(_card.InstanceId);

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
        if (_card.NeedTarget)
        {
            transform.DOScale(1f, 0.1f);
            targetPos = new Vector2(_originalPos.x, _selectPositionY);
            _Target = IsOverTarget(eventData);
            // Debug.Log("获取目标" + _Target == null);
        }

        b_UICardItemRect.anchoredPosition = targetPos;
        _onCardDrag?.Invoke(_Target);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_canUse || transform.localPosition.y < _maxDragY)
        {
            ResetCard();
            return;
        }
        _onPlay?.Invoke();
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
        transform.DOLocalMove(_originalPos, 0.2f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _cardDetailTrans.gameObject.SetActive(true);
        // Debug.Log("卡牌位置刷新" + _selectPositionY);
        transform.DOKill();
        transform.SetParent(_cardDetailTrans, worldPositionStays: true);
        transform.DOScale(1.6f, 0.1f);
        transform.DOLocalMove(new Vector3(transform.localPosition.x, _selectPositionY, transform.localPosition.z), 0.1f);
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.DOKill();
        transform.SetParent(_originalParent, worldPositionStays: true);
        transform.DOScale(1f, 0.1f);
        transform.DORotateQuaternion(_originalRotation, 0.1f);
        transform.DOLocalMove(_originalPos, 0.1f);
        transform.SetSiblingIndex(_originalSiblingIndex);
        _cardDetailTrans.gameObject.SetActive(false);
    }
}
