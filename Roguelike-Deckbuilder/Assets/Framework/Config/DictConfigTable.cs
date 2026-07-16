using System.Collections.Generic;

namespace LitFramework.Config
{
    public class DictConfigTable<T> : IConfigTable<T> where T : IConfig
    {
        private readonly Dictionary<int, T> _dict;

        public DictConfigTable(Dictionary<int, T> dict) => _dict = dict;

        public T Get(int id) => _dict.TryGetValue(id, out var val) ? val : default(T);

        public IEnumerable<T> GetAll() => _dict.Values;

        public Dictionary<int, T> GetDict() => _dict;
    }
}
