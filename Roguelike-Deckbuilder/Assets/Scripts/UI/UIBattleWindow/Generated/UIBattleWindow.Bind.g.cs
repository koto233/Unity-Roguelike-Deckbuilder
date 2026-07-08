//------------------------------------------------------------------------------
// <自动生成>
//     本文件由 UIAutoBindGenerator 自动生成
//     请勿手动修改此文件，重新生成将覆盖所有改动
//
//     来源 UI : UIBattleWindow
//     生成时间 : 2026-07-08
// </自动生成>
//------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using TMPro;

    public partial class UIBattleWindow
    {
        private @Slider b_HPSlider;
        private @TextMeshProUGUI b_HPText;
        private @TextMeshProUGUI b_BlockText;
        private @UIHandZone b_HandZone;
        private @RectTransform b_EnemysRoot;
        private @RectTransform b_PlayerRoot;
        private @TextMeshProUGUI b_EnergyText;
        private @Button b_DrawPileBtn;
        private @Button b_DiscardPileBtn;
        private @RectTransform b_PilePanel;
        private @Button b_ClosePileButton;
        private @Button b_EndTurnBtn;
        private Tooltip b_BuffTooltip;
        private @TextMeshProUGUI b_BuffDescText;
        private Tooltip b_IntentTooltip;
        private @TextMeshProUGUI b_IntentDescText;

        protected override void GetUI()
        {
            base.GetUI();
            b_HPSlider = GetBind<@Slider>(0);
            b_HPText = GetBind<@TextMeshProUGUI>(1);
            b_BlockText = GetBind<@TextMeshProUGUI>(2);
            b_HandZone = GetBind<@UIHandZone>(3);
            b_EnemysRoot = GetBind<@RectTransform>(4);
            b_PlayerRoot = GetBind<@RectTransform>(5);
            b_EnergyText = GetBind<@TextMeshProUGUI>(6);
            b_DrawPileBtn = GetBind<@Button>(7);
            b_DiscardPileBtn = GetBind<@Button>(8);
            b_PilePanel = GetBind<@RectTransform>(9);
            b_ClosePileButton = GetBind<@Button>(10);
            b_EndTurnBtn = GetBind<@Button>(11);
            b_BuffTooltip = GetBind<Tooltip>(12);
            b_BuffDescText = GetBind<@TextMeshProUGUI>(13);
            b_IntentTooltip = GetBind<Tooltip>(14);
            b_IntentDescText = GetBind<@TextMeshProUGUI>(15);
        }
    }
