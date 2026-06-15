using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LitFramework.Config
{
    public class ListConfigTable<T> : IConfigTable where T : IConfig
    {
        private List<T> _list;
        public ListConfigTable(List<T> list) => _list = list;
        public object GetById(string id) => null; // 列表表不支持按 Id 查询
        public IEnumerable<object> GetAll() => _list.Cast<object>();
        public bool IsDictionary => false;
        // 提供泛型方法
        public List<T> GetList() => _list;
        public T this[int index] => _list[index];
    }
}