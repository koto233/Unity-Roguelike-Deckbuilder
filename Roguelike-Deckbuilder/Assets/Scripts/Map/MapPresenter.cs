using System.Collections.Generic;
using System.Linq;
using LitFramework;
using LitFramework.EventBus;
using UnityEngine;

public class MapPresenter
{
    private MapService _mapService;
    private UIMap _view;

    private Dictionary<string, UIMapNode> _nodeDict = new Dictionary<string, UIMapNode>();

    public MapPresenter(UIMap view)
    {
        _mapService = ServiceLocator.Get<MapService>();
        _view = view;
    }


    public void GenerateMap(int templateId)
    {
        _mapService.GenerateMap(templateId);
        GenerateMapUI(_mapService.CurrentMap);
    }

    private void GenerateMapUI(List<MapNodeData> nodes)
    {
        _view.Clear();
        _nodeDict.Clear();

        var rows = nodes.GroupBy(n => n.Row).OrderBy(g => g.Key);
        foreach (var rowGroup in rows)
        {
            var rowGO = _view.CreateRow();
            foreach (var nodeData in rowGroup.OrderBy(n => n.Column))
            {
                var nodeView = _view.CreateNode(nodeData, rowGO.transform);
                _nodeDict[nodeData.Id] = nodeView;
                nodeView.SetNodeData(nodeData);
                nodeView.Button.onClick.AddListener(() => OnNodeClicked(nodeData.Id));
                UpdateNodeVisual(nodeView, nodeData);
            }
        }

        DrawAllLines(nodes);
    }

    private void RefreshNode(string nodeId)
    {
        if (_nodeDict.TryGetValue(nodeId, out var nodeView))
        {
            var data = _mapService.GetNode(nodeId);
            if (data != null)
            {
                nodeView.SetNodeData(data);
                UpdateNodeVisual(nodeView, data);
            }
        }
        RefreshLines();
    }

    private void RefreshAll()
    {
        foreach (var kv in _nodeDict)
        {
            var data = _mapService.GetNode(kv.Key);
            if (data != null)
            {
                kv.Value.SetNodeData(data);
                UpdateNodeVisual(kv.Value, data);
            }
        }
        RefreshLines();
    }

    private void UpdateNodeVisual(UIMapNode view, MapNodeData data)
    {
        view.SetType(data.Type);
        view.SetLocked(data.IsLocked);
        view.Button.interactable = !data.IsLocked && !data.IsVisited;
        view.SetVisited(data.IsVisited);
        view.SetStart(data.IsStart);
    }

    private void OnNodeClicked(string nodeId)
    {
        var data = _mapService.GetNode(nodeId);
        if (data == null || data.IsLocked || data.IsVisited) return;
        if (!_mapService.CanSelectNode(nodeId)) return;

        _mapService.VisitNode(nodeId);
        RefreshNode(nodeId);

        switch (data.Type)
        {
            case MapNodeType.Battle:
            case MapNodeType.Elite:
                EventBus<BattleStartEvent>.Publish(new BattleStartEvent { EnemyId = data.EnemyId, IsElite = data.Type == MapNodeType.Elite });
                break;
            case MapNodeType.Rest:
                EventBus<RestStartEvent>.Publish(new RestStartEvent());
                break;
            case MapNodeType.Shop:
                EventBus<ShopOpenEvent>.Publish(new ShopOpenEvent());
                break;
            case MapNodeType.Event:
                EventBus<EventStartEvent>.Publish(new EventStartEvent());
                break;
            case MapNodeType.Boss:
                EventBus<BossBattleStartEvent>.Publish(new BossBattleStartEvent { EnemyId = data.EnemyId });
                break;
        }
    }

    private void DrawAllLines(List<MapNodeData> nodes)
    {
        _view.ClearLines();
        foreach (var node in nodes)
        {
            if (node.NextNodes == null || node.NextNodes.Count == 0) continue;
            foreach (var nextId in node.NextNodes)
            {
                if (_nodeDict.TryGetValue(node.Id, out var fromView) &&
                    _nodeDict.TryGetValue(nextId, out var toView))
                {
                    _view.DrawLine(fromView, toView, !node.IsLocked);
                }
            }
        }
    }

    private void RefreshLines()
    {
        if (_mapService.CurrentMap != null)
            DrawAllLines(_mapService.CurrentMap);
    }
}