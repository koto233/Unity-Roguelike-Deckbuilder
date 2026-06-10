using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataTable<T> where T : IConfig
{
    private Dictionary<int, T> _dict;
    public void Load(List<T> list)
    {
        _dict = new Dictionary<int, T>();
        foreach (var item in list)
            _dict[item.Id] = item;
    }
    public T Get(int id)
    {
        _dict.TryGetValue(id, out var item);
        return item;
    }
    public IEnumerable<T> GetAll() => _dict.Values;
}