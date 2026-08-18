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
            CurrentTurn = 0,
            IsPlayerTurn = true,
            Target = null,
            GoldReward = 0
        };
        foreach (var enemyId in args.EnemyIds)
        {
            var enemyConfig = ServiceLocator.Get<IConfigService>().GetTable<EnemyConfig>().Get(enemyId);
            var enemyAi = EnemyAIFactory.Create(enemyConfig.AIType);
            battleContext.Enemies.Add(new Enemy(enemyConfig, enemyAi));
        }
        return battleContext;
    }
}