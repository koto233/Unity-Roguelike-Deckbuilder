public interface IEnemyAI
{
    /// <summary>
    /// 决定本回合使用哪个意图
    /// </summary>
    /// <returns>IntentConfig 的 Id</returns>
    IntentType DecideIntent(Enemy enemy, BattleContext context);
}