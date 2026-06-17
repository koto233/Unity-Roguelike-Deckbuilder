using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace LitFramework.Config
{
    public interface IConfigService
    {
        UniTask LoadDictTableAsync<T>(string jsonPath) where T : IConfig;
        UniTask LoadListTableAsync<T>(string jsonPath) where T : IConfig;
        IConfigTable GetTable<T>() where T : IConfig;
    }
}