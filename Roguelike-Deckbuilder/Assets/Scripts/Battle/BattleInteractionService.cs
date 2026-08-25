using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleInteractionService
{
    public bool IsDragging { get; private set; } = false;
    public HandCard DraggingCard { get; private set; } = null;

    public void StartDrag(HandCard card)
    {
        IsDragging = true;
        DraggingCard = card;
    }

    public void EndDrag()
    {
        IsDragging = false;
        DraggingCard = null;
    }

    public bool CanInteract()
    {
        return !IsDragging;
    }
}
