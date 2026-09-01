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
    private UIAtlasService _uiAtlasService;
    private CardIconService _cardIconService;
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
            cardDisplayDataList.Add(CardDisplayData.FromConfig(config));
        }
        View.SpawnCardInList(cardDisplayDataList);
    }

    private void SubscribeEvents()
    {
        View.OnClickBack += ClickBack;
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
