using System;



namespace Framework.ObjectPool
{
    public interface IPoolable
    {
        /// <summary>
        /// 当从池中获取时调用
        /// </summary>
        void OnSpawn();
        /// <summary>
        /// 回到池中时调用
        /// </summary> 

        void OnRecycle();
    }
}