using System.Collections;
using System.Collections.Generic;
using LitFramework.UI.Core.Window;
using UnityEngine;
using UnityEngine.UI;

public partial class RewardItem : UIBase
{
    private Button _clickButton;
    public event System.Action OnClick;
    protected override void Awake()
    {
        base.Awake();
        _clickButton = GetComponent<Button>();
    }
    void OnEnable()
    {
        _clickButton.onClick.AddListener(() =>
        {
            OnClick?.Invoke();
        });

    }
    void OnDisable()
    {
        _clickButton.onClick.RemoveAllListeners();
    }
    public void Init(int coinAmount)
    {
        b_Desc.SetText($"X {coinAmount}");
    }

}
