using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandZone : MonoBehaviour
{
    public Transform handContainer;
    public GameObject cardPrefab;
    
    private List<UICardItem> _cardItems = new List<UICardItem>();
    private BattleContext _battleContext;
    
    void OnEnable()
    {
        // EventBus<HandChangedEvent>.Register(OnHandChanged);
        // // 获取战斗上下文（假设由BattleController提供）
        // _battleContext = ServiceLocator.Get<IBattleController>().GetContext();
        // RefreshHand(_battleContext.Player.Hand);
    }
    
    void OnDisable()
    {
        // EventBus<HandChangedEvent>.Unregister(OnHandChanged);
    }
    
    // private void OnHandChanged(HandChangedEvent evt)
    // {
    //     RefreshHand(evt.NewHand);
    // }
    
    // private void RefreshHand(List<CardRuntime> handCards)
    // {
    //     // 清除现有
    //     foreach (var item in _cardItems)
    //         Destroy(item.gameObject);
    //     _cardItems.Clear();
        
    //     // 重新生成
    //     foreach (var card in handCards)
    //     {
    //         var go = Instantiate(cardPrefab, handContainer);
    //         var uiCard = go.GetComponent<UICardItem>();
    //         uiCard.SetCard(card, _battleContext, (playedCard) =>
    //         {
    //             // 出牌回调 -> 通知 BattleController
    //             var battle = ServiceLocator.Get<IBattleController>();
    //             battle.PlayCard(playedCard);
    //         });
    //         _cardItems.Add(uiCard);
    //     }
    //     // 自动布局（使用HorizontalLayoutGroup或GridLayoutGroup）
    // }
}