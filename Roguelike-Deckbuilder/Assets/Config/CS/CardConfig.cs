//
// Auto Generated Code By excel2json
// https://neil3d.gitee.io/coding/excel2json.html
// 1. 每个 Sheet 形成一个 Struct 定义, Sheet 的名称作为 Struct 的名称
// 2. 表格约定：第一行是变量名称，第二行是变量类型

// Generate From D:\GIT\Unity-Roguelike-Deckbuilder\Roguelike-Deckbuilder\Assets\Config\Excel\CardConfig.xlsx.xlsx
using System;

[Serializable]
public class CardConfig : IConfig
{
    public int Id; // Id
    public string Icon; // 图片
    public string Name; // 名称
    public int Cost; // 费用
    public string Type; // 类型
    public CardEffectEntry[] Effects; // 效果
    public int UpgradeId; // 强化id
}


// End of Auto Generated Code


public class CardEffectEntry
{
    public int EffectId;
    public int Value;
}

