using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LitFramework;
using LitFramework.Config;
using LitFramework.EventBus;
using Newtonsoft.Json;
using UnityEngine;

public class Card
{
    private static int _nextId = 1;
    /// <summary>
    /// 实例Id
    /// </summary>
    /// <value></value>
    public int InstanceId { get; private set; }

    /// <summary>
    /// 卡牌配置
    /// </summary> 
    public CardConfig Config { get; private set; }
    /// <summary>
    /// 临时费用
    /// </summary>
    public int CurrentCost { get; private set; }
    /// <summary>
    /// 是否需要目标
    /// </summary> 
    public bool NeedTarget { get; set; }
    public string Description { get; private set; }
    public bool CanUse { get; set; }
    /// <summary>
    /// 效果实例
    /// </summary>
    public List<CardEffectBase> EffectsInstance { get; private set; } = new();
    public Card(CardConfig config)
    {
        InstanceId = _nextId++;
        NeedTarget = false;
        Config = config;
        CurrentCost = config.Cost;
        var configService = ServiceLocator.Get<IConfigService>();
        var effectsConfigTable = configService.GetTable<CardEffectsConfig>();
        StringBuilder sb = new StringBuilder();
        foreach (var effect in config.Effects)
        {

            var effectConfig = effectsConfigTable.GetById(effect.EffectId) as CardEffectsConfig;
            Debug.Log($"生成效果{JsonConvert.SerializeObject(effect)} {JsonConvert.SerializeObject(effectConfig)}");
            var effectInstance = CardEffectFactory.Create(effectConfig, effect.Value);
            EffectsInstance.Add(effectInstance);
            if (effectConfig.Target == "Enemy")
            {
                NeedTarget = true;
            }
            sb.AppendLine(string.Format(effectConfig.Description, effect.Value));
        }
        Description = sb.ToString();
    }
}
