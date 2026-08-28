//------------------------------------------------------------------------------
// <自动生成>
//     本文件由 UIAutoBindGenerator 自动生成
//     请勿手动修改此文件，重新生成将覆盖所有改动
//
//     来源 UI : ShopView
//     生成时间 : 2026-08-28
// </自动生成>
//------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using TMPro;

    public partial class ShopView
    {
        private @RectTransform b_CardsRoot;
        private @RectTransform b_RelicsRoot;
        private @Button b_RemoveButton;
        private @TextMeshProUGUI b_RemovePriceText;
        private @Button b_ContinueButton;
        private @RectTransform b_RemovePanel;
        private @RectTransform b_ForgeListRoot;
        private @RectTransform b_ConfirmPanel;
        private @RectTransform b_CardRoot;
        private @Button b_ConfirmButton;

        protected override void GetUI()
        {
            base.GetUI();
            b_CardsRoot = GetBind<@RectTransform>(0);
            b_RelicsRoot = GetBind<@RectTransform>(1);
            b_RemoveButton = GetBind<@Button>(2);
            b_RemovePriceText = GetBind<@TextMeshProUGUI>(3);
            b_ContinueButton = GetBind<@Button>(4);
            b_RemovePanel = GetBind<@RectTransform>(5);
            b_ForgeListRoot = GetBind<@RectTransform>(6);
            b_ConfirmPanel = GetBind<@RectTransform>(7);
            b_CardRoot = GetBind<@RectTransform>(8);
            b_ConfirmButton = GetBind<@Button>(9);
        }
    }
