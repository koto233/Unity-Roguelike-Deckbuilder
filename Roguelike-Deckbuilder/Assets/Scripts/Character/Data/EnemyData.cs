using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData : CharacterData
{
    public EnemyData(int maxHp, int strength = 0) : base(maxHp, strength) { }

    protected override EntityType GetEntityType() => EntityType.Enemy;

}
