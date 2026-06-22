//------------------------------------------------------------------------------
// <自动生成>
//     本文件由 UIAutoBindGenerator 自动生成
//     请勿手动修改此文件，重新生成将覆盖所有改动
//
//     来源 UI : UICardItem
//     生成时间 : 2026-06-22
// </自动生成>
//------------------------------------------------------------------------------

using UnityEngine;
using TMPro;

    public partial class UICardItem
    {
        private @RectTransform b_UICardItemRect;
        private @TextMeshProUGUI b_CostText;
        private @TextMeshProUGUI b_NameText;
        private @TextMeshProUGUI b_DescText;

        protected override void GetUI()
        {
            base.GetUI();
            b_UICardItemRect = GetBind<@RectTransform>(0);
            b_CostText = GetBind<@TextMeshProUGUI>(1);
            b_NameText = GetBind<@TextMeshProUGUI>(2);
            b_DescText = GetBind<@TextMeshProUGUI>(3);
        }
    }
