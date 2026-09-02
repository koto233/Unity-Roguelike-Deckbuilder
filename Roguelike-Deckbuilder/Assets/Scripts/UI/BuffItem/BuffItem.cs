using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitFramework;
using LitFramework.Asset;
using LitFramework.EventBus;
using LitFramework.UI.Core.Window;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public partial class BuffItem : UIBase, ITooltipDataProvider
{
    private Image _icon;
    private IBuff _buff;
    protected override void Awake()
    {
        base.Awake();
        _icon = GetComponent<Image>();
    }
    public TooltipData GetTooltipData()
    {
        return new TooltipData
        {
            Description = string.Format(_buff.Config.Description, _buff.Config.Value),
            Value = _buff.Stacks
        };
    }

    public void Init(IBuff buff)
    {
        // 加载图标
        _buff = buff;
        SetIcon().Forget();
    }
    private async UniTask SetIcon()
    {
        _icon.sprite = await ServiceLocator.Get<IAssetService>().LoadAsync<Sprite>($"Assets/Res/Art/Powers/{_buff.Config.Key}.png");
    }

    public void SetStacks(int stacks)
    {
        b_StackText.SetText(stacks.ToString());
        b_StackText.gameObject.SetActive(stacks > 1);
    }
}
