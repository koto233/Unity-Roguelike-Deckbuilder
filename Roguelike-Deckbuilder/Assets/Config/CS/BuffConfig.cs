//
// Auto Generated Code By excel2json
// https://neil3d.gitee.io/coding/excel2json.html
// 1. 每个 Sheet 形成一个 Struct 定义, Sheet 的名称作为 Struct 的名称
// 2. 表格约定：第一行是变量名称，第二行是变量类型

// Generate From D:\GIT\Unity-Roguelike-Deckbuilder\Roguelike-Deckbuilder\Assets\Config\Excel\BuffConfig.xlsx.xlsx

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[System.Serializable]
public class BuffConfig : IConfig
{
	public int Id; // Id
	public string Key; // 资源标识
	public string Name; // 名称
	[JsonConverter(typeof(StringEnumConverter))]
	public BuffDurationType DurationType; // 持续类型
	public int IsDebuff; // 是否是减益
	public int Value; // 数值
	public int MaxStacks; // 最大叠层
	public string Description; // 描述
}


// End of Auto Generated Code
