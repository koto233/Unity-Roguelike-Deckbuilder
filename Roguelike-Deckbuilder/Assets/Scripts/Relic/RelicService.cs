using System.Collections.Generic;
using System.Linq;
using LitFramework;
using LitFramework.Config;
using LitFramework.EventBus;

public class RelicService
{
    private List<Relic> _relics = new();
    private Dictionary<int, IRelicEffect> _activeEffects = new();
    public IReadOnlyList<Relic> Relics => _relics;

    // 添加遗物（从商店/事件获得）
    public void AddRelic(int relicId)
    {
        if (HasRelic(relicId))
        {
            // Debug.Log($"已拥有遗物 {relicId}，无法重复获得");
            return;
        }

        // var config = ServiceLocator.Get<IConfigService>().GetRelicConfig(relicId);
        // if (config == null)
        // {
        //     // Debug.LogError($"遗物配置 {relicId} 不存在");11
        //     return;
        // }

        // var relic = new Relic(config);
        // _relics.Add(relic);

        // // 激活效果
        // ActivateEffect(relic);

        // 保存存档
        // ServiceLocator.Get<ISaveService>().Save();

        // 广播事件（用于成就、UI刷新等）
        // EventBus<RelicAcquiredEvent>.Publish(new RelicAcquiredEvent { RelicId = relicId });
    }

    public void RemoveRelic(int relicId)
    {
        var relic = _relics.FirstOrDefault(r => r.Config.Id == relicId);
        if (relic == null) return;

        // 禁用效果
        DeactivateEffect(relic);

        _relics.Remove(relic);
        // ServiceLocator.Get<ISaveStorage>().Save();
        // EventBus<RelicLostEvent>.Publish(new RelicLostEvent { RelicId = relicId });
    }

    public bool HasRelic(int relicId) => _relics.Any(r => r.Config.Id == relicId);

    // 从存档恢复
    public void LoadRelics(List<int> relicIds)
    {
        var table = ServiceLocator.Get<IConfigService>().GetTable<RelicConfig>();
        // 清空旧效果
        foreach (var relic in _relics)
            DeactivateEffect(relic);
        _relics.Clear();

        foreach (var id in relicIds)
        {
            var config = table.Get(id);
            if (config == null) continue;
            var relic = new Relic(config);
            _relics.Add(relic);
            ActivateEffect(relic);
        }
    }

    private void ActivateEffect(Relic relic)
    {
        if (_activeEffects.ContainsKey(relic.Config.Id)) return;

        // var effect = RelicEffectFactory.Create(relic.Config.Type, relic.Config.EffectParams);
        // if (effect != null)
        // {
        //     effect.OnActivate(relic);
        //     _activeEffects[relic.Config.Id] = effect;
        // }
    }

    private void DeactivateEffect(Relic relic)
    {
        if (_activeEffects.TryGetValue(relic.Config.Id, out var effect))
        {
            effect.OnDeactivate(relic);
            _activeEffects.Remove(relic.Config.Id);
        }
    }

    // 生命周期清理（游戏退出时）
    public void Dispose()
    {
        foreach (var kv in _activeEffects)
            kv.Value.OnDeactivate(_relics.First(r => r.Config.Id == kv.Key));
        _activeEffects.Clear();
        _relics.Clear();
    }
}