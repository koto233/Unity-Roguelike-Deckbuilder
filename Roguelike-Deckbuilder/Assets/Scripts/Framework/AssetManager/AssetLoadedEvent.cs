using LitFramework.EventBus;

namespace LitFramework.AssetManager
{
    public struct AssetLoadedEvent : IEvent
    {
        public string Path;
        public UnityEngine.Object Asset;
    }
}