//------------------------------------------------------------------------------
// <自动生成>
//     本文件由 UIAutoBindGenerator 自动生成
//     请勿手动修改此文件，重新生成将覆盖所有改动
//
//     来源 UI : RestView
//     生成时间 : 2026-08-27
// </自动生成>
//------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

    public partial class RestView
    {
        private @Button b_RestButton;
        private @Button b_ForgeButton;
        private @Button b_ContinueButton;
        private @RectTransform b_ForgePanel;
        private @RectTransform b_ForgeListRoot;
        private @RectTransform b_ConfirmPanel;
        private @RectTransform b_BeforeUpgrade;
        private @RectTransform b_AfterUpgrade;
        private @Button b_ConfirmButton;

        protected override void GetUI()
        {
            base.GetUI();
            b_RestButton = GetBind<@Button>(0);
            b_ForgeButton = GetBind<@Button>(1);
            b_ContinueButton = GetBind<@Button>(2);
            b_ForgePanel = GetBind<@RectTransform>(3);
            b_ForgeListRoot = GetBind<@RectTransform>(4);
            b_ConfirmPanel = GetBind<@RectTransform>(5);
            b_BeforeUpgrade = GetBind<@RectTransform>(6);
            b_AfterUpgrade = GetBind<@RectTransform>(7);
            b_ConfirmButton = GetBind<@Button>(8);
        }
    }
