using System;

namespace LitFramework.EventBus
{
    public interface IEventBinding<T> where T : IEvent
    {
        Action<T> OnEvent { get; set; }
    }
}

