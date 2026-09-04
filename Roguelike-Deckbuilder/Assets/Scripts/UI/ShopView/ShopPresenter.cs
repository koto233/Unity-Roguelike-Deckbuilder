using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using LitFramework;
using LitFramework.Config;
using LitFramework.EventBus;
using LitFramework.UI.Core.Service;
using UnityEngine;

public class ShopPresenter : BasePresenter<ShopView>
{
    private UIService _uiService;
    private IConfigService _configService;
    private int _selectedCardId;
    private PlayerDataService _playerService;
    private IConfigTable<ShopConfig> _shopTable;
    private IConfigTable<RelicConfig> _relicTable;
    public Dictionary<int, CardDisplayData> Cards = new();
    public Dictionary<int, RelicDisplayData> Relics = new();
    private int _removeCost = 75;
    public ShopPresenter(ShopView view) : base(view)
    {
    }

    public override void Init()
    {
        _uiService = ServiceLocator.Get<UIService>();
        _playerService = ServiceLocator.Get<PlayerDataService>();
        _configService = ServiceLocator.Get<IConfigService>();
        _shopTable = ServiceLocator.Get<IConfigService>().GetTable<ShopConfig>();
        _relicTable = ServiceLocator.Get<IConfigService>().GetTable<RelicConfig>();
        SubscribeEvents();
        InitShop().Forget();
        _removeCost += _playerService.RemoveCount * 25;
        View.RefreshRemoveCost(_removeCost);
    }
    private void SubscribeEvents()
    {
        View.OnClickContinue += HandleClickContinue;
        View.OnClickRemove += HandleClickRemove;
        View.OnClickShopCard += HandleClickCard;
        View.OnClickRelic += HandleClickRelic;
        View.OnClickCardToRemove += HandleClickCardToRemove;
        View.OnClickConfirm += HandleClickConfirm;
        EventBus<TooltipShowEvent>.Subscribe(OnHoverEvent);
    }



    private void UnsubscribeEvents()
    {
        View.OnClickContinue -= HandleClickContinue;
        View.OnClickRemove -= HandleClickRemove;
        View.OnClickShopCard -= HandleClickCard;
        View.OnClickRelic -= HandleClickRelic;
        View.OnClickCardToRemove -= HandleClickCardToRemove;
        View.OnClickConfirm -= HandleClickConfirm;
        EventBus<TooltipShowEvent>.Unsubscribe(OnHoverEvent);
    }



    private void HandleClickRelic(int id)
    {
        if (Relics.TryGetValue(id, out var relic))
        {
            if (_playerService.Coin >= relic.Price)
            {
                _playerService.SpendCoin(relic.Price);
                ServiceLocator.Get<RelicService>().AddRelic(id);
                Relics.Remove(id);
                View.RefreshRelics(Relics.Values.ToList());
            }
        }
    }
    private void OnHoverEvent(TooltipShowEvent @event)
    {
        if (!@event.IsHovering)
        {
            View.HideToolTip();
            return;
        }
        View.ShowToolTip(@event.Data, @event.Position);
    }
    private void HandleClickCard(int id)
    {
        if (Cards.TryGetValue(id, out var card))
        {
            if (_playerService.Coin >= card.Price)
            {
                _playerService.SpendCoin(card.Price);
                _playerService.AddCard(id);
                Cards.Remove(id);
                View.RefreshCards(Cards.Values.ToList());
            }
        }
    }
    /// <summary>
    /// 确认移除卡牌
    /// </summary>
    private void HandleClickConfirm()
    {
        _playerService.SpendCoin(_removeCost);
        _playerService.RemoveCard(_selectedCardId);
        View.HideRemovePanel();
    }
    private void HandleClickRemove()
    {


        View.RefreshRemoveCost(_removeCost);
        // Debug.Log($"{_removeCost} {_playerService.Coin}");
        // 查询价格
        if (_playerService.Coin < _removeCost)
        {
            return;
        }

        var deck = _playerService.DeckCardIds;
        var displayDatas = new List<CardDisplayData>();
        foreach (var cardId in deck)
        {
            var config = _configService.GetTable<CardConfig>().Get(cardId);
            displayDatas.Add(CardDisplayData.FromConfig(config));
        }
        View.ShowRemovePanel(displayDatas);
    }
    private void HandleClickCardToRemove(int id)
    {
        var config = _configService.GetTable<CardConfig>().Get(id);
        _selectedCardId = id;
        View.ShowConfirmPanel(CardDisplayData.FromConfig(config));
    }


    public override void Dispose()
    {
        UnsubscribeEvents();
        base.Dispose();
    }
    public async UniTask InitShop()
    {
        View.RefreshCards(GenerateCardDisplayData(3));
        View.RefreshRelics(await GenerateRelicDisplayData(3));
    }

    private void HandleClickContinue()
    {
        _uiService.Close<ShopView>();
    }

    public List<CardDisplayData> GenerateCardDisplayData(int count = 3)
    {

        Cards.Clear();
        // 1. 获取商品池配置
        var cardConfigs = _shopTable
            .GetAll()
            .Where(p => p.ItemType == 1)
            .ToList();

        // 2. 过滤已拥有的
        // var availableRelics = poolConfigs
        //     .Where(p => p.ItemType == 2)
        //     .Where(p => !_relicService.HasRelic(p.ItemId))
        //     .ToList();

        // 3. 随机选取
        var selectedCards = WeightedRandomPicker.Pick(cardConfigs, count, p => p.Weight);
        // var selectedRelics = WeightedRandomPicker.Pick(availableRelics, relicCount, p => p.Weight);

        // 4. 组装数据
        var cardTable = _configService.GetTable<CardConfig>();
        // var relicTable = _configService.GetTable<RelicConfig>();

        foreach (var shopConfig in selectedCards)
        {
            var config = cardTable.Get(shopConfig.ItemId);
            if (config != null)
            {
                Cards.Add(config.Id, CardDisplayData.FromConfig(config, shopConfig.Price));
            }
        }
        return Cards.Values.ToList();
    }
    public async UniTask<List<RelicDisplayData>> GenerateRelicDisplayData(int count = 3)
    {
        Relics.Clear();
        var relicConfigs = _shopTable
            .GetAll()
            .Where(p => p.ItemType == 2)
            .ToList();

        // var availableRelics = poolConfigs
        //     .Where(p => p.ItemType == 2)
        //     .Where(p => !_relicService.HasRelic(p.ItemId))
        //     .ToList();

        var selectedRelics = WeightedRandomPicker.Pick(relicConfigs, count, p => p.Weight);

        var relicTable = _configService.GetTable<RelicConfig>();
        // var relicTable = _configService.GetTable<RelicConfig>();

        foreach (var shopConfig in selectedRelics)
        {
            var config = _relicTable.Get(shopConfig.ItemId);
            if (config != null)
            {
                var relic = await RelicDisplayData.FromConfig(config, shopConfig.Price);
                Relics.Add(config.Id, relic);
            }
        }
        return Relics.Values.ToList();
    }


}
