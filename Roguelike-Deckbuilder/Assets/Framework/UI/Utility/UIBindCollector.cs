using UnityEngine;
using System.Collections.Generic;
using LitFramework.UI.Core.Window;
using System.Linq;

namespace LitFramework.UI.Core.Utility
{
    public static class UIBindCollector
    {
        public static UIBind[] CollectBinds(UIBase root)
        {
            var list = new List<UIBind>();
            CollectBindsExcludingSubUI(root.transform, root, list);
            return list.OrderBy(b => b.Index).ToArray();
        }

        public static void CollectBindsExcludingSubUI(Transform current, UIBase rootUI, List<UIBind> result)
        {
            var bind = current.GetComponent<UIBind>();
            if (bind != null && current != rootUI.transform)
                result.Add(bind);

            var uiBase = current.GetComponent<UIBase>();
            if (uiBase != null && uiBase != rootUI)
                return;

            foreach (Transform child in current)
                CollectBindsExcludingSubUI(child, rootUI, result);
        }
    }
}