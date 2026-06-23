using System.Collections;
using System.Collections.Generic;
using LitFramework.UI.Core.Window;
using UnityEngine;

public partial class UIEnemyItem : UIBase
{
    public EnemyData Enemy { get; private set; }


    private void Update()
    {
        if (Enemy != null)
        {
            UpdateHP(Enemy.CurrentHp, Enemy.MaxHp);
        }
    }
    public void UpdateHP(int currentHp, int maxHp)
    {
        b_HPText.SetText($"{currentHp}/{maxHp}");
        b_HPSlider.value = (float)currentHp / maxHp;
    }
    public void SetEnemy(EnemyData enemy)
    {
        Enemy = enemy;
    }
}
