using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataTable<T> where T : IConfig
{
    private Dictionary<string, T> _dict;
    public void Load(Dictionary<string, T> dict)
    {
        _dict = dict;
        // foreach (var item in list)
        //     _dict[item.Id] = item;
    }
    public T Get(string id)
    {
        _dict.TryGetValue(id, out var item);
        return item;
    }
    public Dictionary<string, T> DictClone => _dict;
    public IEnumerable<T> GetAll() => _dict.Values;
}