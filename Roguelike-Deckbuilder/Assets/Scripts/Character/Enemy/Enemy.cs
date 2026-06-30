using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : CharacterBase
{
    public Enemy(EnemyConfig config) : base(config.MaxHp, config.BaseStrength)
    {
        Config = config;
    }
    public IntentType LastIntent { get; set; }
    public EnemyConfig Config { get; private set; }
    public IntentType CurrentIntent { get; private set; }
    public IEnemyAI AI { get; private set; }
    public override int Id => Config.Id;

    // public EnemyData(EnemyConfig config)
    // {
    //     Config = config;
    // };
    protected override EntityType EntityType => EntityType.Enemy;

    // AI 决策：根据当前状态（回合数、血量等）计算意图
    public void DetermineIntent(BattleContext context)
    {
        CurrentIntent = AI.DecideIntent(this, context);
    }

    // 执行意图
    public void ExecuteIntent(BattleContext context)
    {
        switch (CurrentIntent)
        {
            case IntentType.Attack:
               
                break;
            case IntentType.Defend:
               
                break;
            case IntentType.StrongAttack:
                
                break;
        }
        LastIntent = CurrentIntent;
    }
}
