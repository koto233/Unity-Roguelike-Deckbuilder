using System.Collections;
using System.Collections.Generic;
using System.Text;
using LitFramework;
using LitFramework.Config;
using UnityEngine;

public class CardDisplayData
{
    public int Id;
    public string Name;
    public Sprite Icon;
    public Sprite PortraitBorderSprite;
    public Sprite FrameSprite;
    public string Type;
    public string Description;
    public int EnergyCost;
    public int Price;
    public static CardDisplayData FromConfig(CardConfig config, int price = 0)
    {
        var configService = ServiceLocator.Get<IConfigService>();
        var cardIconService = ServiceLocator.Get<CardIconService>();
        var uiAtlasService = ServiceLocator.Get<UIAtlasService>();

        var sb = new StringBuilder();
        if (config.Effects != null)
        {
            var effectTable = configService.GetTable<CardEffectsConfig>();
            foreach (var effect in config.Effects)
            {
                var effectConfig = effectTable.Get(effect.EffectId);
                if (effectConfig != null)
                    sb.AppendLine(string.Format(effectConfig.Description, effect.Value));
            }
        }

        string typeText = config.Type == "Attack" ? "攻击" : "技能";

        return new CardDisplayData
        {
            Id = config.Id,
            Name = config.Name,
            Description = sb.ToString().TrimEnd(),
            EnergyCost = config.Cost,
            Icon = cardIconService.GetCardIcon(config.Icon),
            PortraitBorderSprite = uiAtlasService.GetSprite($"card_portrait_border_{config.Type.ToLower()}_s"),
            FrameSprite = uiAtlasService.GetSprite($"card_frame_{config.Type.ToLower()}_s"),
            Type = typeText,
            Price = price,
        };
    }
}
