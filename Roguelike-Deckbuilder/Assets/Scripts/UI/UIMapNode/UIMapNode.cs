using LitFramework.UI.Core.Window;
using UnityEngine;
using UnityEngine.UI;

public partial class UIMapNode : UIBase
{
    private MapNodeData _nodeData;
    public event System.Action<string> OnNodeClicked;
    private string _nodeId;
    public MapNodeData GetNodeData() => _nodeData;
    // ========== 对外唯一入口 ==========
    public void Initialize(MapNodeData data)
    {
        _nodeData = data;
        _nodeId = data.Id;

        // 1. 清理旧监听（池复用必须）
        b_Button.onClick.RemoveAllListeners();
        b_Button.onClick.AddListener(() => OnNodeClicked?.Invoke(_nodeData.Id));
        // 2. 一次性刷新所有视觉
        Refresh();
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

        // --- 颜色（优先级：起点 > 访问 > 类型） ---
        Color color = isStart ? Color.blue : GetTypeColor(_nodeData.Type);
        if (isVisited) color = Color.gray;
        b_Icon.color = color;

        // --- 高亮 ---
        b_HighLight.gameObject.SetActive(!isVisited && !isStart);

        // --- 锁 ---
        b_Lock.gameObject.SetActive(isLocked);

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

    private Color GetTypeColor(MapNodeType type)
    {
        return type switch
        {
            MapNodeType.Battle => Color.red,
            MapNodeType.Elite => new Color(1f, 0.7f, 0f),
            MapNodeType.Rest => Color.green,
            MapNodeType.Shop => new Color(0.8f, 0.6f, 0f),
            MapNodeType.Event => Color.yellow,
            MapNodeType.Boss => new Color(0.5f, 0f, 0.5f),
            _ => Color.gray
        };
    }
}