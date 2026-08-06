//------------------------------------------------------------------------------
// <自动生成>
//     本文件由 UIAutoBindGenerator 自动生成
//     请勿手动修改此文件，重新生成将覆盖所有改动
//
//     来源 UI : UICardItem
//     生成时间 : 2026-08-06
// </自动生成>
//------------------------------------------------------------------------------

using UnityEngine;
using TMPro;

    public partial class UICardItem
    {
        private @TextMeshProUGUI b_CostText;
        private @TextMeshProUGUI b_NameText;
        private @TextMeshProUGUI b_DescText;
        private @RectTransform b_ArrowStartRoot;

        protected override void GetUI()
        {
            base.GetUI();
            b_CostText = GetBind<@TextMeshProUGUI>(0);
            b_NameText = GetBind<@TextMeshProUGUI>(1);
            b_DescText = GetBind<@TextMeshProUGUI>(2);
            b_ArrowStartRoot = GetBind<@RectTransform>(3);
        }
    }
