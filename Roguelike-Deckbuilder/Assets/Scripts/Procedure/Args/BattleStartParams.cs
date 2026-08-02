using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class BattleStartParams : IProcedureArgs
{
    public List<int> EnemyIds { get; set; }
    public bool IsElite { get; set; }
}
