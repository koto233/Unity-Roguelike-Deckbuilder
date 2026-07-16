using System.Collections;
using System.Collections.Generic;
using LitFramework;
using LitFramework.Config;
using LitFramework.EventBus;
using UnityEngine;

public class Enemy : CharacterBase
{
    public Enemy(EnemyConfig config, IEnemyAI ai) : base(config.MaxHp)
    {
        Config = config;
        AI = ai;
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
        // Debug.Log("意图" + CurrentIntent);
        var configService = ServiceLocator.Get<IConfigService>();
        var intentConfig = configService.GetTable<IntentConfig>().Get((int)CurrentIntent);
        // Debug.Log("意图" + intentConfig.Name);
        EventBus<IntentEvent>.Publish(new IntentEvent { Enemy = this, IntentConfig = intentConfig });
    }

    // 执行意图
    public void ExecuteIntent(BattleContext context)
    {
        switch (CurrentIntent)
        {
            case IntentType.Attack:
                context.Player.TakeDamage(Config.Damage + Strength);
                break;
            case IntentType.Defend:
                AddBlock(Config.Defend);
                break;
            case IntentType.StrongAttack:
                break;
        }
        LastIntent = CurrentIntent;
    }
}
// public struct IntentDisplayData
// {

// }
