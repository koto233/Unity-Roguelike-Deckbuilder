using System.Collections;
using System.Collections.Generic;
using LitFramework.UI.Core.Window;
using UnityEngine;

public partial class UICardDisplay : UIBase
{
    public Card _card { get; private set; }
    public void Init(Card card)
    {
        RefreshUI(card);
    }
    public void RefreshUI(Card card)
    {
        _card = card;
        b_CostText.SetText(card.Config.Cost.ToString());
        b_CostText.color = Color.black;
        b_NameText.SetText(card.Config.Name);
        b_DescText.SetText(card.Description);
    }

}
