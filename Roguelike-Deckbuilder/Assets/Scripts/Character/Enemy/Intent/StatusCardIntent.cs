using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusCardIntent : IIntent
{
    private int _cardId;
    public StatusCardIntent(IntentConfig config, int value)
    {
        _cardId = value;
    }
    public void Execute(EffectExecutor executor, Enemy enemy)
    {
        executor.AddStatusCardToDiscardPile(_cardId);
    }
}
