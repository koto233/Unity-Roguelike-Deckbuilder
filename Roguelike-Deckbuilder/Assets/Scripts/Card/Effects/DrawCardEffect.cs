using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCardEffect : ICardEffect
{
    private int _count;
    public DrawCardEffect(int value)
    {
        _count = value;
    }
    public void Execute(Card card, BattleContext context)
    {
        Debug.Log("抽牌效果：" + _count);
        context.Player.DrawCards(_count);
    }


}
