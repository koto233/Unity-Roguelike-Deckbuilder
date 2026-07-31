using System;
using LitFramework;
using LitFramework.EventBus;
using UnityEngine;

public class MapPresenter : IDisposable
{
    private MapService _mapService;
    private UIMap _view;

    public MapPresenter(UIMap view)
    {
        SubscribeEvents();
        _mapService = ServiceLocator.Get<MapService>();
        _view = view;
        _view.OnNodeClicked += OnNodeClicked;
    }


    private void SubscribeEvents()
    {

    }

    private void UnSubscribeEvents()
    {

    }

    public void GenerateMap(int templateId)
    {
        _mapService.GenerateMap(templateId);
        _view.CreateMap(_mapService.CurrentMap);
    }

    private void OnNodeClicked(string nodeId)
    {
        var data = _mapService.GetNode(nodeId);
        if (data == null || data.IsLocked || data.IsVisited) return;
        if (!_mapService.CanSelectNode(nodeId)) return;

        // 访问节点（会解锁相邻节点）
        _mapService.VisitNode(nodeId);

        // 刷新 UI
        _view.RefreshMap(_mapService.CurrentMap, nodeId);

        // 根据节点类型触发业务事件
        switch (data.Type)
        {
            case MapNodeType.Battle:
                EventBus<BattleStartEvent>.Publish(new BattleStartEvent { EnemyId = data.EnemyId, IsElite = data.Type == MapNodeType.Elite });
                break;
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
                EventBus<BattleStartEvent>.Publish(new BattleStartEvent { EnemyId = data.EnemyId, IsElite = data.Type == MapNodeType.Elite });
                break;
        }

    }
    public void Dispose()
    {
        UnSubscribeEvents();
    }
}