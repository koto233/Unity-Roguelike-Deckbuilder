using System.Collections;
using System.Collections.Generic;
using LitFramework;
using UnityEngine;

public class RelicDisplayData
{
    public int Id;
    public Sprite Icon;
    public int Price;
    public static RelicDisplayData FromConfig(RelicConfig config, int price = 0)
    {
        return new RelicDisplayData
        {
            Id = config.Id,
            Icon = ServiceLocator.Get<CardIconService>().GetCardIcon(config.Icon),
            Price = price
        };
    }
}
