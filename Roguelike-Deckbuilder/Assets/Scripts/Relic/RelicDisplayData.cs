using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitFramework;
using LitFramework.Asset;
using UnityEngine;

public class RelicDisplayData
{
    // 同一图标只加载一次，避免每次刷新 TopBar/商店都增加引用计数
    private static readonly Dictionary<string, AssetRef<Sprite>> _iconRefs = new();

    public int Id;
    public Sprite Icon;
    public int Price;
    public string Description;
    public static async UniTask<RelicDisplayData> FromConfig(RelicConfig config, int price = 0)
    {
        var path = $"Assets/Res/Art/Relics/{config.Icon}.png";
        if (!_iconRefs.TryGetValue(path, out var iconRef))
        {
            var assetService = ServiceLocator.Get<IAssetService>();
            iconRef = await assetService.LoadRefAsync<Sprite>(path);
            _iconRefs[path] = iconRef;
        }

        return new RelicDisplayData
        {
            Id = config.Id,
            Icon = iconRef.Asset,
            Price = price,
            Description = config.Description
        };
    }
}
