using System.Collections;
using System.Collections.Generic;
using LitFramework.UI.Core.Window;
using UnityEngine;

public partial class UIBattleWindow : UIWindow
{
    public void Init(GameObject cardPrefab, BattleContext battleContext)
    {
        b_HandZone.Init(cardPrefab, battleContext);
    }
    public override void OnOpen(object args)
    {
        base.OnOpen(args);
    }
    protected override void OnShowInternal(object param)
    {

    }

    public void RefreshHp(int currentHp, int maxHp)
    {
        b_HPText.SetText(currentHp + "/" + maxHp);
        b_HPSlider.value = currentHp / maxHp;

    }
    public void RefreshEnergy(int energy) { /* 更新能量显示 */ }
    public void RefreshHand(List<Card> hand)
    {
        b_HandZone.RefreshHand(hand);
    }

}
