using System;
using System.Collections.Generic;
using System.Linq;
using LitFramework;
using LitFramework.Config;

public class RelicService
{
    private ILogger _logger;
    private PlayerDataService _playerData;
    private Dictionary<int, Relic> _relics = new();
    private Dictionary<int, List<IRelicEffect>> _activeEffects = new();
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

        var relicTable = ServiceLocator.Get<IConfigService>().GetTable<RelicConfig>();
        if (relicTable == null)
        {
            _logger.LogError("遗物配置表未加载: RelicConfig");
            return;
        }

        // 2. 从 PlayerDataService 读取 ID 列表，并按 RelicId 关联效果配置
        foreach (int relicId in _playerData.RelicIds)
        {
            var config = relicTable.Get(relicId);
            if (config == null)
            {
                _logger.LogWarning($"遗物配置不存在: {relicId}");
                continue;
            }

            var relic = CreateRelic(config);
            _relics.Add(config.Id, relic);

            // 激活效果：单个遗物效果异常不应影响其他遗物
            try
            {
                ActivateEffects(relic);
            }
            catch (Exception e)
            {
                _logger.LogError($"遗物 {relicId} 效果激活失败: {e.Message}");
            }
        }

        _logger.Log($"已加载 {_relics.Count} 个遗物");
    }

    public bool CanAddRelic(int relicId)
    {
        if (_relics.ContainsKey(relicId)) return false;
        if (_playerData.RelicIds.Contains(relicId)) return false;

        var table = ServiceLocator.Get<IConfigService>().GetTable<RelicConfig>();
        return table != null && table.Get(relicId) != null;
    }

    public bool AddRelic(int relicId)
    {
        if (!CanAddRelic(relicId))
        {
            _logger.LogWarning($"无法添加遗物 {relicId}（已拥有或配置不存在）");
            return false;
        }

        var config = ServiceLocator.Get<IConfigService>().GetTable<RelicConfig>().Get(relicId);
        var relic = CreateRelic(config);

        // 先激活效果，失败则不写入玩家数据，保证“要么完整获得，要么没有”
        try
        {
            ActivateEffects(relic);
        }
        catch (Exception e)
        {
            _logger.LogError($"遗物 {relicId} 效果激活失败: {e.Message}");
            return false;
        }

        _relics.Add(config.Id, relic);
        _playerData.AddRelic(relicId); // 会发布 RelicChangedEvent
        SaveGameSafe();
        return true;
    }

    public void RemoveRelic(int relicId)
    {
        if (!_playerData.RelicIds.Contains(relicId)) return;

        if (_relics.TryGetValue(relicId, out var relic))
        {
            // 禁用效果
            DeactivateEffects(relic);
            _relics.Remove(relicId);
        }

        _playerData.RemoveRelic(relicId); // 会发布 RelicChangedEvent
        SaveGameSafe();
    }

    public void RemoveAllRelics()
    {
        foreach (var relic in _relics.Values.ToList())
            DeactivateEffects(relic);

        _relics.Clear();
        _activeEffects.Clear();
        _playerData.ClearRelics(); // 会发布 RelicChangedEvent
        SaveGameSafe();
    }

    private Relic CreateRelic(RelicConfig config)
    {
        return new Relic(config, GetEffectConfigs(config.Id));
    }

    /// <summary>
    /// 从 RelicEffects 表取出该遗物的所有效果，按 Order、Id 排序。
    /// 一个遗物可以配置多个效果。
    /// </summary>
    private List<RelicEffectsConfig> GetEffectConfigs(int relicId)
    {
        var table = ServiceLocator.Get<IConfigService>().GetTable<RelicEffectsConfig>();
        if (table == null)
        {
            _logger.LogError("遗物效果配置表未加载: RelicEffects");
            return new List<RelicEffectsConfig>();
        }

        return table.GetAll()
            .Where(e => e.RelicId == relicId)
            .OrderBy(e => e.Order)
            .ThenBy(e => e.Id)
            .ToList();
    }

    private void ActivateEffects(Relic relic)
    {
        if (_activeEffects.ContainsKey(relic.Config.Id)) return;

        var effects = new List<IRelicEffect>();
        try
        {
            foreach (var effectConfig in relic.Effects)
            {
                var effect = RelicEffectFactory.Create(effectConfig);
                if (effect == null) continue;

                effect.OnActivate(relic);
                effects.Add(effect);
            }
        }
        catch
        {
            // 回滚已经激活的效果，避免半激活状态
            foreach (var effect in effects)
                effect.OnDeactivate(relic);
            throw;
        }

        if (effects.Count > 0)
            _activeEffects[relic.Config.Id] = effects;
    }

    private void DeactivateEffects(Relic relic)
    {
        if (!_activeEffects.TryGetValue(relic.Config.Id, out var effects)) return;

        foreach (var effect in effects)
            effect.OnDeactivate(relic);

        _activeEffects.Remove(relic.Config.Id);
    }

    private void ClearAllEffects()
    {
        foreach (var relic in _relics.Values.ToList())
            DeactivateEffects(relic);

        _activeEffects.Clear();
    }

    private void SaveGameSafe()
    {
        try
        {
            ServiceLocator.Get<SaveService>().SaveGame();
        }
        catch (Exception e)
        {
            // 存档失败不应该让已经获得的遗物回滚
            _logger.LogError($"保存存档失败: {e.Message}");
        }
    }

    // 生命周期清理（游戏退出时）
    public void Dispose()
    {
        ClearAllEffects();
    }
}
