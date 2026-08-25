using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSceneRoot : MonoBehaviour
{
    [SerializeField] private BattleView _uiBattle;
    public BattleView UIBattle => _uiBattle;
}
