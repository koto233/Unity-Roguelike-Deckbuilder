//------------------------------------------------------------------------------
// <自动生成>
//     本文件由 UIAutoBindGenerator 自动生成
//     请勿手动修改此文件，重新生成将覆盖所有改动
//
//     来源 UI : ConfirmDialogView
//     生成时间 : 2026-09-04
// </自动生成>
//------------------------------------------------------------------------------

using UnityEngine;
using TMPro;
using UnityEngine.UI;

    public partial class ConfirmDialogView
    {
        private @TextMeshProUGUI b_Title;
        private @TextMeshProUGUI b_Desc;
        private @Button b_ConfirmButton;
        private @TextMeshProUGUI b_ConfirmText;
        private @Button b_CancelButton;
        private @TextMeshProUGUI b_CancelText;

        protected override void GetUI()
        {
            base.GetUI();
            b_Title = GetBind<@TextMeshProUGUI>(0);
            b_Desc = GetBind<@TextMeshProUGUI>(1);
            b_ConfirmButton = GetBind<@Button>(2);
            b_ConfirmText = GetBind<@TextMeshProUGUI>(3);
            b_CancelButton = GetBind<@Button>(4);
            b_CancelText = GetBind<@TextMeshProUGUI>(5);
        }
    }
