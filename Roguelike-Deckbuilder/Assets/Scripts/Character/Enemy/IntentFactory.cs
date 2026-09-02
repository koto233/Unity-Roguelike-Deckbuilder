using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IntentFactory
{
    public static IIntent CreateIntent(IntentConfig config, int value)
    {
        switch (config.Type)
        {
            case "Attack":
                return new AttackIntent(config, value);
            case "Buff":
                return new BuffIntent(config, value);
            case "Debuff":
                return new DebuffIntent(config, value);
            default:
                Debug.LogError($"未知的意图类型: {config.Type}");
                return null;
        }
    }
}
