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
            case "StatusCard":
                return new StatusCardIntent(config, value);
            case "AttackX2":
                return new AttackX2Intent(config, value);
            case "AttackX3":
                return new AttackX3Intent(config, value);
            default:
                Debug.LogError($"未知的意图类型: {config.Type}");
                return null;
        }
    }
}
