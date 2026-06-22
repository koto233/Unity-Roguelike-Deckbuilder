using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface ICardEffect
{
    void Execute(Card card, BattleContext context);

}