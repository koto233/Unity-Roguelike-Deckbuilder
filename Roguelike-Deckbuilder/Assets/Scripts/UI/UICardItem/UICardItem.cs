using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using LitFramework;
using LitFramework.UI.Core.Window;
using UnityEngine;
using UnityEngine.EventSystems;

public partial class UICardItem
    : UIBase,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler,
        IPointerDownHandler,
        IPointerUpHandler
{
    public Card _card { get; private set; }
    private Transform _cardDetailTrans;
    private Vector2 _dragStartOffset; // 开始拖拽时鼠标与卡牌的偏移
    private Vector2 _originalPos;
    private float _selectPositionY; // 拖拽时鼠标与卡牌的偏移
    private float _maxDragY = 200f;
    private Quaternion _originalRotation;
    private Transform _originalParent;
    private int _originalSiblingIndex;
    private Action _onPlay;
    private Action _onDragEnd;
    private Action<int, Vector2> _onDragStart;
    private Action<Enemy, Vector2> _onCardDrag;
    private Enemy _Target;
    private bool _canUse = true;
    private bool _isScaleNormalized = false;
    private RectTransform _rect;
    private BattleInteractionService _battleInteraction;

    public void Init(
        Card card,
        Transform cardDetailTrans,
        Action onPlay = null,
        Action _onDragEnd = null,
        Action<int, Vector2> onDragStart = null,
        Action<Enemy, Vector2> onCardDrag = null
    )
    {
        _rect = GetComponent<RectTransform>();
        _onPlay = onPlay;
        this._onDragEnd = _onDragEnd;
        this._onDragStart = onDragStart;
        _onCardDrag = onCardDrag;
        _cardDetailTrans = cardDetailTrans;
        RefreshUI(card);
        _battleInteraction = ServiceLocator.Get<BattleInteractionService>();
    }

    public void OnLayoutComplete()
    {
        _originalPos = _rect.localPosition;
        _originalRotation = _rect.localRotation;
        _originalParent = transform.parent;
        _originalSiblingIndex = transform.GetSiblingIndex();
        var parentRect = _rect.parent as RectTransform;
        var localBottom = parentRect.rect.yMin;
        float cardHeight = _rect.rect.height * 1.6f;
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
        transform.position = _canUse
            ? transform.position
            : new Vector3(transform.position.x, transform.position.y - 10f, transform.position.z);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!_battleInteraction.CanInteract())
            return;
        if (!_canUse)
            return;
        _battleInteraction.StartDrag(this);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rect.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localMousePos
        );
        _dragStartOffset = _rect.anchoredPosition - localMousePos;
        Vector2 localStartPos = _rect.parent.InverseTransformPoint(b_ArrowStartRoot.position);
        _onDragStart?.Invoke(_card.InstanceId, localMousePos);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_battleInteraction.IsDragging)
            return;
        if (_battleInteraction.DraggingCard != this)
            return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rect.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localMousePos
        );
        Vector2 targetPos = localMousePos + _dragStartOffset;
        if (!_canUse)
        {
            targetPos.y = Mathf.Min(targetPos.y, _maxDragY);
        }
        if (_card.NeedTarget)
        {
            if (!_isScaleNormalized)
            {
                transform.DOScale(1f, 0.1f);
                _isScaleNormalized = true;
            }
            targetPos = new Vector2(_originalPos.x, _selectPositionY);
            _Target = IsOverTarget(eventData);
        }

        _rect.anchoredPosition = targetPos;
        _onCardDrag?.Invoke(_Target, localMousePos);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_battleInteraction.IsDragging || _battleInteraction.DraggingCard != this)
            return;

        _battleInteraction.EndDrag();
        _onDragEnd?.Invoke();
        if (ShouldCancelDrag())
        {
            ResetCard();
            return;
        }

        _onPlay?.Invoke();
    }

    private bool ShouldCancelDrag()
    {
        return !_canUse || (_card.NeedTarget ? _Target == null : transform.localPosition.y < _maxDragY);
    }

    private Enemy IsOverTarget(PointerEventData eventData)
    {
        // int layerMask = 1 << LayerMask.NameToLayer("Target");
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            // ✅ 用 Tag 判断具体类型
            if (result.gameObject.CompareTag("Enemy"))
            {
                // ✅ 用 Tag 获取目标
                var enemy = result.gameObject.GetComponent<UIEnemyItem>();
                if (enemy != null)
                {
                    return enemy.Enemy;
                }
            }
        }
        // 判断鼠标位置是否在目标区域（由 View 层的碰撞检测负责）
        return null;
    }

    public void ResetCard()
    {
        _isScaleNormalized = false;
        transform.DOLocalMove(_originalPos, 0.2f);
        transform.DOScale(1f, 0.1f);
        transform.DORotateQuaternion(_originalRotation, 0.1f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_battleInteraction.CanInteract())
            return;
        _cardDetailTrans.gameObject.SetActive(true);
        // Debug.Log("卡牌位置刷新" + _selectPositionY);
        transform.DOKill();
        transform.SetParent(_cardDetailTrans, worldPositionStays: true);
        transform.DOScale(1.6f, 0.1f);
        transform.DOLocalMove(
            new Vector3(transform.localPosition.x, _selectPositionY, transform.localPosition.z),
            0.1f
        );
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.DOKill();
        transform.SetParent(_originalParent, worldPositionStays: true);
        _cardDetailTrans.gameObject.SetActive(false);
        transform.SetSiblingIndex(_originalSiblingIndex);
        _dragStartOffset = Vector2.zero;
        if (_battleInteraction.IsDragging)
        {
            return;
        }
        ResetCard();
    }
}
