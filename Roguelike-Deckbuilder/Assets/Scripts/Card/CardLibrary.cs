using System;
using System.Collections.Generic;
using LitFramework;
using LitFramework.Config;
using UnityEngine;
using System.Linq;
public class CardLibrary : ICardLibrary
{
    private Dictionary<int, CardConfig> _cardConfigs;
    private List<CardEffectsConfig> _cardEffects;
    public void OnRegister()
    {
        var configService = ServiceLocator.Get<IConfigService>();
        var tableCard = configService.GetTable<CardConfig>() as DictConfigTable<CardConfig>;
        _cardConfigs = tableCard.GetDict();
        foreach (var item in _cardConfigs)
        {
            Debug.Log($"卡牌: {item.Key}");
        }
        var tableEffect = configService.GetTable<CardEffectsConfig>() as ListConfigTable<CardEffectsConfig>;
        _cardEffects = tableEffect.GetList();
        foreach (var effect in _cardEffects)
        {
            if (_cardConfigs.TryGetValue(effect.Id, out var cardConfig))
            {
                cardConfig.Effects.Add(effect);
                Debug.Log($"卡牌{cardConfig.Id} 绑定效果: {effect.Id} {effect.Type} {effect.Value} {effect.Target} ");
            }
            else
            {
                Debug.LogWarning($"效果 {effect.Id} 对应的卡牌不存在");
            }
        }
    }

    public Card CreateCard(int cardId)
    {
        if (_cardConfigs.TryGetValue(cardId, out var config))
            return new Card(config);
        throw new Exception($"未找到卡牌: {cardId}");
    }


}