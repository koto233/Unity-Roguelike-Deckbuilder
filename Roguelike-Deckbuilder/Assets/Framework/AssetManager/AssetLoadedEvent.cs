using LitFramework.EventBus;

namespace LitFramework.Asset
{
    public struct AssetLoadedEvent : IEvent
    {
        public string Path;
        public UnityEngine.Object Asset;
    }
}