using LitFramework;
using LitFramework.EventBus;

public class DrawExtraOnTurnStartEffect : IRelicEffect
{
    private int _extraCards;
    private EffectExecutor _executor;
    public DrawExtraOnTurnStartEffect(string paramsJson)
    {
        // var param = JsonUtility.FromJson<EffectParam>(paramsJson);
        // _extraCards = param.amount;
        _executor = ServiceLocator.Get<EffectExecutor>();
    }

    public void OnActivate(Relic relic)
    {
        // EventBus<TurnStartEvent>.Subscribe(OnTurnStart);
    }

    public void OnDeactivate(Relic relic)
    {
        // EventBus<TurnStartEvent>.Unsubscribe(OnTurnStart);
    }

    // private void OnTurnStart(TurnStartEvent evt)
    // {
        // 抽牌逻辑由 BattleController 或 DrawService 实现
        // _executor.DrawCards(_extraCards);
    // }

    [System.Serializable]
    private class EffectParam
    {
        public int amount;
    }
}