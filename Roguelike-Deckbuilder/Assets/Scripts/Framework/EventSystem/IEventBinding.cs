using System;

namespace Framework.EventSystem
{
    public interface IEventBinding<T> where T : IEvent
    {
        Action<T> OnEvent { get; set; }
    }
}

