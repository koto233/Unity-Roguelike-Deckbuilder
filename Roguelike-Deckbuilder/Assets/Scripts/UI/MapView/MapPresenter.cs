using System;
using LitFramework;
using LitFramework.EventBus;
using UnityEngine;

public class MapPresenter : BasePresenter<MapView>
{
    private MapService _mapService;

    public MapPresenter(MapView view) : base(view)
    {

    }
    public override void Init()
    {
        SubscribeEvents();
        _mapService = ServiceLocator.Get<MapService>();
        View.OnNodeClicked += OnNodeClicked;
        CreateMapUI();
    }

    private void SubscribeEvents()
    {

    }

    private void UnSubscribeEvents()
    {

    }

    public void CreateMapUI()
    {
        View.CreateMap(_mapService.CurrentMapList);
    }

    private void OnNodeClicked(string nodeId)
    {
        var data = _mapService.GetNode(nodeId);
        if (data == null || data.IsLocked || data.IsVisited) return;
        if (!_mapService.CanSelectNode(nodeId)) return;

        // 访问节点（会解锁相邻节点）
        _mapService.VisitNode(nodeId);

        // 刷新 UI
        View.RefreshMap(_mapService.CurrentMap);

        // 根据节点类型触发业务事件
        switch (data.Type)
        {
            case MapNodeType.Battle:
                EventBus<BattleStartEvent>.Publish(new BattleStartEvent { EnemyIds = data.EnemyIds });
                break;
            case MapNodeType.Elite:
                EventBus<BattleStartEvent>.Publish(new BattleStartEvent { EnemyIds = data.EnemyIds });
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
                EventBus<BattleStartEvent>.Publish(new BattleStartEvent { EnemyIds = data.EnemyIds });
                break;
        }

    }
    public override void Dispose()
    {
        UnSubscribeEvents();
    }


}