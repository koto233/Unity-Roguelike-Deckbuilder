using LitFramework.EventBus;

/// <summary>
/// 战斗真正开始时发布（Context 已创建、战斗 UI 已就绪）。
/// 与地图层的 BattleStartEvent（点击战斗节点）区分开。
/// </summary>
public struct BattleStartedEvent : IEvent
{
    public BattleContext Context;
}
