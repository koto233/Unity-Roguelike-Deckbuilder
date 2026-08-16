using System.Collections;
using System.Collections.Generic;
using LitFramework.EventBus;
using UnityEngine;

public class Player : CharacterBase
{
    private List<Card> _drawPile = new();    // 抽牌堆
    private List<Card> _hand = new();         // 手牌
    private List<Card> _discardPile = new();  // 弃牌堆
    public IReadOnlyList<Card> DrawPile => _drawPile;
    public IReadOnlyList<Card> Hand => _hand;
    public IReadOnlyList<Card> DiscardPile => _discardPile;
    public int DrawPileCount => DrawPile.Count;
    public int DiscardPileCount => DiscardPile.Count;
    private int _energy;
    private int _maxEnergy;
    public int Energy => _energy;
    public int MaxEnergy => _maxEnergy;
    protected override EntityType EntityType => EntityType.Player;

    public override int Id => -1;

    public Player(int currentHp, int maxHp, int maxEnergy) : base(maxHp)
    {
        _currentHp = currentHp;
        _maxEnergy = maxEnergy;
        _energy = maxEnergy;
    }
    public void AddCardToDrawPile(Card card)
    {
        _drawPile.Add(card);
        EventBus<DrawPileChangedEvent>.Publish(new DrawPileChangedEvent() { CurrentCount = DrawPile.Count });
    }
    public void AddCardToHand(Card card)
    {
        _hand.Add(card);
        EventBus<HandChangedEvent>.Publish(new HandChangedEvent() { ChangedCards = new List<Card> { card }, Cards = Hand, Type = ChangeType.Add });
    }
    public void AddCardToDiscardPile(Card card)
    {
        _discardPile.Add(card);
        EventBus<DiscardPileChangedEvent>.Publish(new DiscardPileChangedEvent() { CurrentCount = DiscardPile.Count });
    }
    public void RemoveCardFromDrawPile(Card card)
    {
        _discardPile.Remove(card);
        EventBus<DiscardPileChangedEvent>.Publish(new DiscardPileChangedEvent() { CurrentCount = DiscardPile.Count });

    }
    public void RemoveCardFromHand(Card card)
    {
        _hand.Remove(card);
        EventBus<HandChangedEvent>.Publish(new HandChangedEvent() { ChangedCards = new List<Card> { card }, Cards = Hand, Type = ChangeType.Remove });
    }
    public void RemoveCardFromDiscardPile(Card card)
    {
        _discardPile.Remove(card);
        EventBus<DiscardPileChangedEvent>.Publish(new DiscardPileChangedEvent() { CurrentCount = DiscardPile.Count });
    }
    /// <summary>
    /// 抽牌，不会重置牌堆，没有则不抽
    /// </summary>
    /// <param name="count"></param>
    public void DrawCards(int count, bool ignore = true)
    {
        List<Card> drawnCards = new List<Card>();
        for (int i = 0; i < count; i++)
        {
            if (DrawPile.Count == 0) break;
            Card card = DrawPile[0];
            _drawPile.RemoveAt(0);
            _hand.Add(card);
            drawnCards.Add(card);
            EventBus<DrawPileChangedEvent>.Publish(new DrawPileChangedEvent() { CurrentCount = DrawPile.Count });
        }
        if (drawnCards.Count > 0)
        {
            EventBus<HandChangedEvent>.Publish(new HandChangedEvent()
            {
                Cards = Hand,
                ChangedCards = drawnCards,  // ← 所有抽到的卡
                Type = ChangeType.Add
            });
        }
    }

    /// <summary>
    /// 回合开始时抽牌 若牌堆为空 则从重置牌堆
    /// </summary>
    /// <param name="count"></param>
    public void DrawCardInTurnStart(int count)
    {
        var changedCards = new List<Card>();
        for (int i = 0; i < count; i++)
        {

            CheckResetPile();
            if (DrawPile.Count > 0)
            {
                var card = DrawPile[0];
                _drawPile.RemoveAt(0);
                _hand.Add(card);
                changedCards.Add(card);
                EventBus<DrawPileChangedEvent>.Publish(new DrawPileChangedEvent() { CurrentCount = DrawPile.Count });
            }
        }
        EventBus<HandChangedEvent>.Publish(new HandChangedEvent() { Cards = Hand, ChangedCards = changedCards, Type = ChangeType.Add });

    }
    /// <summary>
    /// 检查并重置牌堆   
    /// </summary>
    public void CheckResetPile()
    {
        if (DrawPile.Count == 0)
        {
            _drawPile.AddRange(DiscardPile);
            ShuffleDrawDeck();
            _discardPile.Clear();
            EventBus<DiscardPileChangedEvent>.Publish(new DiscardPileChangedEvent() { CurrentCount = DiscardPile.Count });
            EventBus<DrawPileChangedEvent>.Publish(new DrawPileChangedEvent() { CurrentCount = DrawPile.Count });
        }
    }
    /// <summary>
    /// 丢弃指定牌
    /// </summary>
    public void DiscardCard(Card card)
    {
        _hand.Remove(card);
        _discardPile.Add(card);
        EventBus<DiscardPileChangedEvent>.Publish(new DiscardPileChangedEvent() { CurrentCount = DiscardPile.Count });
        EventBus<HandChangedEvent>.Publish(new HandChangedEvent() { ChangedCards = new List<Card> { card }, Cards = Hand, Type = ChangeType.Remove });
    }
    /// <summary>
    /// 使用牌
    /// </summary>
    /// <returns></returns>
    public bool PlayCard()
    {
        return true;
    }
    /// <summary>
    /// 弃掉所有手牌
    /// </summary>
    public void DiscardAllHand()
    {
        var changedCards = new List<Card>(Hand);
        for (int i = Hand.Count - 1; i >= 0; i--)
        {
            var card = Hand[i];
            _hand.RemoveAt(i);
            _discardPile.Add(card);
        }
        EventBus<DiscardPileChangedEvent>.Publish(new DiscardPileChangedEvent() { CurrentCount = DiscardPile.Count });
        EventBus<HandChangedEvent>.Publish(new HandChangedEvent() { ChangedCards = changedCards, Cards = Hand, Type = ChangeType.Remove });
    }
    public void AddEnergy(int amount)
    {
        _energy += amount;
        EventBus<EnergyChangedEvent>.Publish(new EnergyChangedEvent { CurrentEnergy = _energy, MaxEnergy = _maxEnergy, });
    }
    public bool SpendEnergy(int cost)
    {
        if (cost < 0 || Energy < cost) return false;
        _energy -= cost;
        Debug.Log($"消耗能量：{cost} {Energy}");
        EventBus<EnergyChangedEvent>.Publish(new EnergyChangedEvent { CurrentEnergy = _energy, MaxEnergy = _maxEnergy });
        return true;
    }
    public void ResetEnergy()
    {
        _energy = _maxEnergy;
        EventBus<EnergyChangedEvent>.Publish(new EnergyChangedEvent { CurrentEnergy = _energy, MaxEnergy = _maxEnergy });
    }
    public void ShuffleDrawDeck()
    {
        ShuffleWithUnityRandom(_drawPile);
    }

    public void ShuffleWithUnityRandom<T>(List<T> list)
    {
        int n = list.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);

            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}
