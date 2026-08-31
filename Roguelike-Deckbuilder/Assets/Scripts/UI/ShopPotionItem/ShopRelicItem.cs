using LitFramework;
using LitFramework.UI.Core.Window;
using UnityEngine.UI;

public partial class ShopRelicItem : UIBase
{
    private Image _icon;
    public event System.Action<int> OnClick;
    void OnEnable()
    {
        _icon = GetComponent<Image>();
    }
    public void Init(RelicDisplayData data)
    {
        RefreshUI(data);
    }

    public void RefreshUI(RelicDisplayData data)
    {
        b_PriceText.SetText(data.Price.ToString());
        _icon.sprite = data.Icon;
    }
}
