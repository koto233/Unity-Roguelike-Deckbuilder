using System.Collections;
using System.Collections.Generic;
using LitFramework;
using LitFramework.Asset;
using LitFramework.UI.Core.Service;
using LitFramework.UI.Core.Window;
using UnityEngine;

public partial class UITitleWindow : UIWindow
{
    private UIService uiService;

    void Start()
    {
        uiService = ServiceLocator.Get<UIService>();
        b_StartButton.onClick.AddListener(OnClickStart);
        var assetService = ServiceLocator.Get<IAssetService>();
        assetService.LoadAsync<GameObject>("Assets/Res/UI/UICardItem.prefab", null);
    }
    protected override void OnShowInternal(object param)
    {

    }

    private void OnClickStart()
    {
        uiService.Close<UITitleWindow>();
        uiService.OpenUI<UIBattleWindow>();
        var battleContext = new BattleContext()
        {
            Player = new PlayerData(10, 3),
            Enemies = new List<EnemyData>()
            {
                new EnemyData(20, 3),
            },
            CurrentTurn = 0,
            IsPlayerTurn = true,
            Target = null,
            GoldReward = 0
        };
        var battleController = new BattleController(battleContext, () =>
        {
            uiService.Close<UIBattleWindow>();
            uiService.OpenUI<UITitleWindow>();
        });

        battleController.StartBattle();
    }
}
