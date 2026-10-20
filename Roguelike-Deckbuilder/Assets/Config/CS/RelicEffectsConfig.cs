//
// Auto Generated Code By excel2json
// https://neil3d.gitee.io/coding/excel2json.html
// 1. 每个 Sheet 形成一个 Struct 定义, Sheet 的名称作为 Struct 的名称
// 2. 表格约定：第一行是变量名称，第二行是变量类型

// Generate From D:\GIT\Unity-Roguelike-Deckbuilder\Roguelike-Deckbuilder\Assets\Config\Excel\RelicEffects.xlsx
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[System.Serializable]
public class RelicEffectsConfig : IConfig
{
	public int Id; // Id
	public int RelicId; // 遗物ID
	public int Order; // 同一遗物内的执行顺序
	[JsonConverter(typeof(StringEnumConverter))]
	public RelicEffectType Type; // 效果类型
	public int Amount; // 通用数值（治疗/伤害/格挡）
	public float Threshold; // 触发阈值（0~1）
	public int Strength; // 力量值
	public float Multiplier; // 倍率
	public int PlayerStrength; // 玩家力量
	public int EnemyStrength; // 敌人力量
}


// End of Auto Generated Code
