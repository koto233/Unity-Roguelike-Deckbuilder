using System;
using Cysharp.Threading.Tasks;
namespace LitFramework.Asset
{
    /// <summary>
    /// 资源更新服务（负责初始化和热更新）
    /// </summary>
    public interface IResourceUpdater
    {
        /// <summary>
        /// 开始更新流程，完成后回调
        /// </summary>
        UniTask StartUpdate();
    }
}