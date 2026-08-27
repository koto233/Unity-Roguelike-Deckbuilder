//
// Auto Generated Code By excel2json
// https://neil3d.gitee.io/coding/excel2json.html
// 1. 每个 Sheet 形成一个 Struct 定义, Sheet 的名称作为 Struct 的名称
// 2. 表格约定：第一行是变量名称，第二行是变量类型

// Generate From D:\GIT\Unity-Roguelike-Deckbuilder\Roguelike-Deckbuilder\Assets\Config\Excel\EncounterConfig.xlsx.xlsx
using System;

[Serializable]
public class EncounterConfig : IConfig
{
	public int Id; // Id
	public string Name; // 名称
	public int MinRow; // 出现最小列
	public int MaxRow; // 出现最大列
	public string NodeType; // 节点类型
	public int[] EnemyIds; // 怪物配置
	public int Weight; // 权重
}


// End of Auto Generated Code
