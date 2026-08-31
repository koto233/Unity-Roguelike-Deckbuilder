using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LitFramework;
using LitFramework.Config;
using LitFramework.UI.Core.Service;

public class DeckPresenter : BasePresenter<DeckView>
{
    private IConfigService _configService;
private UIAtlasService _uiAtlasService;
    private CardIconService _cardIconService;
    private StringBuilder _description = new();
    public DeckPresenter(DeckView view) : base(view)
    {
    }

    public override void Init()
    {

        SubscribeEvents();
        var globalData = ServiceLocator.Get<PlayerDataService>();
        var cardTable = ServiceLocator.Get<IConfigService>().GetTable<CardConfig>();
        var cardDisplayDataList = new List<CardDisplayData>();
        _uiAtlasService = ServiceLocator.Get<UIAtlasService>();
        _cardIconService = ServiceLocator.Get<CardIconService>();
        foreach (var cardId in globalData.DeckCardIds)
        {
            var config = cardTable.Get(cardId);
            cardDisplayDataList.Add(ToDisplayData(config));
        }
        View.SpawnCardInList(cardDisplayDataList);
    }

    private void SubscribeEvents()
    {
        View.OnClickBack += ClickBack;
    }

    private CardDisplayData ToDisplayData(CardConfig config)
    {
        _description.Clear();
        foreach (var effect in config.Effects)
        {
            var effectConfig = _configService.GetTable<CardEffectsConfig>().Get(effect.EffectId);
            _description.AppendLine(string.Format(effectConfig.Description, effect.Value));
        }

        return new CardDisplayData
        {
            Name = config.Name,
            Description = _description.ToString(),
            EnergyCost = config.Cost,
            Icon = _cardIconService.GetCardIcon(config.Icon),
            PortraitBorderSprite = _uiAtlasService.GetSprite($"card_portrait_border_{config.Type}_s"),
            FrameSprite = _uiAtlasService.GetSprite($"card_frame_{config.Type}_s"),
            Type = config.Type == "attack" ? "攻击" : "技能",
        };
    }

    private void UnsubscribeEvents()
    {
        View.OnClickBack -= ClickBack;
    }
    private void ClickBack()
    {
        ServiceLocator.Get<UIService>().Close<DeckView>();
    }
    public override void Dispose()
    {
        UnsubscribeEvents();
        base.Dispose();
    }
}
