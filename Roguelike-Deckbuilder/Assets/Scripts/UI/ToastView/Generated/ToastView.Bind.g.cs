//------------------------------------------------------------------------------
// <自动生成>
//     本文件由 UIAutoBindGenerator 自动生成
//     请勿手动修改此文件，重新生成将覆盖所有改动
//
//     来源 UI : ToastView
//     生成时间 : 2026-09-04
// </自动生成>
//------------------------------------------------------------------------------

using UnityEngine;
using TMPro;

    public partial class ToastView
    {
        private @CanvasGroup b_CanvasGroup;
        private @TextMeshProUGUI b_Message;

        protected override void GetUI()
        {
            base.GetUI();
            b_CanvasGroup = GetBind<@CanvasGroup>(0);
            b_Message = GetBind<@TextMeshProUGUI>(1);
        }
    }
