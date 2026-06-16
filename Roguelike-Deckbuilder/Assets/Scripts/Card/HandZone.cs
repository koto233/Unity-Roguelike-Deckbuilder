using System.Collections;
using System.Collections.Generic;
using LitFramework;
using LitFramework.Asset;
using LitFramework.EventBus;
using UnityEngine;

public class HandZone : MonoBehaviour
{
    public Transform handContainer;
    private GameObject _cardPrefab;
    private List<UICardItem> _cardItems = new ();
    private BattleContext _battleContext;
    private IEventBinding<HandChangedEvent> m_HandChangedEventBinding;
    void Awake()
    {
        var assetService = ServiceLocator.Get<IAssetService>();
        assetService.LoadAsync<GameObject>("Assets/Res/UI/UICardItem.prefab", prefab =>
        {
            _cardPrefab = prefab;
            Debug.Log("Loaded card prefab: " + prefab.name);
        });
    }

    void OnEnable()
    {
        m_HandChangedEventBinding = new EventBinding<HandChangedEvent>(OnHandChanged);
        EventBus<HandChangedEvent>.Subscribe(m_HandChangedEventBinding);
        // // 获取战斗上下文（假设由BattleController提供）
        // _battleContext = ServiceLocator.Get<IBattleController>().GetContext();
        RefreshHand(_battleContext.Player.Hand);
    }

    void OnDisable()
    {
        EventBus<HandChangedEvent>.Unsubscribe(m_HandChangedEventBinding);
    }

    private void OnHandChanged(HandChangedEvent evt)
    {
        RefreshHand(evt.Cards);
    }

    private void RefreshHand(List<Card> handCards)
    {
        // 清除现有
        foreach (var item in _cardItems)
            Destroy(item.gameObject);
        _cardItems.Clear();

        // 重新生成
        foreach (var card in handCards)
        {
            var go = Instantiate(_cardPrefab, handContainer);
            var uiCard = go.GetComponent<UICardItem>();
            uiCard.SetCard(card, _battleContext, (playedCard) =>
            {
                var battle = ServiceLocator.Get<IBattleController>();
                battle.PlayCard(playedCard);
            });
            _cardItems.Add(uiCard);
        }
        // 自动布局（使用HorizontalLayoutGroup或GridLayoutGroup）
    }
}