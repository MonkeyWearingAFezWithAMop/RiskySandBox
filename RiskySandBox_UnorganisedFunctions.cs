using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_UnorganisedFunctions : MonoBehaviour
{
    [SerializeField] bool debugging;
	


    // Update is called once per frame
    public static void startGameDistributeTroops_simple()
    {
        //ok lets go through all the tiles...
        int _current_Team_index = 0;
        foreach (RiskySandBox_Tile _Tile in RiskySandBox_Tile.all_instances)//make sure every single tile has some troops on it...
        {
            RiskySandBox_MainGame.instance.SET_my_Team(_Tile.ID, RiskySandBox_Team.all_instances[_current_Team_index].ID);
            RiskySandBox_MainGame.instance.SET_num_troops(_Tile.ID, 1);
            _current_Team_index += 1;
            if (_current_Team_index >= RiskySandBox_Team.all_instances.Count)
                _current_Team_index = 0;
        }
    }
}
