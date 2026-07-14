using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleInteractionService
{
    public bool IsDragging { get; private set; } = false;
    public UICardItem DraggingCard { get; private set; } = null;

    public void StartDrag(UICardItem card)
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
