namespace LitFramework.UI.Core.Service
{
    // ========== UIConfig 扩展（加两个字段） ==========
    public class UIConfig
    {
        public string PrefabPath;
        public UILayer Layer;
        public bool AllowMultiple;   // 是否允许同类型堆叠
        public bool PushToStack;     // 是否加入返回栈

        public UIConfig(string path, UILayer layer, bool allowMultiple = false, bool pushToStack = true)
        {
            PrefabPath = path;
            Layer = layer;
            AllowMultiple = allowMultiple;
            PushToStack = pushToStack;
        }
    }
}