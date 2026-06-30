using System.Collections;
using System.Collections.Generic;
using LitFramework.UI.Core.Window;
using UnityEngine;

public partial class UIEnemyItem : UIBase
{
    public Enemy Enemy { get; private set; }

 
    public void UpdateHP(int currentHp, int maxHp)
    {
        b_HPText.SetText($"{currentHp}/{maxHp}");
        b_HPSlider.value = (float)currentHp / maxHp;
    }
    public void SetEnemy(Enemy enemy)
    {
        Enemy = enemy;
    }
}
