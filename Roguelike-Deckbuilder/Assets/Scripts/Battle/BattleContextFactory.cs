using System.Linq;
using LitFramework;
using LitFramework.Config;
using UnityEngine;

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
            EnemyConfigs = new(),
            CurrentTurn = 0,
            IsPlayerTurn = true,
            Target = null,
            GoldReward = 0
        };

        // 1. 根据节点类型确定稀有度
        int rarity = args.Type switch
        {
            MapNodeType.Battle => 1,
            MapNodeType.Elite => 2,
            MapNodeType.Boss => 3,
            _ => 1
        };

        // 2. 从配置表筛选对应稀有度的敌人
        var allEnemies = configService.GetTable<EnemyConfig>().GetAll();
        var candidates = allEnemies.Where(e => e.Rarity == rarity).ToList();

        if (candidates.Count == 0)
        {
            Debug.LogError($"未找到稀有度 {rarity} 的敌人配置，使用默认敌人");
            candidates = allEnemies.Where(e => e.Rarity == 1).ToList();
        }

        // 3. 随机选取一个
        var selectedConfig = candidates[UnityEngine.Random.Range(0, candidates.Count)];

        // 4. 创建敌人实例（AI 根据配置决定）
        var ai = CreateAI(selectedConfig.Key);
        var enemy = new Enemy(selectedConfig, ai);

        battleContext.EnemyConfigs.Add(selectedConfig);
        battleContext.Enemies.Add(enemy);

        // 5. 计算金币奖励（可选）
        battleContext.GoldReward = CalculateGoldReward(rarity);

        return battleContext;
    }

    private static IEnemyAI CreateAI(string aiType)
    {
        return aiType switch
        {
            "Shrinker_beetle" => new BeetleAI(),
            "Twig_slime_s" => new TwigSlimeSAI(),
            "Twig_slime_m" => new TwigSlimeMAI(),
            "Byrdonis" => new ByrdonisAI(),
            "Vantom" => new VantomAI(),
            _ => new DefaultAI()
        };
    }

    private static int CalculateGoldReward(int rarity)
    {
        return rarity switch
        {
            1 => UnityEngine.Random.Range(10, 20),
            2 => UnityEngine.Random.Range(35, 45),
            3 => 100,
            _ => 15
        };
    }
}