using System.Collections;
using System.Collections.Generic;
using LitFramework.EventBus;
using UnityEngine;

public class PlayerModel : IModel
{
    private int _currentHp;
    private int _maxHp;
    private int _energy;
    private int _gold;

    public int CurrentHp => _currentHp;
    public int MaxHp => _maxHp;
    public int Energy => _energy;
    public int Gold => _gold;

    public PlayerModel()
    {
        _maxHp = 80;
        _currentHp = _maxHp;
        _energy = 3;
        _gold = 0;
    }

    public void OnRegister()
    {

    }

    // 用于存档加载（批量恢复，避免多次事件）
    // public void RestoreFromSave(SaveData data)
    // {
    //     _maxHp = data.playerMaxHp;
    //     _currentHp = data.playerCurrentHp;
    //     _gold = data.playerGold;
    //     // 能量通常不存档，回合开始时重置
    //     EventBus<PlayerDataRefreshedEvent>.Publish(new PlayerDataRefreshedEvent());
    // }
}