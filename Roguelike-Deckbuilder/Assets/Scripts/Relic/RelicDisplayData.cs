using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitFramework;
using LitFramework.Asset;
using UnityEngine;

public class RelicDisplayData
{
    public int Id;
    public Sprite Icon;
    public int Price;
    public string Description;
    public static async UniTask<RelicDisplayData> FromConfig(RelicConfig config, int price = 0)
    {
        var assetService = ServiceLocator.Get<IAssetService>();
        var asset = await assetService.LoadRefAsync<Sprite>($"Assets/Res/Art/Relics/{config.Icon}.png");
        return new RelicDisplayData
        {
            Id = config.Id,
            Icon = asset.Asset,
            Price = price,
            Description = config.Description
        };
    }
}
