using System.Collections.Generic;
using System.Linq;
using LitFramework;
using LitFramework.Config;
using LitFramework.EventBus;

public class RelicService
{
    private ILogger _logger;
    private PlayerDataService _playerData;
    private Dictionary<int, Relic> _relics = new();
    private Dictionary<int, IRelicEffect> _activeEffects = new();
    public IReadOnlyDictionary<int, Relic> Relics => _relics;

    public RelicService()
    {
        _logger = ServiceLocator.Get<ILogger>();
        _playerData = ServiceLocator.Get<PlayerDataService>();
    }


    public void Init()
    {
        // 1. 清空运行时数据
        ClearAllEffects();
        _relics.Clear();
        var table = ServiceLocator.Get<IConfigService>().GetTable<RelicConfig>();
        // 2. 从 PlayerDataService 读取 ID 列表
        foreach (int relicId in _playerData.RelicIds) // 需要暴露一个只读属性
        {
            var config = table.Get(relicId);
            if (config == null)
            {
                _logger.LogWarning($"遗物配置不存在: {relicId}");
                continue;
            }

            var relic = new Relic(config);
            _relics.Add(config.Id, relic);

            // 激活效果
            ActivateEffect(relic);
        }

        _logger.Log($"已加载 {_relics.Count} 个遗物");
    }

    public void AddRelic(int relicId)
    {
        if (_playerData.RelicIds.Contains(relicId))
        {
            _logger.LogWarning($"已拥有遗物 {relicId}，无法重复获得");
            return;
        }

        var config = ServiceLocator.Get<IConfigService>().GetTable<RelicConfig>().Get(relicId);
        if (config == null)
        {
            _logger.LogError($"遗物配置 {relicId} 不存在");
            return;
        }
        _playerData.AddRelic(relicId);
        var relic = new Relic(config);
        _relics.Add(config.Id, relic);

        // 激活效果
        ActivateEffect(relic);

        // 保存存档
        ServiceLocator.Get<SaveService>().SaveGame();

        // 广播事件（用于成就、UI刷新等）
     
    }

    public void RemoveRelic(int relicId)
    {
        if (!_playerData.RelicIds.Contains(relicId)) return;

        if (!_relics.TryGetValue(relicId, out var relic)) return;

        // 禁用效果
        DeactivateEffect(relic);

        _relics.Remove(relicId);
        ServiceLocator.Get<SaveService>().SaveGame();
    }


    private void ActivateEffect(Relic relic)
    {
        if (_activeEffects.ContainsKey(relic.Config.Id)) return;

        var effect = RelicEffectFactory.Create(relic.Config.Type, relic.Config.Effects);
        if (effect != null)
        {
            effect.OnActivate(relic);
            _activeEffects[relic.Config.Id] = effect;
        }
    }

    private void DeactivateEffect(Relic relic)
    {
        if (_activeEffects.TryGetValue(relic.Config.Id, out var effect))
        {
            effect.OnDeactivate(relic);
            _activeEffects.Remove(relic.Config.Id);
        }
    }
    private void ClearAllEffects()
    {
        foreach (var kv in _activeEffects)
            kv.Value.OnDeactivate(null);
        _activeEffects.Clear();
    }
    // 生命周期清理（游戏退出时）
    public void Dispose()
    {
        ClearAllEffects();
    }
}