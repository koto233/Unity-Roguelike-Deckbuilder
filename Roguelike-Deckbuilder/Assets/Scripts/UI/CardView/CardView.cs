using System.Collections;
using System.Collections.Generic;
using LitFramework;
using LitFramework.UI.Core.Window;
using UnityEngine;

public partial class CardView : UIBase
{
    public Card _card { get; private set; }
    public void Init(Card card)
    {
        RefreshUI(card);
    }
    public void RefreshUI(Card card)
    {
        b_CostText.SetText(card.Config.Cost.ToString());
        b_Icon.sprite = ServiceLocator.Get<CardIconService>().GetCardIcon(card.Config.IconName);
        b_NameText.SetText(card.Config.Name);
        b_DescText.SetText(card.Description);
        switch (card.Config.Type)
        {
            case "Attack":
                b_PortraitBorder.sprite = ServiceLocator.Get<UIAtlasService>().GetSprite("card_portrait_border_attack_s");
                b_Frame.sprite = ServiceLocator.Get<UIAtlasService>().GetSprite("card_frame_attack_s");
                b_type_text.SetText("攻击");
                break;
            case "Skill":
                b_PortraitBorder.sprite = ServiceLocator.Get<UIAtlasService>().GetSprite("card_portrait_border_skill_s");
                b_Frame.sprite = ServiceLocator.Get<UIAtlasService>().GetSprite("card_frame_skill_s");
                b_type_text.SetText("技能");
                break;
            default:
                break;
        }
    }

}
