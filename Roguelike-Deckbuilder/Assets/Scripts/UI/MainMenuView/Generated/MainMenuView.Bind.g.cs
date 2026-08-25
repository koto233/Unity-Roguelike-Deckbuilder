//------------------------------------------------------------------------------
// <自动生成>
//     本文件由 UIAutoBindGenerator 自动生成
//     请勿手动修改此文件，重新生成将覆盖所有改动
//
//     来源 UI : UITitleWindow
//     生成时间 : 2026-08-16
// </自动生成>
//------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

    public partial class MainMenuView
    {
        private @Button b_ContinueButton;
        private @Button b_NewGameButton;
        private @Button b_SettingButton;
        private @Button b_QuitButton;

        protected override void GetUI()
        {
            base.GetUI();
            b_ContinueButton = GetBind<@Button>(0);
            b_NewGameButton = GetBind<@Button>(1);
            b_SettingButton = GetBind<@Button>(2);
            b_QuitButton = GetBind<@Button>(3);
        }
    }
