public static class BuffFactory
{
    public static IBuff Create(int id, BuffConfig config, int stacks)
    {

        return id switch
        {
            1 => new VulnerableBuff(config, stacks),
            _ => throw new System.NotImplementedException(),
        };
    }
}