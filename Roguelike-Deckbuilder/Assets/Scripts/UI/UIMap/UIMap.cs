using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using LitFramework;
using LitFramework.Asset;
using LitFramework.ObjectPool;
using LitFramework.UI.Core.Window;
using UnityEngine;
using UnityEngine.UI;

public partial class UIMap : UIBase
{
    private AssetRef<GameObject> _nodePrefabRef;
    private AssetRef<GameObject> _linePrefabRef;
    private List<Transform> _rows = new();
    private List<UIMapNode> _nodes = new();
    private List<UIMapLine> _lines = new();
    private Dictionary<string, UIMapNode> _nodeDict = new Dictionary<string, UIMapNode>();
    private ObjectPoolService _poolService;
    public event System.Action<string> OnNodeClicked; // 节点点击事件，参数为节点ID

    public async UniTask InitAsync()
    {
        var assetService = ServiceLocator.Get<IAssetService>();
        _nodePrefabRef = await assetService.LoadRefAsync<GameObject>("Assets/Res/UI/UIMapNode.prefab");
        _linePrefabRef = await assetService.LoadRefAsync<GameObject>("Assets/Res/UI/UIMapLine.prefab");
        b_Row.gameObject.SetActive(false);
        InitObjectPools();
    }
    private void InitObjectPools()
    {
        _poolService = ServiceLocator.Get<ObjectPoolService>();
        _poolService.RegisterGameObjectPool<UIMapNode>(_nodePrefabRef.Asset, initialPoolSize: 10);
        _poolService.RegisterGameObjectPool<UIMapLine>(_linePrefabRef.Asset, initialPoolSize: 10);
    }
    // 外部调用：传入最新的节点数据，完全刷新地图
    public void CreateMap(List<MapNodeData> nodes)
    {
        Clear();
        if (nodes == null || nodes.Count == 0) return;

        // 1. 按行分组创建节点
        var rows = nodes.GroupBy(n => n.Row).OrderBy(g => g.Key);
        foreach (var rowGroup in rows)
        {
            var rowGO = CreateRow();
            foreach (var nodeData in rowGroup.OrderBy(n => n.Column))
            {
                var nodeView = CreateNode(nodeData, rowGO.transform);
            }
        }
        Canvas.ForceUpdateCanvases();
        // foreach (var nodeData in nodes)
        // {
        //     string nexts = nodeData.NextNodes == null ? "空" : string.Join(",", nodeData.NextNodes);
        //     Debug.Log($"节点 {nodeData.Id} (行 {nodeData.Row}) -> 指向: [{nexts}]");
        // }
        // 2. 绘制连线
        DrawAllLines(nodes);
    }
    public void RefreshMap(List<MapNodeData> updatedNodes, string currentNodeId)
    {
        // 1. 更新每个节点的视觉状态
        foreach (var data in updatedNodes)
        {
            if (_nodeDict.TryGetValue(data.Id, out var nodeView))
            {
                nodeView.UpdateState(data);
            }
        }

        // 2. 更新每条线的颜色
        foreach (var lineUI in _lines)
        {
            bool isActive = IsLineActive(lineUI.FromId, lineUI.ToId, currentNodeId, updatedNodes);
            lineUI.LineImage.color = isActive ? Color.white : new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }
    }

    // ============ 判断线条是否高亮 ============
    private bool IsLineActive(string fromId, string toId, string currentNodeId, List<MapNodeData> allNodes)
    {
        // 只有从当前节点出发的线才可能高亮
        if (fromId != currentNodeId) return false;

        // 找到目标节点数据
        var target = allNodes.FirstOrDefault(n => n.Id == toId);
        if (target == null) return false;

        // 目标未被访问且未锁定
        return !target.IsVisited && !target.IsLocked;
    }
    private Transform CreateRow()
    {
        var row = Instantiate(b_Row, b_Content).transform;
        _rows.Add(row);
        row.gameObject.SetActive(true);
        return row;
    }

    private UIMapNode CreateNode(MapNodeData data, Transform parent)
    {
        // 1. 从池里拿对象（你已经在 Clear 里回池了，这里也要用池拿）
        var nodeGO = _poolService.GetGameObject<UIMapNode>();
        nodeGO.transform.SetParent(parent, false);
        nodeGO.SetActive(true);

        var uiMapNode = nodeGO.GetComponent<UIMapNode>();
        // 2. 一行初始化：数据 + 点击回调
        uiMapNode.Initialize(data);
        uiMapNode.OnNodeClicked += OnNodeClicked;
        nodeGO.name = $"Node_{data.Id}";
        _nodes.Add(uiMapNode);
        _nodeDict[data.Id] = uiMapNode;
        return uiMapNode;
    }



    private void DrawAllLines(List<MapNodeData> nodes)
    {
        ClearLines();
        foreach (var nodeData in nodes)
        {
            if (nodeData.NextNodes == null || nodeData.NextNodes.Count == 0) continue;
            if (!_nodeDict.TryGetValue(nodeData.Id, out var fromView)) continue;

            foreach (var nextId in nodeData.NextNodes)
            {
                if (!_nodeDict.TryGetValue(nextId, out var toView)) continue;
                bool isActive = nodeData.IsVisited && !nodeData.IsLocked;
                DrawLine(fromView, toView, isActive);
            }
        }
    }

    private void DrawLine(UIMapNode from, UIMapNode to, bool active)
    {
        var lineGO = _poolService.GetGameObject<UIMapLine>();
        lineGO.transform.SetParent(b_LinesRoot);
        lineGO.SetActive(true);
        var lineUI = lineGO.GetComponent<UIMapLine>();
        var lineImage = lineUI.LineImage;
        _lines.Add(lineUI);
        lineImage.color = active ? Color.white : new Color(0.5f, 0.5f, 0.5f, 0.5f);
        UpdateLineImage(lineImage, from.transform, to.transform);
    }

    private void UpdateLineImage(Image line, Transform fromRT, Transform toRT)
    {
        Debug.Log($"fromRT: {fromRT.position}, toRT: {toRT.position}");
        // 假设 fromRT 和 toRT 有相同的父级（即都在同一个容器下）
        Vector2 fromPos = fromRT.position;
        Vector2 toPos = toRT.position;
        Vector2 dir = toPos - fromPos;
        float dist = dir.magnitude;
        Vector2 mid = (fromPos + toPos) / 2f;

        RectTransform rt = line.rectTransform;
        rt.position = mid;
        rt.sizeDelta = new Vector2(dist, 4f);
        rt.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
    }

    public void Clear()
    {
        foreach (var row in _rows)
        {
            Destroy(row.gameObject);
        }
        _rows.Clear();
        foreach (var node in _nodes)
        {
            _poolService.ReturnGameObject<UIMapNode>(node.gameObject);
        }
        _nodes.Clear();
        _nodeDict.Clear();
        ClearLines();
    }

    private void ClearLines()
    {
        foreach (var line in _lines)
        {
            line.gameObject.SetActive(false);
            _poolService.ReturnGameObject<UIMapLine>(line.gameObject);
        }
        _lines.Clear();
    }
}