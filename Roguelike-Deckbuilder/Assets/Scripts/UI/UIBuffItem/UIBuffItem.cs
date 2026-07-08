using System.Collections;
using System.Collections.Generic;
using LitFramework.EventBus;
using LitFramework.UI.Core.Window;
using UnityEngine;
using UnityEngine.EventSystems;

public partial class UIBuffItem : UIBase, ITooltipDataProvider
{

    private IBuff _buff;

    public TooltipData GetTooltipData()
    {
        return new TooltipData
        {
            Description = _buff.Config.Description,
            Value = _buff.Stacks
        };
    }

    public void Init(IBuff buff)
    {
        // 加载图标
        _buff = buff;
    }

    public void SetStacks(int stacks)
    {
        b_StackText.SetText(stacks.ToString());
        b_StackText.gameObject.SetActive(stacks > 1);
    }
}
