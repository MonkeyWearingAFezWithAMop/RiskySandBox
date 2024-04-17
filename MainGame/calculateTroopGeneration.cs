using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_MainGame
{

    int calculateTroopGeneration(RiskySandBox_Team _Team)
    {
        //TODO  - actually do this...
        List<RiskySandBox_Tile> _my_Tiles = RiskySandBox_Tile.all_instances.Where(x => x.my_Team == _Team).ToList();

        //my_Tiles.count / 3 (round down) + bonuses + this.bonus_generation?
        return 100;
    }
}
