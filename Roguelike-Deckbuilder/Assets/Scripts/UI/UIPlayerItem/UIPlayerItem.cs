using System.Collections;
using System.Collections.Generic;
using LitFramework.UI.Core.Window;
using UnityEngine;

public partial class UIPlayerItem : UIBase
{
    public void UpdateHP(int currentHp, int maxHp)
    {
        b_HPText.SetText($"{currentHp}/{maxHp}");
    }
}
