using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattleController
{
    BattleContext Context { get; }
    void PlayCard(Card card, CharacterData target = null);
    void EndTurn();
    void OnPlayerDeath();
    void OnAllEnemiesDefeated();
}