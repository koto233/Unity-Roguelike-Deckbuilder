using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LitFramework.Config
{
    public class DictConfigTable<T> : IConfigTable where T : IConfig
    {
        private Dictionary<string, T> _dict;
        public DictConfigTable(Dictionary<string, T> dict) => _dict = dict;
        public object GetById(string id) => _dict.TryGetValue(id, out var val) ? val : null;
        public IEnumerable<object> GetAll() => _dict.Values.Cast<object>();
        public bool IsDictionary => true;
        // 提供泛型方法方便调用方
        public T Get(string id) => _dict.GetValueOrDefault(id); 
        public Dictionary<string, T> GetDict() => _dict;
    }
}