//------------------------------------------------------------------------------
// <自动生成>
//     本文件由 UIAutoBindGenerator 自动生成
//     请勿手动修改此文件，重新生成将覆盖所有改动
//
//     来源 UI : UIPlayerItem
//     生成时间 : 2026-06-20
// </自动生成>
//------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using TMPro;

    public partial class UIPlayerItem
    {
        private @Image b_Body;
        private @Slider b_HPSlider;
        private @TextMeshProUGUI b_HPText;

        protected override void GetUI()
        {
            base.GetUI();
            b_Body = GetBind<@Image>(0);
            b_HPSlider = GetBind<@Slider>(1);
            b_HPText = GetBind<@TextMeshProUGUI>(2);
        }
    }
