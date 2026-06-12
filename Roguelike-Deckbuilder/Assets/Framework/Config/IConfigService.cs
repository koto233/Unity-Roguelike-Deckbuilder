using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LitFramework.Config
{
    public interface IConfigService
    {
        public void LoadTable<T>(string jsonPath, Action<bool> onCompleted = null) where T : IConfig { }
        public DataTable<T> GetTable<T>() where T : IConfig { return null; }
    }
}