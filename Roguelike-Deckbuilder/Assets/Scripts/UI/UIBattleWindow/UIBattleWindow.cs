using System.Collections;
using System.Collections.Generic;
using LitFramework.EventBus;
using LitFramework.UI.Core.Window;
using UnityEngine;

public partial class UIBattleWindow : UIWindow
{
    private IEventBinding<HpChangedEvent> m_HpChangedEventBinding;
    public override void OnOpen(object args)
    {
        base.OnOpen(args);
        m_HpChangedEventBinding = new EventBinding<HpChangedEvent>(OnHpChanged);
        EventBus<HpChangedEvent>.Subscribe(m_HpChangedEventBinding);
    }
    protected override void OnShowInternal(object param)
    {

    }
    private void OnHpChanged(HpChangedEvent evt)
    {
        // if (evt.characterData != LogicCharacter) return;
        // UpdateHp(evt.NewHp, LogicCharacter.MaxHp);
        // PlayHitEffect();  // 闪白、震动
        // ShowDamageNumber(evt.OldHp - evt.NewHp);
    }
    private void UpdateHp(int currentHp, int maxHp)
    {
        b_HPText.SetText(currentHp + "/" + maxHp);
        b_HPSlider.value = currentHp / maxHp;
    }

}
