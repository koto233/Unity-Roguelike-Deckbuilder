using System.Collections;
using System.Collections.Generic;
using LitFramework.EventBus;
using UnityEngine;

public struct EnergyChangedEvent : IEvent
{
    public EntityType EntityType;  // Player / Enemy
    public string EntityId;        // 如果是敌人，这里存 EnemyId
    public int CurrentEnergy;
    public int MaxEnergy;
}
public enum EntityType { Player, Enemy }