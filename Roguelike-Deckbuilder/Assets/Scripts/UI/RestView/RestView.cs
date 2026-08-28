using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using LitFramework;
using LitFramework.Asset;
using LitFramework.Config;
using LitFramework.ObjectPool;
using LitFramework.UI.Core.Window;
using UnityEngine;

public partial class RestView : UIWindow
{
    public event Action OnClickContinue;
    public event Action OnClickForge;
    public event Action OnClickRest;
    private ObjectPoolService _poolService;
    private AssetRef<GameObject> _upgradeCardItemRef;
    private List<UpgradeCardItem> _cardItems = new();
    public event Action<CardConfig> OnCardSelected;
    public event Action OnConfirmClicked;

    protected override async UniTask OnOpenAsync()
    {
        await base.OnOpenAsync();
        var assetService = ServiceLocator.Get<IAssetService>();
        _upgradeCardItemRef = await assetService.LoadRefAsync<GameObject>(UIPath.UpgradeCardItem);
        InitObjectPools();
    }
    private void OnEnable()
    {
        SubscribeEvents();
        b_ContinueButton.gameObject.SetActive(false);
        b_ForgePanel.gameObject.SetActive(false);
        b_ConfirmPanel.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }
    private void InitObjectPools()
    {
        _poolService = ServiceLocator.Get<ObjectPoolService>();
        _poolService.RegisterGameObjectPool<UpgradeCardItem>(_upgradeCardItemRef.Asset, initialPoolSize: 10);

    }
    private void SubscribeEvents()
    {
        b_ContinueButton.onClick.AddListener(() => OnClickContinue?.Invoke());
        b_ForgeButton.onClick.AddListener(() => OnClickForge?.Invoke());
        b_RestButton.onClick.AddListener(() => OnClickRest?.Invoke());
    }

    private void UnsubscribeEvents()
    {
        b_ContinueButton.onClick.RemoveAllListeners();
        b_ForgeButton.onClick.RemoveAllListeners();
        b_RestButton.onClick.RemoveAllListeners();
    }
    public void ShowContinue()
    {
        b_RestButton.gameObject.SetActive(false);
        b_ForgeButton.gameObject.SetActive(false);
        b_ContinueButton.gameObject.SetActive(true);
    }
    public void OpenForge()
    {
        b_ForgePanel.gameObject.SetActive(true);
    }
    public void CloseForge()
    {
        b_ForgePanel.gameObject.SetActive(false);
        b_ConfirmPanel.gameObject.SetActive(false);
    }

    public void CreateUpgradeList(List<Card> cards)
    {
        ClearList();
        foreach (var card in cards)
        {
            var item = _poolService.Get<UpgradeCardItem>();
            item.Init(card);
            // 点击时发事件，不存状态
            item.OnClick += config => OnCardSelected?.Invoke(config);
            item.transform.SetParent(b_ForgeListRoot);
            _cardItems.Add(item);
        }
    }

    public void ShowConfirm(CardConfig config, CardConfig targetConfig)
    {
        // 1. 防御
        if (config == null || targetConfig == null)
        {
            Debug.LogError("ShowConfirm: config or targetConfig is null");
            return;
        }

        // 2. 清空容器（回收旧卡）
        ClearContainer(b_BeforeUpgrade);
        ClearContainer(b_AfterUpgrade);

        // 3. 创建并填充新卡
        CreateUpgradeCard(config, b_BeforeUpgrade);
        CreateUpgradeCard(targetConfig, b_AfterUpgrade);

        // 4. 显示面板
        b_ConfirmPanel.gameObject.SetActive(true);
    }

    // ---- 辅助方法 ----
    private void ClearContainer(Transform container)
    {
        // 反向遍历，因为 childCount 会动态变化
        for (int i = container.childCount - 1; i >= 0; i--)
        {
            Transform child = container.GetChild(i);
            UpgradeCardItem item = child.GetComponent<UpgradeCardItem>();
            if (item != null)
                _poolService.RegisterGameObjectPool<UpgradeCardItem>(item.gameObject);
            else
                Destroy(child.gameObject); // 保险
        }
    }

    private void CreateUpgradeCard(CardConfig config, Transform parent)
    {
        UpgradeCardItem item = _poolService.Get<UpgradeCardItem>();
        item.transform.SetParent(parent, false);
        // 重置本地变换（防止残留）
        item.transform.localPosition = Vector3.zero;
        item.transform.localScale = Vector3.one;
        // 如果是 RectTransform，可以重置 anchoredPosition / sizeDelta
        if (item.transform is RectTransform rect)
        {
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = Vector2.zero; // 视情况而定
        }
        item.Init(new Card(config));
    }

    private void ClearList()
    {
        foreach (var item in _cardItems)
            _poolService.RegisterGameObjectPool<UpgradeCardItem>(item.gameObject);
        _cardItems.Clear();
    }
}
