using LitFramework;
using LitFramework.Config;

public static class BattleContextFactory
{
    public static BattleContext Create(BattleStartParams args)
    {
        var globalPlayer = ServiceLocator.Get<PlayerDataService>();
        var battleContext = new BattleContext()
        {
            Player = new Player(globalPlayer.CurrentHp, globalPlayer.MaxHp, BattleRules.MaxEnergy),
            Enemies = new(),
            EnemyConfigs = new(),
            CurrentTurn = 0,
            IsPlayerTurn = true,
            Target = null,
            GoldReward = 0
        };
        switch (args.Type)
        {
            case MapNodeType.Battle:
                var enemyConfig = ServiceLocator.Get<IConfigService>().GetTable<EnemyConfig>().Get(1);
                battleContext.EnemyConfigs.Add(enemyConfig);
                battleContext.Enemies.Add(new Enemy(enemyConfig, new BeetleAI()));
                break;
            case MapNodeType.Elite:
                enemyConfig = ServiceLocator.Get<IConfigService>().GetTable<EnemyConfig>().Get(1);
                battleContext.EnemyConfigs.Add(enemyConfig);
                battleContext.Enemies.Add(new Enemy(enemyConfig, new BeetleAI()));
                break;
            case MapNodeType.Boss:
                enemyConfig = ServiceLocator.Get<IConfigService>().GetTable<EnemyConfig>().Get(1);
                battleContext.EnemyConfigs.Add(enemyConfig);
                battleContext.Enemies.Add(new Enemy(enemyConfig, new BeetleAI()));
                break;


        }

        return battleContext;
    }



}