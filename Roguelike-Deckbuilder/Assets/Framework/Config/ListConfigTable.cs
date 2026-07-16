using System.Collections.Generic;

namespace LitFramework.Config
{
    public class ListConfigTable<T> : IConfigTable<T> where T : IConfig
    {
        private readonly List<T> _list;

        public ListConfigTable(List<T> list) => _list = list;

        public T Get(int id) => default(T);

        public IEnumerable<T> GetAll() => _list;

        public List<T> GetList() => _list;

        public T this[int index] => _list[index];
    }
}
