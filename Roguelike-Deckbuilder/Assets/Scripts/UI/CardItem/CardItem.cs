using System.Collections;
using System.Collections.Generic;
using LitFramework;
using LitFramework.UI.Core.Window;
using UnityEngine;

public partial class CardItem : UIBase
{
    public void Init(CardDisplayData data)
    {
        b_CostText.SetText(data.EnergyCost.ToString());
        b_Icon.sprite = data.Icon;
        b_NameText.SetText(data.Name);
        b_DescText.SetText(data.Description);
        b_PortraitBorder.sprite = data.PortraitBorderSprite;
        b_Frame.sprite = data.FrameSprite;
        b_type_text.SetText(data.Type);
    }

}
