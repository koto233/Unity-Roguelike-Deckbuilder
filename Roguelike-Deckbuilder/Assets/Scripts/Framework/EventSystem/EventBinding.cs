using System;
namespace Framework.EventSystem
{
    public class EventBinding<T> : IEventBinding<T> where T : IEvent
    {
        private Action<T> onEvent = delegate { };

        Action<T> IEventBinding<T>.OnEvent
        {
            get => onEvent;
            set => onEvent = value;
        }

        public EventBinding(Action<T> onEvent) => this.onEvent = onEvent;

        public void Add(Action<T> handler) => onEvent += handler;
        public void Remove(Action<T> handler) => onEvent -= handler;
    }



}