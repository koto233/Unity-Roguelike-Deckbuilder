using System.Collections.Generic;
namespace Framework.EventSystem
{
    /// <summary>
    /// 事件总线，负责管理事件的注册、注销和触发
    /// </summary>
    /// <typeparam name="T"></typeparam> 
    public static class EventBus<T> where T : IEvent
    {
        private static readonly List<IEventBinding<T>> _bindings = new();

        public static void Subscribe(IEventBinding<T> handler) => _bindings.Add(handler);
        public static void Unsubscribe(IEventBinding<T> handler) => _bindings.Remove(handler);
        public static void Publish(T eventData)
        {
            for (int i = _bindings.Count - 1; i >= 0; i--)
                _bindings[i].OnEvent?.Invoke(eventData);
        }
    }
}