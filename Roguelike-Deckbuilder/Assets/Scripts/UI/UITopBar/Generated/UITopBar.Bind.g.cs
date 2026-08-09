//------------------------------------------------------------------------------
// <自动生成>
//     本文件由 UIAutoBindGenerator 自动生成
//     请勿手动修改此文件，重新生成将覆盖所有改动
//
//     来源 UI : UITopBar
//     生成时间 : 2026-08-09
// </自动生成>
//------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using TMPro;

    public partial class UITopBar
    {
        private @Button b_MapBtn;
        private @Button b_PileBtn;
        private @TextMeshProUGUI b_Num;
        private @Button b_SettingBtn;

        protected override void GetUI()
        {
            base.GetUI();
            b_MapBtn = GetBind<@Button>(0);
            b_PileBtn = GetBind<@Button>(1);
            b_Num = GetBind<@TextMeshProUGUI>(2);
            b_SettingBtn = GetBind<@Button>(3);
        }
    }
