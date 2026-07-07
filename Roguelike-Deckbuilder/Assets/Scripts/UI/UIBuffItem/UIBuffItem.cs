using System.Collections;
using System.Collections.Generic;
using LitFramework.EventBus;
using LitFramework.UI.Core.Window;
using UnityEngine;
using UnityEngine.EventSystems;

public partial class UIBuffItem : UIBase, IPointerEnterHandler, IPointerExitHandler
{

    private IBuff _buff;
    public void Init(IBuff buff)
    {
        // 加载图标
        _buff = buff;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EventBus<HoverEvent>.Publish(new HoverEvent
        {
            Data = _buff,
            ScreenPosition = eventData.position,
            IsHovering = true
        });
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        EventBus<HoverEvent>.Publish(new HoverEvent
        {
            IsHovering = false
        });
    }

    public void SetStacks(int stacks)
    {
        b_StackText.SetText(stacks.ToString());
        b_StackText.gameObject.SetActive(stacks > 1);
    }
}
