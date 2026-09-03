//------------------------------------------------------------------------------
// <自动生成>
//     本文件由 UIAutoBindGenerator 自动生成
//     请勿手动修改此文件，重新生成将覆盖所有改动
//
//     来源 UI : GameOverView
//     生成时间 : 2026-09-03
// </自动生成>
//------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

    public partial class GameOverView
    {
        private @Button b_Restart;
        private @Button b_Quit;

        protected override void GetUI()
        {
            base.GetUI();
            b_Restart = GetBind<@Button>(0);
            b_Quit = GetBind<@Button>(1);
        }
    }
