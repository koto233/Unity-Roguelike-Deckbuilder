using System;

[Flags]
public enum CardTrait
{
    None = 0,
    Exhaust = 1 << 0,      // 消耗：使用后移除
    Ethereal = 1 << 1,     // 虚无：回合结束若在手牌则消耗
    Retain = 1 << 2,       // 保留：回合结束不弃牌
    Innate = 1 << 3,       // 固有：战斗开始时必定在起手
}