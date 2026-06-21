//------------------------------------------------------------------------------
// <自动生成>
//     本文件由 UIAutoBindGenerator 自动生成
//     请勿手动修改此文件，重新生成将覆盖所有改动
//
//     来源 UI : UIBattleWindow
//     生成时间 : 2026-06-21
// </自动生成>
//------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using TMPro;

    public partial class UIBattleWindow
    {
        private @Slider b_HPSlider;
        private @TextMeshProUGUI b_HPText;
        private @UIHandZone b_HandZone;
        private @RectTransform b_EnemysRoot;
        private @RectTransform b_PlayerRoot;
        private @TextMeshProUGUI b_EnergyText;

        protected override void GetUI()
        {
            base.GetUI();
            b_HPSlider = GetBind<@Slider>(0);
            b_HPText = GetBind<@TextMeshProUGUI>(1);
            b_HandZone = GetBind<@UIHandZone>(2);
            b_EnemysRoot = GetBind<@RectTransform>(3);
            b_PlayerRoot = GetBind<@RectTransform>(4);
            b_EnergyText = GetBind<@TextMeshProUGUI>(5);
        }
    }
