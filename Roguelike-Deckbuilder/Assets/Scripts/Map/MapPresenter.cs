using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LitFramework;
using LitFramework.EventBus;
using UnityEngine;


public class MapPresenter
{
    private MapService _mapService;

    [SerializeField] private UIMap _view;

    private List<UIMapNode> _nodeViews = new List<UIMapNode>();
    private Dictionary<string, UIMapNode> _nodeDict = new Dictionary<string, UIMapNode>();

    // private void Awake()
    // {
    //     _eventBus.Subscribe<MapChangedEvent>(OnMapChanged);
    //     _eventBus.Subscribe<MapGeneratedEvent>(OnMapGenerated);
    //     // 其他事件：战斗结束返回地图等，触发 MapChangedEvent
    // }

    // private void OnDestroy()
    // {
    //     _eventBus.Unsubscribe<MapChangedEvent>(OnMapChanged);
    //     _eventBus.Unsubscribe<MapGeneratedEvent>(OnMapGenerated);
    // }

    // private void OnMapGenerated(MapGeneratedEvent evt)
    // {
    //     GenerateMapUI(evt.Nodes);
    // }

    // private void OnMapChanged(MapChangedEvent evt)
    // {
    //     // 局部刷新：只更新受影响的节点及其连线
    //     if (!string.IsNullOrEmpty(evt.ChangedNodeId))
    //     {
    //         RefreshNode(evt.ChangedNodeId);
    //         RefreshLines();
    //     }
    //     else
    //     {
    //         // 全量刷新（安全模式）
    //         RefreshAll();
    //     }
    // }

    private void GenerateMapUI(List<MapNodeData> nodes)
    {
        // 清除旧 UI（如果已有）
        _view.Clear();

        // 按行分组
        var rows = nodes.GroupBy(n => n.Row).OrderBy(g => g.Key);
        foreach (var rowGroup in rows)
        {
            var rowGO = _view.CreateRow(); // 创建行容器
            foreach (var nodeData in rowGroup.OrderBy(n => n.Column))
            {
                var nodeView = _view.CreateNode(nodeData, rowGO.transform);
                _nodeDict[nodeData.Id] = nodeView;
                // 绑定点击事件
                // nodeView.Button.onClick.AddListener(() => OnNodeClicked(nodeData.Id));
                // 初始状态
                UpdateNodeVisual(nodeView, nodeData);
            }
        }

        // 绘制所有连线
        DrawAllLines(nodes);
    }

    private void RefreshNode(string nodeId)
    {
        if (_nodeDict.TryGetValue(nodeId, out var nodeView))
        {
            var data = _mapService.GetNode(nodeId);
            if (data != null)
                UpdateNodeVisual(nodeView, data);
        }
    }

    private void RefreshAll()
    {
        foreach (var kv in _nodeDict)
        {
            var data = _mapService.GetNode(kv.Key);
            if (data != null)
                UpdateNodeVisual(kv.Value, data);
        }
        RefreshLines();
    }

    private void UpdateNodeVisual(UIMapNode view, MapNodeData data)
    {
        // // 设置图标
        // view.SetType(data.Type);
        // // 锁定状态
        // view.SetLocked(data.IsLocked);
        // view.Button.interactable = !data.IsLocked && !data.IsVisited && _mapService.CanSelectNode(data.Id);
        // // 是否已访问（当前所在节点）
        // view.SetVisited(data.IsVisited);
        // // 是否起始节点（特殊标记）
        // view.SetStart(data.IsStart);
        // // 如果当前是玩家所在节点，额外高亮
        // // 可通过 MapService 的 CurrentNodeId 获取
    }

    private void OnNodeClicked(string nodeId)
    {
        var data = _mapService.GetNode(nodeId);
        if (data == null || data.IsLocked || data.IsVisited) return;
        if (!_mapService.CanSelectNode(nodeId)) return;

        // 访问该节点（内部会解锁相邻节点并发送 MapChangedEvent）
        _mapService.VisitNode(nodeId);
        // 然后根据类型触发相应事件（如 BattleStartEvent）
        // 由上层处理战斗/商店等逻辑
    }

    private void DrawAllLines(List<MapNodeData> nodes)
    {
        _view.ClearLines();
        foreach (var node in nodes)
        {
            if (node.NextNodes == null || node.NextNodes.Count == 0) continue;
            // 只绘制从已访问节点出发的路径？也可以全部绘制但用颜色区分
            // 建议：全部绘制，未解锁路径置灰
            foreach (var nextId in node.NextNodes)
            {
                if (_nodeDict.TryGetValue(node.Id, out var fromView) &&
                    _nodeDict.TryGetValue(nextId, out var toView))
                {
                    _view.DrawLine(fromView, toView, node.IsVisited && !node.IsLocked);
                }
            }
        }
    }

    private void RefreshLines()
    {
        // 重新绘制所有连线（或仅更新颜色）
        // 优化：可以只更新与变化节点相关的连线
    }
    public void OnNodeClicked(MapNodeData node)
    {
        if (!_mapService.CanSelectNode(node.Id) || node.IsLocked) return;
        _mapService.VisitNode(node.Id);
        // 根据类型分发
        switch (node.Type)
        {
            case MapNodeType.Battle:
            case MapNodeType.Elite:
                // EventBus<BattleStartEvent>.Fire(new BattleStartEvent { EnemyId = node.EnemyId });
                break;
            case MapNodeType.Rest:
                // EventBus.Fire(new RestStartEvent());
                break;
            case MapNodeType.Shop:
                // EventBus.Fire(new ShopOpenEvent());
                break;
            case MapNodeType.Event:
                // EventBus.Fire(new TreasureOpenEvent());
                break;
            case MapNodeType.Boss:
                // EventBus.Fire(new BossBattleStartEvent { EnemyId = node.EnemyId });
                break;
        }
    }
}