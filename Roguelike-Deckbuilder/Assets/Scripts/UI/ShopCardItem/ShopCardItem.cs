using System.Collections;
using System.Collections.Generic;
using LitFramework;
using LitFramework.UI.Core.Window;
using UnityEngine;

public partial class ShopCardItem : UIBase
{
    public event System.Action<int> OnClick;
    public void Init(CardDisplayData config)
    {
        b_CostText.SetText(config.EnergyCost.ToString());
        b_Icon.sprite = config.Icon;
        b_NameText.SetText(config.Name);
        b_DescText.SetText(config.Description);
        b_PortraitBorder.sprite = config.PortraitBorderSprite;
        b_Frame.sprite = config.FrameSprite;
        b_type_text.SetText(config.Type);
        b_PriceText.SetText(config.Price.ToString());
        // b_PortraitBorder.sprite = ServiceLocator.Get<UIAtlasService>().GetSprite("card_portrait_border_attack_s");
        // b_Frame.sprite = ServiceLocator.Get<UIAtlasService>().GetSprite("card_frame_attack_s");
    }

}
