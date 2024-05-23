using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_MainGame
{

    int calculateTroopGeneration(RiskySandBox_Team _Team)
    {
        List<RiskySandBox_Tile> _my_Tiles = RiskySandBox_Tile.all_instances.Where(x => x.my_Team == _Team).ToList();



        int _natural_generation = Math.Max(3, Mathf.FloorToInt(_my_Tiles.Count() / 3f));

        int _from_bonus = generationFromBonuses(_Team, _my_Tiles);

        if (debugging)
            GlobalFunctions.print(string.Format("_natural_generation {0}, _from_bonus {1}",_natural_generation,_from_bonus),this);

        return _natural_generation + _from_bonus;//TODO + _Team.bonus_generation e.g. maybe some people get different generation than others...
    }


    int generationFromBonuses(RiskySandBox_Team _Team,List<RiskySandBox_Tile> _Tiles)
    {
        int _return_value = 0;
        List<int> _Tiles_IDs = _Tiles.Select(x => x.ID.value).ToList();
        foreach(RiskySandBox_Bonus _Bonus in RiskySandBox_Bonus.all_instances)//go through every bonus...
        {

            bool _has_bonus = true;
            //if they are missing any of the tiles?
            foreach(int _required_ID in _Bonus.tile_IDs)//go through each of the IDs that the Team needs in order to have this bonus
            {
                if (_Tiles_IDs.Contains(_required_ID) == false)//if they don't have this tile id...
                { 
                    if (debugging)
                        GlobalFunctions.print("checking if the Team has the '" + _Bonus.name + "' bonus - answer is no as they dont control the tile with id "+_required_ID,this);
                    _has_bonus = false;
                    break;
                }
            }
            if (_has_bonus == true)//great! - give them the bonus troops...
                _return_value += _Bonus.generation;
        }

        return _return_value;//TODO - * '_Team.bonus_generation_multiplier'  for now lets keep it simple...
    }
}
