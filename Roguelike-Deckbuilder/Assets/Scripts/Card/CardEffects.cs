//
// Auto Generated Code By excel2json
// https://neil3d.gitee.io/coding/excel2json.html
// 1. 每个 Sheet 形成一个 Struct 定义, Sheet 的名称作为 Struct 的名称
// 2. 表格约定：第一行是变量名称，第二行是变量类型

// Generate From D:\GIT\Unity-Roguelike-Deckbuilder\Roguelike-Deckbuilder\Assets\Config\Excel\CardEffects.xlsx.xlsx
using System;

[Serializable]
public class CardEffects : IConfig
{
	public string ID; // ID
	public EffectType Type; // 类型
	public int Value; // 效果值
	public string Target; // 目标

    string IConfig.ID => ID;

}


// End of Auto Generated Code
