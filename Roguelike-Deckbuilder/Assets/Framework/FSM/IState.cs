namespace LitFramework.FSM
{
    public interface IState
    {
        void OnInit();
        /// <summary>
        /// 进入状态
        /// </summary>
        void OnEnter();

        /// <summary>
        /// 每帧更新（可选，不需要可空实现）
        /// </summary>
        void OnUpdate();

        /// <summary>
        /// 退出状态
        /// </summary>
        void OnExit();
        void OnDestroy();
    }
}