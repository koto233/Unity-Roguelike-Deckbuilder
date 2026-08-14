using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitFramework;
using LitFramework.Asset;
using UnityEngine;

public class UIAtlasService
{
    // 卡牌图缓存（懒加载）
    private const string CardAtlasPath = "Assets/Res/Art/UIAtlas/";
    private readonly Dictionary<string, Sprite> _uiSpriteCache = new();

    /// <summary>
    /// 预加载所有UI图集（启动时调用一次）
    /// </summary>
    public async UniTask PreLoadCardIcons()
    {
        if (_uiSpriteCache.Count > 0)
        {
            Debug.Log("UI图集已预加载，无需重复加载。");
            return;
        }
        var assetService = ServiceLocator.Get<IAssetService>();
        var paths = new[] { "ui_atlas_0", "ui_atlas_1" };
        foreach (var path in paths)
        {
            var handle = await assetService.LoadSubAssetsAsync<Sprite>(CardAtlasPath + path);
            var allSprites = handle.GetSubAssetObjects<Sprite>();
            foreach (Sprite sprite in allSprites)
            {
                if (!_uiSpriteCache.ContainsKey(sprite.name))
                    _uiSpriteCache.Add(sprite.name, sprite);
            }
            handle.Release();
        }
        Debug.Log($"UI图集预加载完成，共 {_uiSpriteCache.Count} 张UI图。");
    }

    /// <summary>
    /// 获取UI图集 Sprite
    /// </summary>
    public Sprite GetSprite(string spriteName)
    {
        Debug.Log($"尝试获取UI图集: {spriteName}");
        if (_uiSpriteCache.TryGetValue(spriteName, out var sprite))
            return sprite;

        Debug.LogWarning($"未找到UI图集 Sprite: {spriteName}");
        return null;
    }

}