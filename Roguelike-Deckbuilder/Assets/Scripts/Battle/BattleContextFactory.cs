using System.Collections.Generic;
using System.Linq;
using LitFramework;
using LitFramework.Config;


public static class BattleContextFactory
{
    public static BattleContext Create(BattleStartParams args)
    {
        var globalPlayer = ServiceLocator.Get<PlayerDataService>();
        var configService = ServiceLocator.Get<IConfigService>();

        var battleContext = new BattleContext
        {
            Player = new Player(globalPlayer.CurrentHp, globalPlayer.MaxHp, BattleRules.MaxEnergy),
            Enemies = new(),
            CurrentTurn = 0,
            IsPlayerTurn = true,
            Target = null,
            GoldReward = 0
        };

        var enemyTable = configService.GetTable<EnemyConfig>();
        var enemiesRarity = new List<int>();
        foreach (var key in args.EnemyKeys)
        {
            var config = enemyTable.Get(key);
            if (config == null)
            {
                // Debug.LogError($"敌人配置不存在: {key}");
                continue;
            }
            var ai = EnemyAIFactory.Create(config.Key);
            enemiesRarity.Add(config.Rarity);
            battleContext.Enemies.Add(new Enemy(config, ai));
        }
        battleContext.GoldReward = CalculateGoldReward(enemiesRarity);
        return battleContext;
    }



    private static int CalculateGoldReward(List<int> raritys)
    {
        System.Random random = new System.Random();
        var rewardConfig = ServiceLocator.Get<IConfigService>().GetTable<RewardConfig>();
        int rewardCoin = 0;
        foreach (var rarity in raritys)
        {
            var reward = rewardConfig.Get(rarity);
            rewardCoin += random.Next(reward.CoinMin, reward.CoinMax);
        }
        return rewardCoin;
    }
}