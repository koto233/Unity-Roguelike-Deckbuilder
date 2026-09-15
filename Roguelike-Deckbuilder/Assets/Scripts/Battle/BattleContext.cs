using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleContext
{
    public List<CharacterBase> AllCharacters { get; set; }      // 所有战斗单位（玩家+敌人）
    public Player Player { get; set; }            // 玩家角色
    public List<Enemy> Enemies { get; set; }     // 当前敌人列表
    public CharacterBase Target { get; set; }            // 当前选中的目标
    public CharacterBase Attacker { get; set; }
    public int CurrentTurn { get; set; } = 0;
    public bool IsPlayerTurn { get; set; }
    public int GoldReward { get; set; }            // 战斗胜利金币
    // // 可选：战斗管理器引用（用于触发额外效果、状态变更等）
    // public IBattleController BattleController { get; set; } // 可以是一个接口，但不是必须

    // // 其他临时数据
    // public int CurrentTurnCount { get; set; }
    // public Dictionary<string, object> CustomData { get; set; } // 扩展用
}
