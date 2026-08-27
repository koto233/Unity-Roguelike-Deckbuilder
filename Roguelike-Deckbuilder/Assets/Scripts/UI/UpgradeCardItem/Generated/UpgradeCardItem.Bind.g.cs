//------------------------------------------------------------------------------
// <自动生成>
//     本文件由 UIAutoBindGenerator 自动生成
//     请勿手动修改此文件，重新生成将覆盖所有改动
//
//     来源 UI : UpgradeCardItem
//     生成时间 : 2026-08-27
// </自动生成>
//------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using TMPro;

    public partial class UpgradeCardItem
    {
        private @Button b_Click;
        private @Image b_Icon;
        private @Image b_Frame;
        private @Image b_PortraitBorder;
        private @Image b_Banner;
        private @TextMeshProUGUI b_type_text;
        private @TextMeshProUGUI b_CostText;
        private @TextMeshProUGUI b_NameText;
        private @TextMeshProUGUI b_DescText;
        private @RectTransform b_ArrowStartRoot;

        protected override void GetUI()
        {
            base.GetUI();
            b_Click = GetBind<@Button>(0);
            b_Icon = GetBind<@Image>(1);
            b_Frame = GetBind<@Image>(2);
            b_PortraitBorder = GetBind<@Image>(3);
            b_Banner = GetBind<@Image>(4);
            b_type_text = GetBind<@TextMeshProUGUI>(5);
            b_CostText = GetBind<@TextMeshProUGUI>(6);
            b_NameText = GetBind<@TextMeshProUGUI>(7);
            b_DescText = GetBind<@TextMeshProUGUI>(8);
            b_ArrowStartRoot = GetBind<@RectTransform>(9);
        }
    }
