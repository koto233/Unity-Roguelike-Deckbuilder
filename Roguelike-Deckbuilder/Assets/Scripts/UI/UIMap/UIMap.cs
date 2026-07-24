using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitFramework;
using LitFramework.Asset;
using LitFramework.UI.Core.Window;
using UnityEngine;

public partial class UIMap : UIBase
{
    private LineRenderer _lineRenderer;
    private AssetRef<GameObject> _nodePrefabRef;
    private List<Transform> _rows = new List<Transform>();
    private List<UIMapNode> _nodes = new List<UIMapNode>();

    public async UniTask InitAsync()
    {
        var assetService = ServiceLocator.Get<IAssetService>();
        _nodePrefabRef = await assetService.LoadRefAsync<GameObject>("Assets/Res/UI/UIMapNode.prefab");
    }
    public Transform CreateRow()
    {
        var row = Instantiate(b_Row, b_Content).transform;
        _rows.Add(row);
        return row;
    }

    public UIMapNode CreateNode(MapNodeData data, Transform parent)
    {
        var node = Instantiate(_nodePrefabRef.Asset, parent);
        var uiMapNode = node.GetComponent<UIMapNode>();
        node.name = $"Node_{data.Id}";
        _nodes.Add(uiMapNode);
        return uiMapNode;
    }

    public void Clear()
    {
        foreach (var row in _rows) Destroy(row.gameObject);
        _rows.Clear();
        foreach (var node in _nodes) Destroy(node.gameObject);
        _nodes.Clear();
        ClearLines();
    }

    public void ClearLines()
    {
        // 如果有多个 LineRenderer 对象池，清空或隐藏
        // 简单起见，直接清除所有子 LineRenderer
        // foreach (Transform child in _lineRenderer.transform) Destroy(child.gameObject);
    }

    public void DrawLine(UIMapNode from, UIMapNode to, bool active)
    {
        // 实例化一个 LineRenderer（或使用对象池）
        var lineGO = new GameObject("Line", typeof(LineRenderer));
        lineGO.transform.SetParent(_lineRenderer.transform);
        var lr = lineGO.GetComponent<LineRenderer>();
        // 设置材质、宽度、颜色等
        lr.startColor = active ? Color.white : Color.gray;
        lr.endColor = active ? Color.white : Color.gray;
        // 设置位置：将世界坐标转换为 UI 坐标（需要 Screen Space - Camera 模式）
        // 如果 Canvas 是 Overlay，则使用 RectTransform 的 anchoredPosition 转世界坐标
        Vector3 fromPos = from.transform.position; // 对于 Overlay，transform.position 就是屏幕坐标
        Vector3 toPos = to.transform.position;
        // 如果使用 Camera，需要转换，这里简单用屏幕坐标
        lr.SetPosition(0, fromPos);
        lr.SetPosition(1, toPos);
        // 也可以添加中间点形成曲线
    }
}
