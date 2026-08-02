using LitFramework;
using LitFramework.Asset;
using LitFramework.FSM.Procedure;
using LitFramework.UI.Core.Service;
using LitFramework.UI.Core.Window;

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
        ServiceLocator.Get<MapService>().GenerateMap(1);
        GameRoot.Instance.ProcedureManager.ChangeProcedure<ProcedureMap>();
        // StartBattleAsync().Forget();
    }



}
