//------------------------------------------------------------------------------
// <自动生成>
//     本文件由 UIAutoBindGenerator 自动生成
//     请勿手动修改此文件，重新生成将覆盖所有改动
//
//     来源 UI : BattleView
//     生成时间 : 2026-09-13
// </自动生成>
//------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using TMPro;

    public partial class BattleView
    {
        private @Slider b_HPSlider;
        private @TextMeshProUGUI b_BlockText;
        private @TextMeshProUGUI b_HPText;
        private @RectTransform b_EnemysRoot;
        private @RectTransform b_PlayerRoot;
        private @TargetArrow b_TargetArrow;
        private @HandZone b_HandZone;
        private @TextMeshProUGUI b_Desc;
        private @TextMeshProUGUI b_EnergyText;
        private @Button b_DrawPileBtn;
        private @TextMeshProUGUI b_DrawPileCount;
        private @Button b_DiscardPileBtn;
        private @TextMeshProUGUI b_DiscardPileCount;
        private @RectTransform b_PilePanel;
        private @Button b_ClosePileButton;
        private @Button b_EndTurnBtn;
        private @Tooltip b_BuffTooltip;
        private @Tooltip b_IntentTooltip;

        protected override void GetUI()
        {
            base.GetUI();
            b_HPSlider = GetBind<@Slider>(0);
            b_BlockText = GetBind<@TextMeshProUGUI>(1);
            b_HPText = GetBind<@TextMeshProUGUI>(2);
            b_EnemysRoot = GetBind<@RectTransform>(3);
            b_PlayerRoot = GetBind<@RectTransform>(4);
            b_TargetArrow = GetBind<@TargetArrow>(5);
            b_HandZone = GetBind<@HandZone>(6);
            b_Desc = GetBind<@TextMeshProUGUI>(7);
            b_EnergyText = GetBind<@TextMeshProUGUI>(8);
            b_DrawPileBtn = GetBind<@Button>(9);
            b_DrawPileCount = GetBind<@TextMeshProUGUI>(10);
            b_DiscardPileBtn = GetBind<@Button>(11);
            b_DiscardPileCount = GetBind<@TextMeshProUGUI>(12);
            b_PilePanel = GetBind<@RectTransform>(13);
            b_ClosePileButton = GetBind<@Button>(14);
            b_EndTurnBtn = GetBind<@Button>(15);
            b_BuffTooltip = GetBind<@Tooltip>(16);
            b_IntentTooltip = GetBind<@Tooltip>(17);
        }
    }
