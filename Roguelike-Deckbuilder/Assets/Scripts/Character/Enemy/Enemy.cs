using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitFramework;
using LitFramework.Config;
using LitFramework.EventBus;
using LitFramework.UI.Core.Service;
using Newtonsoft.Json;
using UnityEngine;

public class Enemy : CharacterBase
{
    public Enemy(EnemyConfig config, IEnemyAI ai) : base(config.MaxHp)
    {
        Config = config;
        AI = ai;
        Init();
    }
    public EnemyConfig Config { get; private set; }
    public int CurrentActionId { get; private set; }
    public IEnemyAI AI { get; private set; }
    // public override int ConfigId => Config.Id;
    public override EntityType EntityType => EntityType.Enemy;
    private Dictionary<int, List<IIntent>> _actions = new();
    public void Init()
    {
        for (int i = 0; i < Config.Actions.Length; i++)
        {
            var actionId = Config.Actions[i];
            var actionConfig = ServiceLocator.Get<IConfigService>().GetTable<ActionConfig>().Get(actionId);
            for (int j = 0; j < actionConfig.Intents.Length; j++)
            {
                var intentConfig = ServiceLocator.Get<IConfigService>().GetTable<IntentConfig>().Get(actionConfig.Intents[j].IntentId);
                var intent = IntentFactory.CreateIntent(intentConfig, actionConfig.Intents[j].Value);
                if (!_actions.ContainsKey(actionId))
                    _actions[actionId] = new List<IIntent>();
                _actions[actionId].Add(intent);
                Debug.Log($"敌人{Config.Name}的意图{intentConfig.Description}{intent == null}");
            }
        }
    }

    public void DetermineAction()
    {
        CurrentActionId = AI.DecideAction(Config.Actions);
        var configService = ServiceLocator.Get<IConfigService>();
        var actionConfig = configService.GetTable<ActionConfig>().Get(CurrentActionId);
        EventBus<IntentEvent>.Publish(new IntentEvent { InstanceId = InstanceId, IntentEntries = actionConfig.Intents });
    }

    // 执行意图
    public void ExecuteIntent()
    {

        var executor = ServiceLocator.Get<EffectExecutor>();
        if (!_actions.ContainsKey(CurrentActionId))
        {
            Debug.LogError($"没有意图：{CurrentActionId}");
            return;
        }
        var intents = _actions[CurrentActionId];
        foreach (var intent in intents)
        {
            Debug.Log($"敌人{Config.Name}执行意图{intent.GetType().Name}");
            intent.Execute(executor, this);
        }

    }
    protected override void OnDeath()
    {
        if (Config.Rarity == 3)
        {
            ServiceLocator.Get<UIService>().OpenAsync<GameOverView>().Forget();
        }
        base.OnDeath();
    }
}
public class IntentEntry
{
    public int IntentId;
    public int Value;
}