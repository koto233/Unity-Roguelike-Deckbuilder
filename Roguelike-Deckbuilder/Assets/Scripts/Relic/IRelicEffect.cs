public interface IRelicEffect
{
    // 当遗物被激活时调用（例如玩家获得遗物时）
    void OnActivate(Relic relic);
    
    // 当遗物被移除/禁用时调用（取消订阅事件）
    void OnDeactivate(Relic relic);
}