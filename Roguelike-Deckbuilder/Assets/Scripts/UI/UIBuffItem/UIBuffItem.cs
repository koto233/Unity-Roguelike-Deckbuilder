using System.Collections;
using System.Collections.Generic;
using LitFramework.UI.Core.Window;
using UnityEngine;

public partial class UIBuffItem : UIBase
{

    public void Init()
    {
        // 加载图标
    }

    public void SetStacks(int stacks)
    {
        if (stacks > 1)
        {
            b_StackText.SetText(stacks.ToString());
            b_StackText.gameObject.SetActive(true);
        }
        else
        {
            b_StackText.gameObject.SetActive(false);
        }
    }
}
