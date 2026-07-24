using LitFramework.UI.Core.Window;
using UnityEngine;
using UnityEngine.UI;

public partial class UIMapNode : UIBase
{
    public Button Button => b_Button;

    private MapNodeData _nodeData;

    public void SetNodeData(MapNodeData data)
    {
        _nodeData = data;
    }

    public void SetType(MapNodeType type)
    {
        b_Name.text = type switch
        {
            MapNodeType.Battle => "战斗",
            MapNodeType.Elite => "精英",
            MapNodeType.Rest => "休息",
            MapNodeType.Shop => "商店",
            MapNodeType.Event => "事件",
            MapNodeType.Boss => "Boss",
            _ => "未知"
        };

        Color color = type switch
        {
            MapNodeType.Battle => Color.red,
            MapNodeType.Elite => new Color(1f, 0.7f, 0f),
            MapNodeType.Rest => Color.green,
            MapNodeType.Shop => new Color(0.8f, 0.6f, 0f),
            MapNodeType.Event => Color.yellow,
            MapNodeType.Boss => new Color(0.5f, 0f, 0.5f),
            _ => Color.gray
        };
        b_Icon.color = color;
    }

    public void SetLocked(bool isLocked)
    {
        b_Lock.gameObject.SetActive(isLocked);
    }

    public void SetVisited(bool isVisited)
    {
        if (isVisited)
        {
            b_Icon.color = new Color(0.5f, 0.5f, 0.5f);
            b_HighLight.gameObject.SetActive(false);
        }
        else
        {
            b_HighLight.gameObject.SetActive(true);
        }
    }

    public void SetStart(bool isStart)
    {
        if (isStart)
        {
            b_Name.text = "起点";
            b_HighLight.color = Color.blue;
        }
    }
}