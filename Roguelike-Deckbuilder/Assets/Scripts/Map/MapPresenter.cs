using System.Collections;
using System.Collections.Generic;
using LitFramework;
using LitFramework.EventBus;
using UnityEngine;

// 示例伪代码
public class MapPresenter : MonoBehaviour
{
    private MapService _mapService;

    void Start()
    {
        _mapService = ServiceLocator.Get<MapService>();

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