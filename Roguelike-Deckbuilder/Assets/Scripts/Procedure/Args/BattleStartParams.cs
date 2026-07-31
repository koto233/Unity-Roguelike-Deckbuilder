using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class BattleStartParams : IProcedureArgs
{
    public int EnemyId { get; set; }
    public bool IsElite { get; set; }
}
