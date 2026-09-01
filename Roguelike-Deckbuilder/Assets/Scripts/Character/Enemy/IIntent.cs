using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IIntent
{
    void Execute(EffectExecutor executor, Enemy enemy);
}
