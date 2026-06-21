using System.Collections;
using System.Collections.Generic;
using LitFramework.EventBus;
using UnityEngine;

public class PlayerData : CharacterData
{
    public List<Card> DrawPile { get; private set; }      // 抽牌堆
    public List<Card> Hand { get; private set; }          // 手牌
    public List<Card> DiscardPile { get; private set; }   // 弃牌堆
    private int _energy;
    private int _maxEnergy;
    public int Energy => _energy;
    public int MaxEnergy => _maxEnergy;
    public PlayerData(int maxHp, int maxEnergy, int strength = 0) : base(maxHp, strength)
    {
        _maxEnergy = maxEnergy;
        _energy = maxEnergy;
        Hand = new();
        DrawPile = new();
        DiscardPile = new();
    }
    /// <summary>
    /// 抽牌
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
    /// 使用牌
    /// </summary>
    /// <returns></returns>
    public bool PlayCard()
    {
        return true;
    }
    /// <summary>
    /// 弃牌
    /// </summary>
    public void DiscardAllHand()
    {

    }
    public bool SpendEnergy(int cost)
    {
        if (cost <= 0 || Energy < cost) return false;
        _energy -= cost;
        EventBus<EnergyChangedEvent>.Publish(new EnergyChangedEvent { OldEnergy = Energy, NewEnergy = Energy - cost });
        return true;
    }
    public void ShuffleDrawPile()
    {

    }
    public void StartTurn(int baseEnergy = 3)
    {

    }
}
