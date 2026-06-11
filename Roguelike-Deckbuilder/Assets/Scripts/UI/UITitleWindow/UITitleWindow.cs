using System.Collections;
using System.Collections.Generic;
using LitFramework;
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
    }
    protected override void OnShowInternal(object param)
    {

    }

    private void OnClickStart()
    {
        uiService.Close<UITitleWindow>();
        uiService.OpenUI<UIBattleWindow>();
    }
}
