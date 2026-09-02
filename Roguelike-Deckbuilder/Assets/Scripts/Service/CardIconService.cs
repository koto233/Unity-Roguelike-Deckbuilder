using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitFramework;
using LitFramework.Asset;
using UnityEngine;

public class CardIconService
{
    // 卡牌图缓存（懒加载）
    private const string CardAtlasPath = "Assets/Res/Art/CardAtlas/";
    private readonly Dictionary<string, Sprite> _cardIconCache = new();

    /// <summary>
    /// 预加载所有卡图（启动时调用一次）
    /// </summary>
    public async UniTask PreLoadCardIcons()
    {
        if (_cardIconCache.Count > 0)
        {
            Debug.Log("卡图已预加载，无需重复加载。");
            return;
        }
        var assetService = ServiceLocator.Get<IAssetService>();
        var paths = new[] { "card_atlas_0", "card_atlas_1", "card_atlas_2" };
        foreach (var path in paths)
        {
            var handle = await assetService.LoadSubAssetsAsync<Sprite>(CardAtlasPath + path);
            var allSprites = handle.GetSubAssetObjects<Sprite>();
            foreach (Sprite sprite in allSprites)
            {
                if (!_cardIconCache.ContainsKey(sprite.name))
                    _cardIconCache.Add(sprite.name, sprite);
            }
            handle.Release();
        }
        Debug.Log($"卡图预加载完成，共 {_cardIconCache.Count} 张卡图。");
    }

    /// <summary>
    /// 获取卡图 Sprite
    /// </summary>
    public Sprite GetCardIcon(string spriteName)
    {
        // Debug.Log($"尝试获取卡图: {spriteName}");
        if (_cardIconCache.TryGetValue(spriteName, out var sprite))
            return sprite;

        Debug.LogWarning($"未找到卡图 Sprite: {spriteName}");
        return null;
    }

}