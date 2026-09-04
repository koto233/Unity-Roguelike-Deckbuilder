using System.Collections;
using System.Collections.Generic;
using LitFramework.UI.Core.Window;
using Newtonsoft.Json;
using UnityEngine;

public partial class Tooltip : UIBase
{
    public void Show(TooltipData data, Vector2 position)
    {
        // Debug.Log($"ShowTooltip: {JsonConvert.SerializeObject(data)} at {position}");
        if (string.IsNullOrEmpty(data.Description)) return;
        string desc = string.Format(data.Description, data.Value);
        b_DescText.SetText(desc);
        transform.position = position;
        gameObject.SetActive(true);
    }

}
