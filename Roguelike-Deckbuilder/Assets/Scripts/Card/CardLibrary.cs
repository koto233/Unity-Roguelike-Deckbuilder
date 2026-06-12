using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using LitFramework;
using LitFramework.Config;
using UnityEngine;

public class CardLibrary : ICardLibrary
{
    private Dictionary<string, CardConfig> _configMap;

    public void OnRegister()
    {
        var table = ServiceLocator.Get<IConfigService>().GetTable<CardConfig>();
        _configMap = table.GetAll().ToDictionary(x => x.Id.ToString(), x => x);
        
    }

    public Card CreateCard(string cardId)
    {
        if (_configMap.TryGetValue(cardId, out var config))
            return new Card(config);
        throw new Exception($"未找到卡牌: {cardId}");
    }

    public Card CreateRandomCard()
    {
        return CreateCard(new System.Random().Next(_configMap.Count).ToString());
    }


}