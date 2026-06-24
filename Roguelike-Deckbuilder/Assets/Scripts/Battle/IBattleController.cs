using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattleController
{
    BattleContext Context { get; }
    bool PlayCard(string cardId, EnemyData target);
    void EndTurn();
    void OnPlayerDeath();
    void OnAllEnemiesDefeated();
}