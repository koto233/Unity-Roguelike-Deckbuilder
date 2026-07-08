using LitFramework.EventBus;
using UnityEngine;

public struct TooltipShowEvent : IEvent
{
    public TooltipType Type; // Intent, Card, Buff, Relic...
    public TooltipData Data;
    public Vector2 Position;
    public bool IsHovering;
}
public enum TooltipType
{
    Intent,
    Card,
    Buff,
    Relic
}