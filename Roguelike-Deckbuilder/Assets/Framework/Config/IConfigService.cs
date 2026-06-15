using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LitFramework.Config
{
    public interface IConfigService
    {
        public void LoadDictTable<T>(string jsonPath, Action<bool> onCompleted = null) where T : IConfig { }
        public void LoadListTable<T>(string jsonPath, Action<bool> onCompleted= null) where T : IConfig { }
        public IConfigTable GetTable<T>() where T : IConfig { return null; }
    }
}