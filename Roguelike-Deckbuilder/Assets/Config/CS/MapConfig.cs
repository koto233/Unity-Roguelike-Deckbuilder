//
// Auto Generated Code By excel2json
// https://neil3d.gitee.io/coding/excel2json.html
// 1. 每个 Sheet 形成一个 Struct 定义, Sheet 的名称作为 Struct 的名称
// 2. 表格约定：第一行是变量名称，第二行是变量类型

// Generate From D:\GIT\Unity-Roguelike-Deckbuilder\Roguelike-Deckbuilder\Assets\Config\Excel\MapConfig.xlsx.xlsx

public class MapConfig : IConfig
{
	public int Templateld; // 模板Id
	public int Row; // 行
	public int[] ColumnPositions; // 节点
	public int BattleWeight; // 战斗概率
	public int EliteWeight; // 精英概率
	public int RestWeight; // 休息点概率
	public int ShopWeight; // 商店概率
	public int EventWeight; // 事件概率
}


// End of Auto Generated Code
