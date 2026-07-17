using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using LitFramework.UI.Core.Window;
using UnityEngine;

public partial class UIPlayerItem : UIBase
{
    public Vector3 DamageTextPos => b_DamageTextPos.transform.position;
    public void UpdateHP(int currentHp, int maxHp)
    {
        b_HPText.SetText($"{currentHp}/{maxHp}");
        b_HPSlider.DOValue((float)currentHp / maxHp, 0.5f).SetEase(Ease.Linear);
    }
}
