using System.Collections.Generic;
using UnityEngine;
using System;
namespace LitFramework
{
    /// <summary>
    /// 服务定位器
    /// </summary>
    public static class ServiceLocator
    {
        private static Dictionary<Type, object> _services = new Dictionary<Type, object>();

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <param name="service">服务实例</param>
        public static void Register<T>(T service)
        {
            var type = typeof(T);
            if (!_services.ContainsKey(type))
            {
                _services.Add(type, service);
                Debug.Log($"注册 {type} 服务成功。");
            }
            else
            {
                Debug.LogWarning($" {type} 服务已注册。请勿重复注册。");
            }
        }
        /// <summary>
        /// 获取服务
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <returns>服务实例</returns>
        public static T Get<T>()
        {
            var type = typeof(T);
            if (_services.TryGetValue(type, out var obj))
            {
                return (T)obj;
            }
            else
            {
                Debug.LogError($"未找到 {type} 服务。请确保已注册该服务。");
                return default;
            }
        }

    }
}