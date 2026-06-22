using System;
using System.Collections.Generic;
using LitFramework;
using LitFramework.Config;
using UnityEngine;
using System.Linq;
public class CardLibrary : ICardLibrary
{
    private Dictionary<string, CardConfig> _cardConfigs;
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
            if (_cardConfigs.TryGetValue(effect.ID, out var cardConfig))
            {
                cardConfig.Effects.Add(effect);
            }
            else
            {
                Debug.LogWarning($"效果 {effect.ID} 对应的卡牌不存在");
            }
        }
    }

    public Card CreateCard(string cardId)
    {
        if (_cardConfigs.TryGetValue(cardId, out var config))
            return new Card(config);
        throw new Exception($"未找到卡牌: {cardId}");
    }

    public Card CreateRandomCard()
    {
        // 1.取出字典中所有的英文ID（Keys）
        var keys = _cardConfigs.Keys.ToList();

        // 2. 随机取一个英文ID
        string randomCardId = keys[new System.Random().Next(keys.Count)];

        // 3. 传入英文ID创建卡牌
        return CreateCard(randomCardId);
    }


}