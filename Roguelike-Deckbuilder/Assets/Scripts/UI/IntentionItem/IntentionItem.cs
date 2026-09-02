using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitFramework;
using LitFramework.Asset;
using LitFramework.UI.Core.Window;
using UnityEngine;
using UnityEngine.UI;

public partial class IntentionItem : UIBase, ITooltipDataProvider
{
    private IntentDisplayData _intentDisplayData;
    private Image _icon;
    protected override void Awake()
    {
        base.Awake();
        _icon = GetComponent<Image>();
    }
    public void Init(IntentDisplayData displayData)
    {
        _intentDisplayData = displayData;
        b_Num.SetText(displayData.Value.ToString());
        _icon.sprite = displayData.Icon;
    }
   
    public TooltipData GetTooltipData()
    {
        return new TooltipData
        {
            Description = _intentDisplayData.Description,
        };
    }
}
