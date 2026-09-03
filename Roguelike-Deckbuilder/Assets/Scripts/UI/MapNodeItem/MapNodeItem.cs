using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitFramework;
using LitFramework.Asset;
using LitFramework.UI.Core.Window;
using UnityEngine;
using UnityEngine.UI;

public partial class MapNodeItem : UIBase
{
    private MapNodeData _nodeData;
    public event System.Action<string> OnNodeClicked;
    private string _nodeId;
    public MapNodeData GetNodeData() => _nodeData;
    private AssetRef<Sprite> _iconRef;
    private static readonly IReadOnlyDictionary<MapNodeType, string> IconPathMap =
    new Dictionary<MapNodeType, string>
    {
        [MapNodeType.Start] = "Assets/Res/Art/MapNode/Start.png",
        [MapNodeType.Battle] = "Assets/Res/Art/MapNode/Battle.png",
        [MapNodeType.Elite] = "Assets/Res/Art/MapNode/Elite.png",
        [MapNodeType.Rest] = "Assets/Res/Art/MapNode/Rest.png",
        [MapNodeType.Event] = "Assets/Res/Art/MapNode/Event.png",
        [MapNodeType.Boss] = "Assets/Res/Art/MapNode/Boss.png",
        [MapNodeType.Shop] = "Assets/Res/Art/MapNode/Shop.png",
    };
    // ========== 对外唯一入口 ==========
    public void Initialize(MapNodeData data)
    {
        _nodeData = data;
        _nodeId = data.Id;
        SetIcon(data.Type).Forget();
        // 1. 清理旧监听（池复用必须）
        b_Button.onClick.RemoveAllListeners();
        b_Button.onClick.AddListener(() => OnNodeClicked?.Invoke(_nodeData.Id));
        // 2. 一次性刷新所有视觉
        Refresh();
    }


    private async UniTask SetIcon(MapNodeType type)
    {
        var assetService = ServiceLocator.Get<IAssetService>();
        _iconRef = await assetService.LoadRefAsync<Sprite>(IconPathMap[type]);
        // Debug.Log($"加载完成，asset: {_iconRef.Asset}, 类型: {_iconRef.Asset?.GetType()}");
        b_Icon.sprite = _iconRef.Asset;
    }
    // ========== 回收时调用 ==========
    public void ResetNode()
    {
        b_Button.onClick.RemoveAllListeners();
        _nodeData = null;
        _nodeId = null;
    }

    public void UpdateState(MapNodeData data)
    {
        _nodeData = data;
        Refresh();
    }

    private void Refresh()
    {
        bool isStart = _nodeData.IsStart;
        bool isVisited = _nodeData.IsVisited;
        bool isLocked = _nodeData.IsLocked;
        bool isInteractable = _nodeData.IsInteractable;
        // --- 名称 ---
        b_Name.text = isStart ? "起点" : GetTypeName(_nodeData.Type);
        b_Icon.color = isVisited ? Color.gray : Color.white;
        // --- 高亮 ---
        // b_HighLight.gameObject.SetActive(!isVisited && !isStart);

        // --- 交互 ---
        b_Button.interactable = isInteractable;
    }

    private string GetTypeName(MapNodeType type)
    {
        return type switch
        {
            MapNodeType.Battle => "战斗",
            MapNodeType.Elite => "精英",
            MapNodeType.Rest => "休息",
            MapNodeType.Shop => "商店",
            MapNodeType.Event => "事件",
            MapNodeType.Boss => "Boss",
            _ => "未知"
        };
    }
    public void Clear()
    {
        _iconRef?.Dispose();
        _iconRef = null;
        OnNodeClicked = null;
    }
}