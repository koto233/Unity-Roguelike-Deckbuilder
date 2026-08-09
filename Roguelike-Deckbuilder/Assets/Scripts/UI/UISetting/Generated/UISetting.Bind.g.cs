//------------------------------------------------------------------------------
// <自动生成>
//     本文件由 UIAutoBindGenerator 自动生成
//     请勿手动修改此文件，重新生成将覆盖所有改动
//
//     来源 UI : UISetting
//     生成时间 : 2026-08-09
// </自动生成>
//------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

    public partial class UISetting
    {
        private @Button b_Continue;
        private @Button b_GiveUp;
        private @Button b_SaveAndQuit;

        protected override void GetUI()
        {
            base.GetUI();
            b_Continue = GetBind<@Button>(0);
            b_GiveUp = GetBind<@Button>(1);
            b_SaveAndQuit = GetBind<@Button>(2);
        }
    }
