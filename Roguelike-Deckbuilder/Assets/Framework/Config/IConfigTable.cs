using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConfigTable
{
    object GetById(string id);   // 列表表可返回 null 或抛出异常
    IEnumerable<object> GetAll();
    bool IsDictionary { get; }
}