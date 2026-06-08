using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Framework.EventSystem;

namespace Framework.AssetManager
{
    public struct AssetLoadedEvent : IEvent
    {
        public string Path;
        public UnityEngine.Object Asset;
    }
}