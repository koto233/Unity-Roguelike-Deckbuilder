using Framework.EventSystem;

namespace Framework.AssetManager
{
    public struct AssetLoadedEvent : IEvent
    {
        public string Path;
        public UnityEngine.Object Asset;
    }
}