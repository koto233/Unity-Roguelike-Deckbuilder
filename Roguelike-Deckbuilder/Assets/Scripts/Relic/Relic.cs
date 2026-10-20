using System.Collections.Generic;

public class Relic
{
    public RelicConfig Config { get; private set; }
    public IReadOnlyList<RelicEffectsConfig> Effects { get; private set; }
    public bool IsActive { get; set; } = true; // 某些遗物可能被临时禁用

    public Relic(RelicConfig config, IReadOnlyList<RelicEffectsConfig> effects)
    {
        Config = config;
        Effects = effects ?? new List<RelicEffectsConfig>();
    }
}
