using System.Collections;
using System.Collections.Generic;
using LitFramework.UI.Core.Window;
using UnityEngine;

public partial class BuffTooltip : UIBase
{
    public void Show(object data, Vector2 position)
    {
        if (data is not IBuff buff) return;
        // _name.text = buff.Config.DisplayName;
        // _stacks.text = $"层数: {buff.Stacks}";
        // _duration.text = buff.Duration > 0 ? $"持续: {buff.Duration}回合" : "无限";
        string desc = string.Format(buff.Config.Description, buff.Config.Value);
        b_DescText.SetText(desc);
        transform.position = position;
        gameObject.SetActive(true);
    }

    public void Hide() => gameObject.SetActive(false);
}
