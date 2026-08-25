//------------------------------------------------------------------------------
// <自动生成>
//     本文件由 UIAutoBindGenerator 自动生成
//     请勿手动修改此文件，重新生成将覆盖所有改动
//
//     来源 UI : UIShop
//     生成时间 : 2026-08-21
// </自动生成>
//------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using TMPro;

    public partial class ShopView
    {
        private @RectTransform b_CardsRoot;
        private @RectTransform b_PotionsRoot;
        private @Button b_RemoveButton;
        private @TextMeshProUGUI b_RemovePriceText;
        private @Button b_ContinueButton;

        protected override void GetUI()
        {
            base.GetUI();
            b_CardsRoot = GetBind<@RectTransform>(0);
            b_PotionsRoot = GetBind<@RectTransform>(1);
            b_RemoveButton = GetBind<@Button>(2);
            b_RemovePriceText = GetBind<@TextMeshProUGUI>(3);
            b_ContinueButton = GetBind<@Button>(4);
        }
    }
