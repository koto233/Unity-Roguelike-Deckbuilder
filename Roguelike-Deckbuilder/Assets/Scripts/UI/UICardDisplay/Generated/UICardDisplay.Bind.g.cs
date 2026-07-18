//------------------------------------------------------------------------------
// <自动生成>
//     本文件由 UIAutoBindGenerator 自动生成
//     请勿手动修改此文件，重新生成将覆盖所有改动
//
//     来源 UI : UICardDisplay
//     生成时间 : 2026-07-18
// </自动生成>
//------------------------------------------------------------------------------

using UnityEngine;
using TMPro;

    public partial class UICardDisplay
    {
        private @TextMeshProUGUI b_CostText;
        private @TextMeshProUGUI b_NameText;
        private @TextMeshProUGUI b_DescText;

        protected override void GetUI()
        {
            base.GetUI();
            b_CostText = GetBind<@TextMeshProUGUI>(0);
            b_NameText = GetBind<@TextMeshProUGUI>(1);
            b_DescText = GetBind<@TextMeshProUGUI>(2);
        }
    }
