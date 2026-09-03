//------------------------------------------------------------------------------
// <自动生成>
//     本文件由 UIAutoBindGenerator 自动生成
//     请勿手动修改此文件，重新生成将覆盖所有改动
//
//     来源 UI : TopBar
//     生成时间 : 2026-09-03
// </自动生成>
//------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using TMPro;

    public partial class TopBar
    {
        private @Button b_MapBtn;
        private @Button b_DeckBtn;
        private @TextMeshProUGUI b_Num;
        private @Button b_SettingBtn;
        private @TextMeshProUGUI b_HpText;
        private @TextMeshProUGUI b_CoinText;
        private @RectTransform b_RelicRoot;

        protected override void GetUI()
        {
            base.GetUI();
            b_MapBtn = GetBind<@Button>(0);
            b_DeckBtn = GetBind<@Button>(1);
            b_Num = GetBind<@TextMeshProUGUI>(2);
            b_SettingBtn = GetBind<@Button>(3);
            b_HpText = GetBind<@TextMeshProUGUI>(4);
            b_CoinText = GetBind<@TextMeshProUGUI>(5);
            b_RelicRoot = GetBind<@RectTransform>(6);
        }
    }
