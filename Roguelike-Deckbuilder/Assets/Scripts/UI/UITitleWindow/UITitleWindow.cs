using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using LitFramework;
using LitFramework.Asset;
using LitFramework.FSM.Procedure;
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
        // assetService.LoadAsync<GameObject>("Assets/Res/UI/UICardItem.prefab", null);
    }
    
    private void OnClickStart()
    {
        GameRoot.Instance.ProcedureManager.ChangeProcedure<ProcedureMap>();
        // StartBattleAsync().Forget();
    }



}
