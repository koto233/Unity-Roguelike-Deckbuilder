using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LitFramework;
using LitFramework.Config;
using LitFramework.EventBus;
using UnityEngine;

public class Card
{
    /// <summary>
    /// 卡牌配置
    /// </summary> 
    public CardConfig Config { get; private set; }
    /// <summary>
    /// 临时费用
    /// </summary>
    public int CurrentCost { get; set; }
    /// <summary>
    /// 效果实例
    /// </summary>
    public List<CardEffectBase> EffectsInstance { get; private set; }
    public Card(CardConfig config)
    {
        Config = config;
        CurrentCost = config.Cost;
        var configService = ServiceLocator.Get<ConfigService>();
        var effectsConfigTable = configService.GetTable<CardEffectsConfig>();
        foreach (var effect in config.Effects)
        {
            var effectConfig = effectsConfigTable.GetById(effect.Id) as CardEffectsConfig;
            var effectInstance = CardEffectFactory.Create(effectConfig, effect.Value);
            EffectsInstance.Add(effectInstance);
        }
    }
}
