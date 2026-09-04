using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using LitFramework;
using LitFramework.Asset;
using LitFramework.UI.Core.Window;
using UnityEngine;

public partial class TopBar : UIWindow
{
    private AssetRef<GameObject> _relicItemRef;
    public event System.Action OnClickSetting;
    public event System.Action OnClickMap;
    public event System.Action OnClickDeck;
    private List<RelicItem> _relicItems = new();
    [SerializeField] private Tooltip _tooltip;
    public void Init()
    {

    }
    protected async override UniTask OnOpenAsync()
    {
        _tooltip.Hide();
        var assetService = ServiceLocator.Get<IAssetService>();
        _relicItemRef = await assetService.LoadRefAsync<GameObject>(UIPath.RelicItem);
        await base.OnOpenAsync();

    }
    private void SubscribeEvents()
    {
        b_SettingBtn.onClick.AddListener(() => OnClickSetting?.Invoke());
        b_MapBtn.onClick.AddListener(() => OnClickMap?.Invoke());
        b_DeckBtn.onClick.AddListener(() => OnClickDeck?.Invoke());
    }
    void OnEnable()
    {
        SubscribeEvents();
    }
    void OnDisable()
    {
        b_SettingBtn.onClick.RemoveAllListeners();
        b_MapBtn.onClick.RemoveAllListeners();
        b_DeckBtn.onClick.RemoveAllListeners();
    }
    public void RefreshCoin(int oldCoin, int newCoin)
    {
        NumberAnimator.Play(b_CoinText, oldCoin, newCoin, 0.5f);
    }
    public void RefreshHp(int currentHp, int maxHp)
    {
        b_HpText.SetText(currentHp + "/" + maxHp);
    }
    public void RefreshRelics(List<RelicDisplayData> relicDataList)
    {
        foreach (var item in _relicItems)
        {
            Destroy(item.gameObject);
        }
        _relicItems.Clear();
        for (int i = 0; i < relicDataList.Count; i++)
        {
            if (i >= _relicItems.Count)
            {
                var item = Instantiate(_relicItemRef.Asset, b_RelicRoot);
                var relicItem = item.GetComponent<RelicItem>();
                _relicItems.Add(relicItem);
            }
            _relicItems[i].Init(relicDataList[i]);
        }
    }
    public void ShowToolTip(TooltipData data, Vector2 position)
    {
        _tooltip.Show(data, position);
    }
    public void HideToolTip()
    {
        _tooltip.Hide();
    }
}
