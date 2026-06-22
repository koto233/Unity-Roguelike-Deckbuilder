using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LitFramework.EventBus;
using UnityEngine;

public class Card
{
    public CardConfig Config { get; private set; }
    public int CurrentCost { get; set; }  // 临时费用变化（如被减费）
    public List<ICardEffect> Effects { get; private set; }
    public Card(CardConfig config)
    {
        Config = config;
        CurrentCost = config.Cost;
        // 预创建效果对象（工厂）
        Effects = config.Effects.Select(CardEffectFactory.Create).ToList();
    }
}
