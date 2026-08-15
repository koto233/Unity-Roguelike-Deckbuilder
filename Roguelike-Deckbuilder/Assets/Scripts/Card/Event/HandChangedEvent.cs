using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LitFramework.EventBus;


public struct HandChangedEvent : IEvent
{
    /// <summary>
    /// 变化的卡牌
    /// </summary>
    public IReadOnlyList<Card> ChangedCards;
    public IReadOnlyList<Card> Cards;
    public ChangeType Type;

}
public enum ChangeType
{
    Add,        // 抽牌
    Remove,     // 弃牌
    Refresh       // 其他变化（如使用卡牌）
}