using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitFramework;
using LitFramework.EventBus;
using LitFramework.UI.Core.Service;
using UnityEngine;

public class MapPresenter : BasePresenter<MapView>
{
    private MapService _mapService;
    private UIService _uiService;
    public MapPresenter(MapView view) : base(view)
    {

    }
    public override void Init()
    {
        SubscribeEvents();
        _mapService = ServiceLocator.Get<MapService>();
        _uiService = ServiceLocator.Get<UIService>();
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
            case MapNodeType.Boss:
            case MapNodeType.Elite:
                EventBus<BattleStartEvent>.Publish(new BattleStartEvent { Type = data.Type });
                break;
            case MapNodeType.Rest:
                _uiService.OpenAsync<RestView>().Forget();
                break;
            case MapNodeType.Shop:
                _uiService.OpenAsync<ShopView>().Forget();
                break;
            case MapNodeType.Event:
                _uiService.OpenAsync<EventView>().Forget();
                break;
        }

    }


    public override void Dispose()
    {
        UnSubscribeEvents();
    }


}