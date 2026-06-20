//------------------------------------------------------------------------------
// <自动生成>
//     本文件由 UIAutoBindGenerator 自动生成
//     请勿手动修改此文件，重新生成将覆盖所有改动
//
//     来源 UI : UIEnemyItem
//     生成时间 : 2026-06-20
// </自动生成>
//------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using TMPro;

    public partial class UIEnemyItem
    {
        private @Image b_Body;
        private @Slider b_HPSlider;
        private @TextMeshProUGUI b_HPText;
        private @Image b_IntentionIcon;

        protected override void GetUI()
        {
            base.GetUI();
            b_Body = GetBind<@Image>(0);
            b_HPSlider = GetBind<@Slider>(1);
            b_HPText = GetBind<@TextMeshProUGUI>(2);
            b_IntentionIcon = GetBind<@Image>(3);
        }
    }
