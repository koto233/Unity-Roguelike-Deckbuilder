using LitFramework;
using LitFramework.UI.Core.Window;
using UnityEngine.UI;

public partial class ShopRelicItem : UIBase, ITooltipDataProvider
{
    private int _id;
    private Image _icon;
    private Button _buyButton;
    private RelicDisplayData _data;
    public event System.Action<int> OnClick;

    protected override void Awake()
    {
        base.Awake();
        _icon = GetComponent<Image>();
        _buyButton = GetComponentInChildren<Button>();
    }


    void OnEnable()
    {
        _buyButton.onClick.AddListener(() =>
              {
                  OnClick?.Invoke(_id);
              });
    }
    void OnDisable()
    {
        _buyButton.onClick.RemoveAllListeners();
    }

    public void Init(RelicDisplayData data)
    {
        _data = data;
        _id = data.Id;
        b_PriceText.SetText(data.Price.ToString());
        _icon.sprite = data.Icon;

    }
    public void ClearEvents()
    {
        OnClick = null;
    }
    public TooltipData GetTooltipData()
    {
        return new TooltipData
        {
            Description = _data.Description,
        };
    }
}
