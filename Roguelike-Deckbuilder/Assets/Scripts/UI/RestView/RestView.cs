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
    private List<ClickableCardItem> _cardItems = new();
    public event Action<int> OnCardSelected;
    public event Action OnConfirmClicked;

    protected override async UniTask OnOpenAsync()
    {
        await base.OnOpenAsync();
        var assetService = ServiceLocator.Get<IAssetService>();
        _upgradeCardItemRef = await assetService.LoadRefAsync<GameObject>(UIPath.ClickableCardItem);
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
        _poolService.RegisterGameObjectPool<ClickableCardItem>(_upgradeCardItemRef.Asset, initialPoolSize: 10);

    }
    private void SubscribeEvents()
    {
        b_ContinueButton.onClick.AddListener(() => OnClickContinue?.Invoke());
        b_ForgeButton.onClick.AddListener(() => OnClickForge?.Invoke());
        b_RestButton.onClick.AddListener(() => OnClickRest?.Invoke());
        b_ConfirmButton.onClick.AddListener(() => OnConfirmClicked?.Invoke());
    }

    private void UnsubscribeEvents()
    {
        b_ContinueButton.onClick.RemoveAllListeners();
        b_ForgeButton.onClick.RemoveAllListeners();
        b_RestButton.onClick.RemoveAllListeners();
        b_ConfirmButton.onClick.RemoveAllListeners();
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
        Debug.Log("CreateUpgradeList: " + _cardItems.Count);
        foreach (var card in cards)
        {
            var item = _poolService.GetGameObject<ClickableCardItem>();
            var upgradeCardItem = item.GetComponent<ClickableCardItem>();

            upgradeCardItem.Init(CardDisplayData.FromConfig(card.Config));
            // 点击时发事件，不存状态
            upgradeCardItem.OnClick += id => OnCardSelected?.Invoke(id);
            item.transform.SetParent(b_ForgeListRoot);
            _cardItems.Add(upgradeCardItem);
        }
    }

    public void ShowConfirm(CardDisplayData display, CardDisplayData targetDisplay)
    {
        // 1. 防御
        if (display == null || targetDisplay == null)
        {
            Debug.LogError("ShowConfirm: config or targetConfig is null");
            return;
        }

        // 2. 清空容器（回收旧卡）
        ClearContainer(b_BeforeUpgrade);
        ClearContainer(b_AfterUpgrade);

        // 3. 创建并填充新卡
        CreateUpgradeCard(display, b_BeforeUpgrade);
        CreateUpgradeCard(targetDisplay, b_AfterUpgrade);

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
            ClickableCardItem item = child.GetComponent<ClickableCardItem>();
            if (item != null)
                _poolService.ReturnGameObject<ClickableCardItem>(item.gameObject);
            else
                Destroy(child.gameObject); // 保险
        }
    }

    private void CreateUpgradeCard(CardDisplayData data, Transform parent)
    {
        var item = _poolService.GetGameObject<ClickableCardItem>();
        var upgradeCardItem = item.GetComponent<ClickableCardItem>();
        item.transform.SetParent(parent, false);
        upgradeCardItem.Init(data);
    }

    private void ClearList()
    {
        foreach (var item in _cardItems)
        {
            Debug.Log("ClearList: " + item.name);
            _poolService.ReturnGameObject<ClickableCardItem>(item.gameObject);
        }

        _cardItems.Clear();
    }
}
