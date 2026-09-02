using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using LitFramework;
using LitFramework.Asset;
using LitFramework.UI.Core.Window;
using UnityEngine;

public partial class EnemyItem : UIBase
{
    private AssetRef<GameObject> _buffItemRef;
    private AssetRef<GameObject> _intentionItemRef;
    private Dictionary<int, BuffItem> _buffSlots = new();
    public Vector3 DamageTextPos => b_DamageTextPos.transform.position;
    private List<IntentionItem> _intentionItems = new(5);
    public Enemy Enemy { get; private set; }

    void Start()
    {
        InitAsync().Forget();
    }
    private async UniTask InitAsync()
    {
        var assetService = ServiceLocator.Get<IAssetService>();
        _buffItemRef = await assetService.LoadRefAsync<GameObject>(UIPath.BuffItem);
        _intentionItemRef = await assetService.LoadRefAsync<GameObject>(UIPath.IntentionItem);
        Enemy.DetermineAction();
    }

    public void RefreshBlock(int oldBlock, int newBlock)
    {
        if (newBlock <= 0)
        {
            b_BlockNum.transform.parent.gameObject.SetActive(false);
            return;
        }
        b_BlockNum.transform.parent.gameObject.SetActive(newBlock > 0);
        NumberAnimator.Play(b_BlockNum, oldBlock, newBlock, 0.5f);
    }
    public void RefreshHP(int currentHp, int maxHp)
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
            if (_buffSlots.TryGetValue(buff.Config.Id, out BuffItem slot))
            {
                // 更新层数（如果层数变化）
                slot.SetStacks(buff.Stacks);
            }
            else
            {
                // 新建 Slot
                GameObject go = Instantiate(_buffItemRef.Asset);
                go.transform.SetParent(b_BuffRoot.transform);
                slot = go.GetComponent<BuffItem>();
                slot.Init(buff);
                slot.SetStacks(buff.Stacks);
                _buffSlots[buff.Config.Id] = slot;
            }
        }
    }
    public void RefreshIntent(List<IntentConfig> intentConfigs)
    {
        foreach (var config in intentConfigs)
        {
            var _intentionGo = Instantiate(_intentionItemRef.Asset);
            _intentionGo.SetActive(true);
            _intentionGo.transform.SetParent(b_IntentionRoot.transform);
            var intentionUI = _intentionGo.GetComponent<IntentionItem>();
            intentionUI.Init(config, 5);
            _intentionItems.Add(intentionUI);
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
        _buffItemRef?.Dispose();
    }
}
