using System.Collections;
using System.Collections.Generic;
using LitFramework;
using LitFramework.Asset;
using LitFramework.EventBus;
using UnityEngine;

public class UIHandZone : MonoBehaviour
{
    public Transform handContainer;
    private GameObject _cardPrefab;
    private List<UICardItem> _cardItems = new();
    private BattleContext _battleContext;
    public void Init(GameObject cardPrefab, BattleContext battleContext)
    {
        _battleContext = battleContext;
        _cardPrefab = cardPrefab;
    }

    public void RefreshHand(List<Card> handCards)
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