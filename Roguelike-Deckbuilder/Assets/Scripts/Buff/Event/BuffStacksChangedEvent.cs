using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LitFramework.EventBus;

public class BuffStacksChangedEvent: IEvent
{
    public CharacterBase Owner;
    public IBuff Buff;
    public int OldStacks;
    public int NewStacks;
}