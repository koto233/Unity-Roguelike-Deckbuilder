using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using LitFramework;
using LitFramework.Asset;
using LitFramework.EventBus;
using LitFramework.ObjectPool;
using LitFramework.UI.Core.Window;
using Newtonsoft.Json;
using UnityEngine;

public partial class UIHandZone : MonoBehaviour
{
    [SerializeField] private FanLayout _fanLayout;
    [SerializeField] private Transform _handContainer;
    [SerializeField] private Transform _cardDetailTrans;
    private readonly Dictionary<int, UICardItem> _cardItems = new();
    private ObjectPoolService _poolService;
    private UIBattle _battleWindow;
    private CancellationTokenSource _animationCts;
    public event Action<Card> OnAnyCardPlay;
    public event Action<Card, Vector2> OnAnyCardDragStart;
    public event Action<Card> OnAnyCardDragEnd;
    public event Action<Enemy, Vector2> OnAnyCardDrag;
    public void Init(ObjectPoolService poolService, UIBattle battleWindow)
    {
        _poolService = poolService;
        _battleWindow = battleWindow;
        _cardDetailTrans.gameObject.SetActive(false);
    }

    public void RefreshHand(
        IReadOnlyList<Card> handCards,
        IReadOnlyList<Card> changedCards,
        ChangeType type)
    {
        Debug.Log("刷新手牌" + type);
        switch (type)
        {
            case ChangeType.Remove:
                List<UICardItem> removedItems = new List<UICardItem>();
                Debug.Log($"手牌移除 {changedCards.Count} 张");
                foreach (var card in changedCards)
                {
                    if (_cardItems.TryGetValue(card.InstanceId, out var item))
                    {
                        _cardItems.Remove(card.InstanceId);
                        item.transform.SetParent(transform.parent, worldPositionStays: true);
                        removedItems.Add(item);
                    }
                }
                _fanLayout.Refresh();
                // 改为 UniTask 启动，不阻塞 UI
                PlayDiscardAnimations(removedItems).Forget();
                break;

            case ChangeType.Add:
                AddCards(changedCards);
                PlayAddAnimations(changedCards).Forget();
                break;

            case ChangeType.Refresh:
                RebuildUI(handCards);
                break;
        }
    }

    private void AddCards(IReadOnlyList<Card> changedCards)
    {
        foreach (var card in changedCards)
        {
            if (_cardItems.ContainsKey(card.InstanceId))
                continue;
            var uiCard = CreateCard(card);
            _cardItems.Add(card.InstanceId, uiCard);
        }

        _fanLayout.Refresh();
        foreach (var item in _cardItems.Values)
            item.OnLayoutComplete();
    }

    private void RebuildUI(IReadOnlyList<Card> handCards)
    {
        ClearHandContainer();
        _cardItems.Clear();
        foreach (var card in handCards)
        {
            var uiCard = CreateCard(card);
            _cardItems.Add(card.InstanceId, uiCard);
        }
        _fanLayout.Refresh();
        foreach (var item in _cardItems.Values)
            item.OnLayoutComplete();
    }
    /// <summary>
    /// 从池中获取一个卡牌实例并初始化
    /// </summary>
    private UICardItem CreateCard(Card card)
    {
        var go = _poolService.GetGameObject<UICardItem>();
        go.SetActive(true);
        go.transform.SetParent(_handContainer);

        var uiCard = go.GetComponent<UICardItem>();
        uiCard.Init(card, _cardDetailTrans);
        uiCard.OnPlay += (c) => OnAnyCardPlay?.Invoke(c);
        uiCard.OnDragStart += (c, pos) => OnAnyCardDragStart?.Invoke(c, pos);
        uiCard.OnDragEnd += (c) => OnAnyCardDragEnd?.Invoke(c);
        uiCard.OnCardDrag += (c, enemy) => OnAnyCardDrag?.Invoke(c, enemy);
        return uiCard;
    }

    private async UniTask PlayDiscardAnimations(List<UICardItem> cards)
    {
        if (cards.Count == 0) return;

        // 取消旧动画
        CancelAnimations();
        _animationCts = new CancellationTokenSource();
        var cts = _animationCts;
        var token = cts.Token;

        var flyTasks = new List<UniTask>();
        var flyObjs = new List<GameObject>();

        try
        {
            // ---- 阶段1：缩卡 + 创建飞行任务 ----
            foreach (var item in cards)
            {
                // 🔥 每轮循环检查取消
                token.ThrowIfCancellationRequested();

                // 缩放动画传入 token，支持中途停止
                await item.transform.DOScale(Vector3.zero, 0.1f)
                    .ToUniTask(cancellationToken: token);

                var flyFx = _poolService.GetComponent<CardFlyFx>();
                flyFx.gameObject.SetActive(true);
                flyObjs.Add(flyFx.gameObject);

                // 🔥 飞行任务传入 token，由 FlyToTarget 内部响应取消
                flyTasks.Add(flyFx.FlyToTarget(
                    item.transform.position,
                    _battleWindow.DiscardPileTrans.position,
                    transform.parent, token, 0.2f));
            }

            // ---- 阶段2：等待所有飞行完成 ----
            // 🔥 不再使用 AttachExternalCancellation，直接等待
            // 取消时 flyTasks 内部会抛 OperationCanceledException
            await UniTask.WhenAll(flyTasks);
        }
        catch (OperationCanceledException)
        {
            // 🔥 取消是预期的，静默退出
            Debug.Log("丢弃动画被取消");
        }
        finally
        {
            // 回收卡牌
            foreach (var item in cards)
                ReturnCardToPool(item);
            // 回收飞行特效
            foreach (var flyObj in flyObjs)
                _poolService.ReturnGameObject<CardFlyFx>(flyObj);

            // 如果已经取消了，把 cts 标记为 null（避免重复取消）
            if (cts.IsCancellationRequested)
                _animationCts = null;
        }
    }

    // ========== 🔥 核心改动：播放新增动画（真正可取消） ==========
    private async UniTask PlayAddAnimations(IReadOnlyList<Card> changedCards)
    {
        if (changedCards.Count == 0) return;

        Debug.Log("添加手牌数" + changedCards.Count);

        CancelAnimations();
        _animationCts = new CancellationTokenSource();
        var cts = _animationCts;
        var token = cts.Token;

        var flyTasks = new List<UniTask>();
        var flyObjs = new List<GameObject>();
        var newItems = new List<UICardItem>();

        try
        {
            // ---- 阶段1：隐藏卡牌 + 创建飞行任务 ----
            foreach (var card in changedCards)
            {
                token.ThrowIfCancellationRequested();

                if (_cardItems.TryGetValue(card.InstanceId, out var item))
                {
                    item.gameObject.SetActive(false);
                    item.transform.localScale = Vector3.zero;

                    var flyFx = _poolService.GetComponent<CardFlyFx>();
                    flyFx.gameObject.SetActive(true);
                    flyObjs.Add(flyFx.gameObject);
                    newItems.Add(item);

                    flyTasks.Add(flyFx.FlyToTarget(
                        _battleWindow.DrawPileTrans.position,
                        item.transform.position,
                        transform.parent, token,
                        0.2f
                        ));
                }
            }

            // ---- 阶段2：等待所有飞行完成 ----
            await UniTask.WhenAll(flyTasks);

            // ---- 阶段3：显示卡牌（缩放弹出） ----
            foreach (var item in newItems)
            {
                token.ThrowIfCancellationRequested();
                item.transform.DOScale(Vector3.one, 0.1f)
                   .ToUniTask(cancellationToken: token).Forget();
                item.gameObject.SetActive(true);
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("添加动画被取消");
        }
        finally
        {
            foreach (var flyObj in flyObjs)
                _poolService.ReturnGameObject<CardFlyFx>(flyObj);

            if (cts.IsCancellationRequested)
                _animationCts = null;
        }
    }

    public void CancelAnimations()
    {
        if (_animationCts != null)
        {
            _animationCts.Cancel();
            // _animationCts.Dispose();
            _animationCts = null;
        }
    }

    private void ClearHandContainer()
    {
        for (int i = _handContainer.childCount - 1; i >= 0; i--)
        {
            Transform child = _handContainer.GetChild(i);
            var item = child.GetComponent<UICardItem>();
            if (item != null)
                ReturnCardToPool(item);
            else
                Destroy(child.gameObject);
        }
        _cardItems.Clear();
    }
    private void ReturnCardToPool(UICardItem item)
    {
        if (item == null) return;
        item.gameObject.SetActive(false);
        item.transform.localScale = Vector3.one;
        item.transform.localRotation = Quaternion.identity;
        _poolService.ReturnGameObject<UICardItem>(item.gameObject);
    }

    public void RefreshHandState(int currentEnergy)
    {
        foreach (var item in _cardItems.Values)
            item.RefreshState(currentEnergy);
    }

    public void ResetCard()
    {
        foreach (var item in _cardItems.Values)
            item.ResetCard();
        _fanLayout.Refresh();
    }
}