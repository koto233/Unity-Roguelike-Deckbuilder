public class Relic
{
    public RelicConfig Config { get; private set; }
    public bool IsActive { get; set; } = true; // 某些遗物可能被临时禁用

    public Relic(RelicConfig config)
    {
        Config = config;
    }
}