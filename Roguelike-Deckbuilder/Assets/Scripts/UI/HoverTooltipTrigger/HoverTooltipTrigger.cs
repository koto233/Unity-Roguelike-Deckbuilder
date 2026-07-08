using LitFramework.EventBus;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private ITooltipDataProvider _provider;
    [SerializeField] private TooltipType _tooltipType;
    private TooltipType GetTooltipType() => _tooltipType;
    private void Awake()
    {
        // 尝试从自身或父物体获取数据提供者
        _provider = GetComponent<ITooltipDataProvider>()
                     ?? GetComponentInParent<ITooltipDataProvider>();

        if (_provider == null)
        {
            Debug.LogWarning("HoverTooltipTrigger 缺少 ITooltipDataProvider 实现", this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_provider == null) return;

        // 发布事件，Presenter会接收
        EventBus<TooltipShowEvent>.Publish(new TooltipShowEvent
        {
            Type = GetTooltipType(), // 可由子类重写，或通过 Inspector 配置
            Data = _provider.GetTooltipData(),
            Position = eventData.position,
            IsHovering = true
        });
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        EventBus<TooltipShowEvent>.Publish(new TooltipShowEvent
        {
            IsHovering = false
        });
    }
}

