using System;
using System.Collections.Generic;
using System.Linq;

public static class WeightedRandomPicker
{
    public static List<T> Pick<T>(List<T> items, int count, Func<T, int> weightSelector)
    {
        if (items.Count == 0) return new List<T>();
        if (items.Count <= count) return items.ToList();

        var result = new List<T>();
        var pool = items.ToList();

        for (int i = 0; i < count; i++)
        {
            int totalWeight = pool.Sum(item => weightSelector(item));
            if (totalWeight <= 0) break;

            int randomValue = UnityEngine.Random.Range(0, totalWeight);
            int accumulated = 0;
            T selected = default;

            for (int j = 0; j < pool.Count; j++)
            {
                accumulated += weightSelector(pool[j]);
                if (randomValue < accumulated)
                {
                    selected = pool[j];
                    break;
                }
            }

            if (selected != null)
            {
                result.Add(selected);
                pool.Remove(selected);
            }
        }

        return result;
    }
}