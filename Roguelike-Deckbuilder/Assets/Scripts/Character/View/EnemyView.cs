using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyView : CharacterView
{
    public TextMeshProUGUI intentText;
    public Image intentIcon;

    private void Start()
    {
        // EventBus<EnemyIntentEvent>.Register(OnIntentChanged);
    }

    // private void OnIntentChanged(EnemyIntentEvent evt)
    // {
    //     if (evt.Enemy != LogicCharacter) return;
    //     intentText.text = evt.Damage.ToString();
    //     // 切换图标等
    // }
}
