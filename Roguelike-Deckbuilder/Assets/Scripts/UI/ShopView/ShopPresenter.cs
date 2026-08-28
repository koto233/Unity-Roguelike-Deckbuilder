using System;
using System.Collections;
using System.Collections.Generic;
using LitFramework;
using LitFramework.UI.Core.Service;
using UnityEngine;

public class ShopPresenter : BasePresenter<ShopView>
{
    private UIService _uiService;
    private int _selectedCardId;
    private PlayerDataService playerService;
    public ShopPresenter(ShopView view) : base(view)
    {
    }

    public override void Init()
    {
        _uiService = ServiceLocator.Get<UIService>();
        playerService = ServiceLocator.Get<PlayerDataService>();
        SubscribeEvents();
        InitShop();
    }
    private void SubscribeEvents()
    {
        View.OnClickContinue += HandleClickContinue;
        View.OnClickRemove += HandleClickRemove;
        View.OnClickShopCard += HandleClickCard;
        View.OnClickRelic += HandleClickRelic;
        View.OnClickCardToRemove += HandleClickCardToRemove;
        View.OnClickConfirm += HandleClickConfirm;
    }


    private void UnsubscribeEvents()
    {
        View.OnClickContinue -= HandleClickContinue;
        View.OnClickRemove -= HandleClickRemove;
        View.OnClickShopCard -= HandleClickCard;
        View.OnClickRelic -= HandleClickRelic;
        View.OnClickCardToRemove -= HandleClickCardToRemove;
        View.OnClickConfirm -= HandleClickConfirm;
    }

    private void HandleClickConfirm()
    {
        playerService.RemoveCard(_selectedCardId);
    }

    private void HandleClickRelic(int id)
    {
        // 查询价格
        // if(playerService.Coin >=)
        //  playerService.SpendCoin();

        playerService.AddRelic(id);
    }

    private void HandleClickCard(int id)
    {
        // 查询价格
        // if(playerService.Coin >=)
        //  playerService.SpendCoin();
        playerService.AddCard(id);
    }
    private void HandleClickCardToRemove(int id)
    {
        _selectedCardId = id;
        View.ShowConfirmPanel();
    }

    private void HandleClickRemove()
    {
         // 查询价格
        // if(playerService.Coin >=)
        //  playerService.SpendCoin();
        View.ShowRemovePanel();
    }
    public override void Dispose()
    {
        base.Dispose();
        UnsubscribeEvents();
    }
    public void InitShop()
    {
        // View.CreateCardList();
        // View.CreatePotionList();
    }

    private void HandleClickContinue()
    {
        _uiService.Close<ShopView>();
    }
}
