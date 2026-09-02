using UnityEngine;

public class RewardDisplayData
{
    public Sprite Icon;
    public string Desc;
    public static RewardDisplayData Create(Reward reward)
    {
        RewardDisplayData data = new RewardDisplayData();
        return data;
    }
}

