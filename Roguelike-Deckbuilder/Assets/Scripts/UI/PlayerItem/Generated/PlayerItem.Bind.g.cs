//------------------------------------------------------------------------------
// <自动生成>
//     本文件由 UIAutoBindGenerator 自动生成
//     请勿手动修改此文件，重新生成将覆盖所有改动
//
//     来源 UI : PlayerItem
//     生成时间 : 2026-09-02
// </自动生成>
//------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using TMPro;

    public partial class PlayerItem
    {
        private @Image b_Body;
        private @Slider b_HPSlider;
        private @TextMeshProUGUI b_HPText;
        private @TextMeshProUGUI b_BlockNum;
        private @RectTransform b_DamageTextPos;
        private @HorizontalLayoutGroup b_BuffRoot;

        protected override void GetUI()
        {
            base.GetUI();
            b_Body = GetBind<@Image>(0);
            b_HPSlider = GetBind<@Slider>(1);
            b_HPText = GetBind<@TextMeshProUGUI>(2);
            b_BlockNum = GetBind<@TextMeshProUGUI>(3);
            b_DamageTextPos = GetBind<@RectTransform>(4);
            b_BuffRoot = GetBind<@HorizontalLayoutGroup>(5);
        }
    }
