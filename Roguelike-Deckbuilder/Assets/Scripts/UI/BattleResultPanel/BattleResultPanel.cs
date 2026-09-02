using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitFramework;
using LitFramework.Asset;
using LitFramework.UI.Core.Window;
using UnityEngine;

public partial class BattleResultPanel : UIWindow
{
    public event Action OnSkipClick;
    private AssetRef<GameObject> _rewardItemRef;
    private List<RewardItem> _rewardItems = new();
    public event Action OnRewardItemClick;
    protected override async UniTask OnOpenAsync()
    {
        var assetService = ServiceLocator.Get<IAssetService>();
        _rewardItemRef = await assetService.LoadRefAsync<GameObject>(UIPath.RewardItem);
    }
    void OnEnable()
    {
        SubscribeEvents();
    }
    void OnDisable()
    {
        UnsubscribeEvents();
    }
    private void SubscribeEvents()
    {
        b_SkipButton.onClick.AddListener(() => OnSkipClick?.Invoke());
    }
    private void UnsubscribeEvents()
    {
        b_SkipButton.onClick.RemoveAllListeners();

    }

    public void ShowReward(List<Reward> rewards)
    {
        foreach (var rewardItem in _rewardItems)
        {
            Destroy(rewardItem.gameObject);
        }
        _rewardItems.Clear();
        foreach (var reward in rewards)
        {
            var go = Instantiate(_rewardItemRef.Asset, b_RewardsRoot);
            go.transform.localScale = Vector3.one;
            var rewardItem = go.GetComponent<RewardItem>();
            rewardItem.Init(reward.Value);
            rewardItem.Show();
            rewardItem.OnClick += () => OnRewardItemClick?.Invoke();
            _rewardItems.Add(rewardItem);
        }
    }


}
