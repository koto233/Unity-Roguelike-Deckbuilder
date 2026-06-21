using System.Collections;
using System.Collections.Generic;
using LitFramework.EventBus;
using UnityEngine;

public struct EnergyChangedEvent: IEvent
{
    public int OldEnergy;
    public int NewEnergy;
}