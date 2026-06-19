using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LitFramework.EventBus;
using UnityEngine;

public class Card
{
    public CardConfig Config { get; private set; }
    public int CurrentCost { get; set; }  // 临时费用变化（如被减费）
    private List<ICardEffect> _effects;
    public bool IsPlayable { get; set; }
    public Card(CardConfig config)
    {
        Config = config;
        CurrentCost = config.Cost;
        // 预创建效果对象（工厂）
        _effects = config.Effects.Select(CardEffectFactory.Create).ToList();
    }

    public void Play(BattleContext context)
    {
        // 检查能量、触发前置事件等
        foreach (var effect in _effects)
            effect.Execute(this, context);
        // 后处理：触发卡牌打出事件、移入手牌到弃牌堆等
        EventBus<CardPlayedEvent>.Publish(new CardPlayedEvent { Card = this });
    }
}
