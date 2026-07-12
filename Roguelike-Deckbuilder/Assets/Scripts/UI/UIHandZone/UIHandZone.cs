using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField]
    private FanLayout _fanLayout;
    [SerializeField]
    private Transform _handContainer;
    [SerializeField]
    private Transform _cardDetailTrans;
    private Dictionary<int, UICardItem> _cardItems = new();
    private BattleContext _battleContext;
    private string _cardPoolKey;
    private string _orbPoolKey;
    private ObjectPoolService _poolService;
    private UIBattleWindow _battleWindow;
    private List<GameObject> _flyObjs = new();
    private List<UniTask> _flyTasks = new List<UniTask>();
    public void Init(string cardPoolKey, string orbPoolKey, ObjectPoolService poolService, UIBattleWindow battleWindow)
    {
        _cardPoolKey = cardPoolKey;
        _orbPoolKey = orbPoolKey;
        _poolService = poolService;
        _battleWindow = battleWindow;
        _cardDetailTrans.gameObject.SetActive(false);

    }
    // void Update()
    // {
    //     _fanLayout.Refresh();
    // }

    // public void RefreshHand(List<CardDisplayData> handCards, List<Card> changedCards, ChangeType type, Action onPlay = null, Action onCancel = null, Action<int> onDragStart = null, Action<Enemy> onCardDrag = null)
    // {
    //     Debug.Log("刷新手牌");
    //     foreach (var card in changedCards)
    //     {
    //         switch (type)
    //         {
    //             case ChangeType.Add:
    //                 if (_cardItems.ContainsKey(card.Config.Id))
    //                 {
    //                     // DiscardCard(card.Config.Id, _battleWindow.DrawPileTrans.position).Forget();
    //                 }
    //                 Debug.Log($"手牌新增 {card.Config.Id}");
    //                 break;
    //             case ChangeType.Remove:
    //                 if (_cardItems.ContainsKey(card.Config.Id))
    //                 {
    //                     DiscardCard(card.Config.Id, _battleWindow.DiscardPileTrans.position).Forget();
    //                 }
    //                 Debug.Log($"手牌移除 {card.Config.Id}");
    //                 break;
    //             case ChangeType.Refresh:
    //                 Debug.Log($"手牌刷新 {card.Config.Id}");
    //                 break;
    //         }

    //     }
    //     // 清除现有
    //     foreach (var item in _cardItems.Values)
    //     {
    //         ReturnCard(item);
    //     }
    //     _cardItems.Clear();

    //     // 重新生成
    //     foreach (var card in handCards)
    //     {
    //         var go = _poolService.GetGameObject(_cardPoolKey);
    //         go.SetActive(true);
    //         go.transform.SetParent(_handContainer);
    //         var uiCard = go.GetComponent<UICardItem>();
    //         uiCard.Init(card, _cardDetailTrans, onPlay, onCancel, onDragStart, onCardDrag);
    //         _cardItems.Add(card.CardId, uiCard);
    //     }
    //     _fanLayout.Refresh();
    //     foreach (var item in _cardItems.Values)
    //     {
    //         item.OnLayoutComplete();
    //     }
    // }
    // private void ReturnCard(UICardItem item)
    // {
    //     item.gameObject.SetActive(false);
    //     item.transform.localScale = Vector3.one;
    //     item.transform.localRotation = Quaternion.identity;
    //     _cardItems.Remove(item.CardId);
    //     _poolService.ReturnGameObject(_cardPoolKey, item.gameObject);

    // }
    // public async UniTask DiscardCard(int cardId, Vector2 targetPos = default)
    // {
    //     if (_cardItems.TryGetValue(cardId, out var cardItem))
    //     {
    //         ReturnCard(cardItem);
    //     }
    //     else
    //     {
    //         Debug.LogWarning($"手牌中未找到卡牌 {cardId}");
    //         return;
    //     }
    //     var flyFx = _poolService.GetGameObject(_orbPoolKey).GetComponent<CardFlyFx>();
    //     flyFx.gameObject.SetActive(true);
    //     try
    //     {
    //         await flyFx.FlyToTarget(transform.position, targetPos, transform, 0.4f);
    //         Debug.Log("光点已抵达弃牌堆");
    //     }
    //     finally
    //     {

    //         _poolService.ReturnGameObject(_orbPoolKey, flyFx.gameObject);
    //     }
    // }
    // ---- 外部调用接口 ----
    public void RefreshHand(
        List<Card> handCards,
        List<Card> changedCards,
        ChangeType type,
        Action onPlay = null,
        Action onCancel = null,
        Action<int> onDragStart = null,
        Action<Enemy> onCardDrag = null)
    {
        Debug.Log("刷新手牌" + type);
        switch (type)
        {
            case ChangeType.Remove:
                // ★ 第一步：先把“要消失的卡”从手牌容器里抢出来，移到顶层画布
                List<UICardItem> removedItems = new List<UICardItem>();
                Debug.Log($"手牌移除 {changedCards.Count} 张");
                // ★ 获取要删除的卡牌
                foreach (var card in changedCards)
                {
                    if (_cardItems.TryGetValue(card.InstanceId, out var item))
                    {
                        _cardItems.Remove(card.InstanceId);
                        // 移出容器，放到顶层画布（这样UI刷新时不会动它）
                        item.transform.SetParent(transform.parent, worldPositionStays: true);
                        removedItems.Add(item);
                    }
                }
                _fanLayout.Refresh();
                // // ★ 第二步：立即重建UI（手牌容器已经清掉了要删除的卡）
                // RebuildUI(handCards, onPlay, onCancel, onDragStart, onCardDrag);

                // ★ 第三步：播放飞行光点动画（不阻塞UI刷新）
                PlayDiscardAnimations(removedItems);
                break;

            case ChangeType.Add:
                RebuildUI(handCards, onPlay, onCancel, onDragStart, onCardDrag);
                PlayAddAnimations(changedCards);
                break;

            case ChangeType.Refresh:
                // 无动画，直接全量刷新
                RebuildUI(handCards, onPlay, onCancel, onDragStart, onCardDrag);
                break;
        }
    }

    // ---- 重建UI（纯同步，不碰动画） ----
    private void RebuildUI(List<Card> handCards, Action onPlay, Action onCancel, Action<int> onDragStart, Action<Enemy> onCardDrag)
    {
        Debug.Log("手牌数" + handCards.Count);
        ClearHandContainer();
        Debug.Log("_handContainer" + _handContainer.childCount);
        // 清空字典（但不要动 _animatingItems）
        _cardItems.Clear();

        // 生成新卡
        foreach (var card in handCards)
        {
            var go = _poolService.GetGameObject(_cardPoolKey);
            go.SetActive(true);
            go.transform.SetParent(_handContainer);
            var uiCard = go.GetComponent<UICardItem>();
            uiCard.Init(card, _cardDetailTrans, onPlay, onCancel, onDragStart, onCardDrag);
            _cardItems.Add(card.InstanceId, uiCard);
        }

        _fanLayout.Refresh();
        foreach (var item in _cardItems.Values)
        {
            item.OnLayoutComplete();
        }
    }

    // ---- 播放移除动画（光点飞行） ----
    private async void PlayDiscardAnimations(List<UICardItem> cards)
    {
        if (cards.Count == 0) return;
        _flyTasks.Clear();
        _flyObjs.Clear();
        foreach (var item in cards)
        {
            await item.transform.DOScale(Vector3.zero, 0.1f);
            // 从对象池取光点
            var flyFx = _poolService.GetGameObject(_orbPoolKey).GetComponent<CardFlyFx>();
            flyFx.gameObject.SetActive(true);
            _flyObjs.Add(flyFx.gameObject);
            // 启动飞行
            _flyTasks.Add(flyFx.FlyToTarget(item.transform.position, _battleWindow.DiscardPileTrans.position, transform.parent, 0.2f));
        }

        // 等待所有光点飞完
        await UniTask.WhenAll(_flyTasks);

        // 所有光点飞完后，回收卡牌视图和光点
        foreach (var item in cards)
        {
            // 回收卡牌视图到池
            ReturnCardToPool(item);
        }
        foreach (var flyObj in _flyObjs)
        {
            _poolService.ReturnGameObject(_orbPoolKey, flyObj);
        }
    }

    // ---- 播放新增动画 ----
    private async void PlayAddAnimations(List<Card> changedCards)
    {
        Debug.Log("添加手牌数" + changedCards.Count);
        _flyTasks.Clear();
        _flyObjs.Clear();
        List<UICardItem> newItems = new List<UICardItem>();
        // 从手牌容器里找到新增的卡，播放飞入动画
        foreach (var card in changedCards)
        {
            if (_cardItems.TryGetValue(card.InstanceId, out var item))
            {
                item.gameObject.SetActive(false);
                item.transform.localScale = Vector3.zero;
                var flyFx = _poolService.GetGameObject(_orbPoolKey).GetComponent<CardFlyFx>();
                flyFx.gameObject.SetActive(true);
                _flyObjs.Add(flyFx.gameObject);
                newItems.Add(item);
                // 启动飞行
                _flyTasks.Add(flyFx.FlyToTarget(_battleWindow.DrawPileTrans.position, item.transform.position, transform.parent, 0.2f));
            }
        }
        await UniTask.WhenAll(_flyTasks);
        foreach (var item in newItems)
        {
            item.transform.DOScale(Vector3.one, 0.1f);
            item.gameObject.SetActive(true);
        }
        foreach (var flyObj in _flyObjs)
        {
            _poolService.ReturnGameObject(_orbPoolKey, flyObj);
        }
    }
    private void ClearHandContainer()
    {
        // 1. 先收集所有子物体
        List<UICardItem> itemsToRemove = new List<UICardItem>();
        foreach (Transform child in _handContainer)
        {
            var item = child.GetComponent<UICardItem>();
            if (item != null)
            {
                itemsToRemove.Add(item);
            }
            else
            {
                // 非 UICardItem 的直接销毁
                Destroy(child.gameObject);
            }
        }

        // 2. 统一处理收集到的卡
        foreach (var item in itemsToRemove)
        {
            _cardItems.Remove(item.InstanceId);
            ReturnCardToPool(item); // 确保移出容器
        }

        // 3. 清空字典（双保险）
        _cardItems.Clear();
    }
    // ---- 回收卡牌到对象池 ----
    private void ReturnCardToPool(UICardItem item)
    {
        if (item == null) return;
        item.gameObject.SetActive(false);
        item.transform.localScale = Vector3.one;
        item.transform.localRotation = Quaternion.identity;
        // 注意：这里不要 Remove 字典，由调用方决定何时移除
        _poolService.ReturnGameObject(_cardPoolKey, item.gameObject);
    }
    /// <summary>
    ///  根据现有行动点刷新手牌状态
    /// </summary>
    /// <param name="currentEnergy"></param>
    public void RefreshHandState(int currentEnergy)
    {
        foreach (var item in _cardItems.Values)
        {
            item.RefreshState(currentEnergy);
        }
    }

    public void ResetCard()
    {
        foreach (var item in _cardItems.Values)
        {
            item.ResetCard();
        }
        _fanLayout.Refresh();
    }
}