using System.Collections;
using System.Collections.Generic;
using LitFramework;
using LitFramework.Config;
using LitFramework.EventBus;
using Newtonsoft.Json;
using UnityEngine;

public class Enemy : CharacterBase
{
    public Enemy(EnemyConfig config, IEnemyAI ai) : base(config.MaxHp)
    {
        Config = config;
        AI = ai;
    }
    public EnemyConfig Config { get; private set; }
    public int CurrentActionId { get; private set; }
    public IEnemyAI AI { get; private set; }
    public override int Id => Config.Id;
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
        var intentConfigs = new List<IntentConfig>();
        foreach (var intent in actionConfig.Intents)
        {
            var intentConfig = configService.GetTable<IntentConfig>().Get(intent.IntentId);
            intentConfigs.Add(intentConfig);
        }
        EventBus<IntentEvent>.Publish(new IntentEvent { Enemy = this, IntentConfigs = intentConfigs });
    }

    // 执行意图
    public void ExecuteIntent()
    {
        Debug.Log($"执行意图：{JsonConvert.SerializeObject(_actions[CurrentActionId])}");
        var executor = ServiceLocator.Get<EffectExecutor>();
        if (!_actions.ContainsKey(CurrentActionId))
        {
            Debug.LogError($"没有意图：{CurrentActionId}");
            return;
        }
        var intents = _actions[CurrentActionId];
        foreach (var intent in intents)
        {
            intent.Execute(executor, this);
        }
    }
}
