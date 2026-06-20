using System.Collections;
using System.Collections.Generic;
using LitFramework.UI.Core.Window;
using UnityEngine;

public class UICombatantItem : UIBase
{
    // ===== 完整实现，子类不需要改 =====
    public void UpdateHP(int current, int max) { }
    public void UpdateBlock(int block) { }
    // public void UpdateBuffs(List<BuffData> buffs) { }

    // ===== 默认实现，子类可以重写（加特效） =====
    public virtual void PlayHitAnimation() { }
    public virtual void PlayBlockAnimation() { }
    public virtual void PlayHealAnimation() { }
    public virtual void PlayDeathAnimation() { }
}
