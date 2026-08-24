//------------------------------------------------------------------------------
// <自动生成>
//     本文件由 UIAutoBindGenerator 自动生成
//     请勿手动修改此文件，重新生成将覆盖所有改动
//
//     来源 UI : UITopBar
//     生成时间 : 2026-08-24
// </自动生成>
//------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using TMPro;

    public partial class UITopBar
    {
        private @Button b_MapBtn;
        private @Button b_DeckBtn;
        private @TextMeshProUGUI b_Num;
        private @Button b_SettingBtn;
        private @TextMeshProUGUI b_HpText;
        private @TextMeshProUGUI b_CoinText;

        protected override void GetUI()
        {
            base.GetUI();
            b_MapBtn = GetBind<@Button>(0);
            b_DeckBtn = GetBind<@Button>(1);
            b_Num = GetBind<@TextMeshProUGUI>(2);
            b_SettingBtn = GetBind<@Button>(3);
            b_HpText = GetBind<@TextMeshProUGUI>(4);
            b_CoinText = GetBind<@TextMeshProUGUI>(5);
        }
    }
