using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitFramework;
using LitFramework.Asset;
using LitFramework.UI.Core.Window;
using UnityEngine;
using UnityEngine.UI;

public partial class UIIntentionItem : UIBase, ITooltipDataProvider
{
    private IntentConfig _intentConfig;
    private Image _icon;
    void Start()
    {
        _icon = GetComponent<Image>();
    }
    public void Init(IntentConfig intentConfig, int num)
    {
        _intentConfig = intentConfig;
        SetIcon().Forget();
        b_Num.SetText(num.ToString());
        _intentConfig.Description = string.Format(_intentConfig.Description, num);
        Debug.Log("意图" + _intentConfig.Description);
    }
    private async UniTask SetIcon()
    {
        _icon.sprite = await ServiceLocator.Get<IAssetService>().LoadAsync<Sprite>($"Assets/Res/Art/Intents/intent_{_intentConfig.Key}.png");
    }
    public TooltipData GetTooltipData()
    {
        return new TooltipData
        {
            Description = _intentConfig.Description,
        };
    }
}
