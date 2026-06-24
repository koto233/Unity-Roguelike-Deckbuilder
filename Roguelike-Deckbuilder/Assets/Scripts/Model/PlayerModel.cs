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


    public void TakeDamage(int damage)
    {
        if (damage <= 0) return;
        int finalDamage = damage;
        int oldHp = _currentHp;
        _currentHp = Mathf.Max(0, _currentHp - finalDamage);
        if (oldHp != _currentHp)
        {
            EventBus<PlayerHpChangedEvent>.Publish(new PlayerHpChangedEvent { OldHp = oldHp, NewHp = _currentHp });
            if (_currentHp == 0)
                EventBus<PlayerDiedEvent>.Publish(new PlayerDiedEvent());
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;
        int oldHp = _currentHp;
        _currentHp = Mathf.Min(_maxHp, _currentHp + amount);
        if (oldHp != _currentHp)
            EventBus<PlayerHpChangedEvent>.Publish(new PlayerHpChangedEvent { OldHp = oldHp, NewHp = _currentHp });
    }

    public void AddGold(int amount)
    {
        if (amount <= 0)
        {
            Debug.Log("添加金币数量必须大于0");
            return;
        }
        int oldGold = _gold;
        _gold = Mathf.Max(0, _gold + amount);
        if (oldGold != _gold)
            EventBus<PlayerGoldChangedEvent>.Publish(new PlayerGoldChangedEvent { OldGold = oldGold, NewGold = _gold });
    }

    public bool SpendGold(int amount)
    {
        if (amount <= 0 || _gold < amount)
        {
            Debug.Log("金币不足或者消耗数量必须大于0");
            return false;
        }
        int oldGold = _gold;
        _gold -= amount;
        EventBus<PlayerGoldChangedEvent>.Publish(new PlayerGoldChangedEvent { OldGold = oldGold, NewGold = _gold });
        return true;
    }

    public void SetEnergy(int energy)
    {
        int old = _energy;
        _energy = Mathf.Max(0, energy);
        if (old != _energy)
            EventBus<EnergyChangedEvent>.Publish(new EnergyChangedEvent { CurrentEnergy = old, });
    }
    public void AddEnergy(int amount)
    {
        int old = _energy;
        _energy = Mathf.Min(3, _energy + amount);
        if (old != _energy)
            EventBus<EnergyChangedEvent>.Publish(new EnergyChangedEvent { CurrentEnergy = old, });
    }
    public bool SpendEnergy(int cost)
    {
        if (cost <= 0 || _energy < cost) return false;
        int old = _energy;
        _energy -= cost;
        EventBus<EnergyChangedEvent>.Publish(new EnergyChangedEvent { CurrentEnergy = old });
        return true;
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