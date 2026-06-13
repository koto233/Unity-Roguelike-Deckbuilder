using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerView : CharacterView
{
    [Header("玩家特有")]
    public TextMeshProUGUI energyText;

    // protected override void OnEnable()
    // {
    //     base.OnEnable();
    //     EventBus<EnergyChangedEvent>.Register(OnEnergyChanged);
    // }

    // protected override void OnDisable()
    // {
    //     base.OnDisable();
    //     EventBus<EnergyChangedEvent>.Unregister(OnEnergyChanged);
    // }

    // private void OnEnergyChanged(EnergyChangedEvent evt)
    // {
    //     if (evt.Character != LogicCharacter) return;
    //     energyText.text = evt.NewEnergy.ToString();
    // }
}
