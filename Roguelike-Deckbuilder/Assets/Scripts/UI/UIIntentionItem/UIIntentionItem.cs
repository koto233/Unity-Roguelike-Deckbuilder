using System.Collections;
using System.Collections.Generic;
using LitFramework.UI.Core.Window;
using UnityEngine;

public partial class UIIntentionItem : UIBase, ITooltipDataProvider
{
    private IntentConfig _intentConfig;
    public void Init(IntentConfig intentConfig)
    {
        _intentConfig = intentConfig;
        Debug.Log("意图"+_intentConfig.Description);
    }
    public TooltipData GetTooltipData()
    {
        return new TooltipData
        {
            Description = _intentConfig.Description,
        };
    }
}
