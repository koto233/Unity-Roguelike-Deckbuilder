using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LitFramework.Config
{
    public class DictConfigTable<T> : IConfigTable where T : IConfig
    {
        private Dictionary<int, T> _dict;
        public DictConfigTable(Dictionary<int, T> dict) => _dict = dict;
        public object GetById(int id) => _dict.TryGetValue(id, out var val) ? val : null;
        public IEnumerable<object> GetAll() => _dict.Values.Cast<object>();
        public bool IsDictionary => true;
        // 提供泛型方法方便调用方
        public T Get(int id) => _dict.GetValueOrDefault(id); 
        public Dictionary<int, T> GetDict() => _dict;
    }
}