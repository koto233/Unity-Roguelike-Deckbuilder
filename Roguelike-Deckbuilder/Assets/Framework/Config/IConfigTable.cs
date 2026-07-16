using System.Collections.Generic;

namespace LitFramework.Config
{
    public interface IConfigTable<out T> where T : IConfig
    {
        T Get(int id);
        IEnumerable<T> GetAll();
    }
}
