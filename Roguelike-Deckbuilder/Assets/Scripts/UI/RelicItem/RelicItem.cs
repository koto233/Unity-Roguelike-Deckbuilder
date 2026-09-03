using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RelicItem : MonoBehaviour, ITooltipDataProvider
{
    private RelicDisplayData _data;
    private Image _icon;
    void Awake()
    {
        _icon = GetComponent<Image>();
    }

    public void Init(RelicDisplayData data)
    {
        _data = data;
        _icon.sprite = data.Icon;
        
    }

    public TooltipData GetTooltipData()
    {
        return new TooltipData
        {
            Description = _data.Description,
        };
    }
}
