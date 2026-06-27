using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData : CharacterData
{
    public EnemyData(EnemyConfig config) : base(config.MaxHp, config.BaseStrength)
    {
        Config = config;
    }

    public EnemyConfig Config { get; private set; }
    public EnemyIntent CurrentIntent { get; private set; }
    public List<EnemyAIPattern> Patterns { get; private set; }
    // public EnemyData(EnemyConfig config)
    // {
    //     Config = config;
    // };
    protected override EntityType GetEntityType() => EntityType.Enemy;
    // AI 决策：根据当前状态（回合数、血量等）计算意图
    public void DetermineIntent(BattleContext context)
    {

    }

    // 执行意图
    public void ExecuteIntent(BattleContext context)
    {

    }
}
