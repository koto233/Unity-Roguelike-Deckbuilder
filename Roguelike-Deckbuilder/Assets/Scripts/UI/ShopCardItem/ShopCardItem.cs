using System.Collections;
using System.Collections.Generic;
using LitFramework;
using LitFramework.UI.Core.Window;
using UnityEngine;

public partial class ShopCardItem : UIBase
{
    private int _id;
    public event System.Action<int> OnClick;
    void OnEnable()
    {
        b_Click.onClick.AddListener(() =>
        {
            OnClick?.Invoke(_id);
        });
    }
    void OnDisable()
    {
        b_Click.onClick.RemoveAllListeners();
    }

    public void Init(CardDisplayData data)
    {
        _id = data.Id;
        b_CostText.SetText(data.EnergyCost.ToString());
        b_Icon.sprite = data.Icon;
        b_NameText.SetText(data.Name);
        b_DescText.SetText(data.Description);
        b_PortraitBorder.sprite = data.PortraitBorderSprite;
        b_Frame.sprite = data.FrameSprite;
        b_type_text.SetText(data.Type);
        b_PriceText.SetText(data.Price.ToString());
    }
    public void ClearEvents()
    {
        OnClick = null;
    }
}
