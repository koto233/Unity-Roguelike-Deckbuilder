//------------------------------------------------------------------------------
// <自动生成>
//     本文件由 UIAutoBindGenerator 自动生成
//     请勿手动修改此文件，重新生成将覆盖所有改动
//
//     来源 UI : UIMapNode
//     生成时间 : 2026-07-23
// </自动生成>
//------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using TMPro;

    public partial class UIMapNode
    {
        private @Image b_Icon;
        private @TextMeshProUGUI b_Name;
        private @Image b_Lock;
        private @Button b_Button;
        private @Image b_HighLight;

        protected override void GetUI()
        {
            base.GetUI();
            b_Icon = GetBind<@Image>(0);
            b_Name = GetBind<@TextMeshProUGUI>(1);
            b_Lock = GetBind<@Image>(2);
            b_Button = GetBind<@Button>(3);
            b_HighLight = GetBind<@Image>(4);
        }
    }
