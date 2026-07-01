using System.Collections;
using System.Collections.Generic;
using LitFramework.EventBus;
using UnityEngine;

public class Player : CharacterBase
{
    public List<Card> DrawPile { get; private set; }      // 抽牌堆
    public List<Card> Hand { get; private set; }          // 手牌
    public List<Card> DiscardPile { get; private set; }   // 弃牌堆
    private int _energy;
    private int _maxEnergy;
    public int Energy => _energy;
    public int MaxEnergy => _maxEnergy;
    protected override EntityType EntityType => EntityType.Player;

    public override int Id => -1;

    public Player(int maxHp, int maxEnergy) : base(maxHp)
    {
        _maxEnergy = maxEnergy;
        _energy = maxEnergy;
        Hand = new();
        DrawPile = new();
        DiscardPile = new();
    }
    /// <summary>
    /// 抽牌，不会重置牌堆，没有则不抽
    /// </summary>
    /// <param name="count"></param>
    public void DrawCards(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (DrawPile.Count > 0)
            {
                var card = DrawPile[0];
                DrawPile.RemoveAt(0);
                Hand.Add(card);
            }
        }
        EventBus<HandChangedEvent>.Publish(new HandChangedEvent() { Cards = Hand });
    }
    /// <summary>
    /// 回合开始时抽牌 若牌堆为空 则从重置牌堆
    /// </summary>
    /// <param name="count"></param>
    public void DrawCardInTurnStart(int count)
    {
        for (int i = 0; i < count; i++)
        {
            CheckResetPile();
            DrawCards(1);
        }
    }
    /// <summary>
    /// 检查并重置牌堆   
    /// </summary>
    public void CheckResetPile()
    {
        if (DrawPile.Count == 0)
        {
            DrawPile.AddRange(DiscardPile);
            ShuffleDrawPile();
            DiscardPile.Clear();
        }
    }
    /// <summary>
    /// 丢弃指定牌
    /// </summary>
    public void DiscardCard(Card card)
    {
        Hand.Remove(card);
        DiscardPile.Add(card);
        EventBus<HandChangedEvent>.Publish(new HandChangedEvent() { Cards = Hand });
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
        for (int i = Hand.Count - 1; i >= 0; i--)
        {
            var card = Hand[i];
            Hand.RemoveAt(i);
            DiscardPile.Add(card);
        }
        EventBus<HandChangedEvent>.Publish(new HandChangedEvent() { Cards = Hand });
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
    public void ShuffleDrawPile()
    {
        ShuffleWithUnityRandom(DrawPile);
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
