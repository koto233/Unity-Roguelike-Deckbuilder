using Cysharp.Threading.Tasks;
using LitFramework;
using LitFramework.Asset;
using UnityEngine;

public class IntentDisplayData
{
    public string Description;
    public int Value;
    public Sprite Icon;
    public bool IsBuff;
    public static async UniTask<IntentDisplayData> CreateAsync(IntentConfig config, int value)
    {
        var data = new IntentDisplayData
        {
            Value = value,
            Description = string.Format(config.Description, value),
            IsBuff = config.Type == "Buff" || config.Type == "Debuff" || config.Type == "StatusCard"
        };
        data.Icon = await ServiceLocator.Get<IAssetService>().LoadAsync<Sprite>($"Assets/Res/Art/Intents/intent_{config.Icon}.png");
        return data;
    }
}
