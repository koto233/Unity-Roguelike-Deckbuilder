using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using LitFramework;
using LitFramework.Asset;
using LitFramework.UI.Core.Window;
using UnityEngine;

public partial class UIEnemyItem : UIBase
{
    private AssetRef<GameObject> _buffPrefabAssetRef;
    [SerializeField] private UIIntentionItem _intentionUI;
    private Dictionary<int, UIBuffItem> _buffSlots = new();
    public Vector3 DamageTextPos => b_DamageTextPos.transform.position;
    public Enemy Enemy { get; private set; }

    void Start()
    {
        InitAsync().Forget();
    }
    private async UniTask InitAsync()
    {
        var assetService = ServiceLocator.Get<IAssetService>();
        _buffPrefabAssetRef = await assetService.LoadRefAsync<GameObject>("Assets/Res/UI/UIBuffItem.prefab");
        Enemy.DetermineIntent(null);
    }
    public void UpdateHP(int currentHp, int maxHp)
    {
        b_HPText.SetText($"{currentHp}/{maxHp}");
        b_HPSlider.DOValue((float)currentHp / maxHp, 0.5f).SetEase(Ease.Linear);
    }
    public void SetEnemy(Enemy enemy)
    {
        Enemy = enemy;
    }


    public void RefreshBuffs(List<IBuff> buffs)
    {
        // 1. 收集当前需要显示的 Buff Id
        HashSet<int> activeIds = new();
        foreach (var buff in buffs)
        {
            activeIds.Add(buff.Config.Id);
        }

        // 2. 移除已经不存在的 Buff UI
        List<int> toRemove = new List<int>();
        foreach (var kv in _buffSlots)
        {
            if (!activeIds.Contains(kv.Key))
                toRemove.Add(kv.Key);
        }
        foreach (var id in toRemove)
        {
            Destroy(_buffSlots[id].gameObject);
            _buffSlots.Remove(id);
        }

        // 3. 添加或更新现有的 Buff UI
        foreach (var buff in buffs)
        {
            if (_buffSlots.TryGetValue(buff.Config.Id, out UIBuffItem slot))
            {
                // 更新层数（如果层数变化）
                slot.SetStacks(buff.Stacks);
            }
            else
            {
                // 新建 Slot
                GameObject go = Instantiate(_buffPrefabAssetRef.Asset);
                go.transform.SetParent(b_BuffRoot.transform);
                slot = go.GetComponent<UIBuffItem>();
                slot.Init(buff);
                slot.SetStacks(buff.Stacks);
                _buffSlots[buff.Config.Id] = slot;
            }
        }
    }
    public void RefreshIntent(IntentConfig intentConfig)
    {
        if (intentConfig != null && _intentionUI != null)
        {
            _intentionUI.Show();
            _intentionUI.Init(intentConfig);
        }
        else
        {
            // b_IntentionIcon.gameObject.SetActive(false);
            // _intentionUI.Hide();
        }
    }
    // 清理（战斗结束）
    public void ClearBuffs()
    {
        foreach (var kv in _buffSlots)
            Destroy(kv.Value.gameObject);
        _buffSlots.Clear();
    }
    void OnDestroy()
    {
        _buffPrefabAssetRef?.Dispose();
    }
}
