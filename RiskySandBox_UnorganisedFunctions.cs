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

    public static void assignAssassinTargets()
    {

        List<int> _unassigned_targets = RiskySandBox_Team.all_instances.Select(x => x.ID.value).ToList();



        foreach (RiskySandBox_Team _Team in RiskySandBox_Team.all_instances)
        {

            int _Team_target = -1;

            List<int> _candidates = _unassigned_targets.Where(x => x != _Team.ID).ToList();

            if (_candidates.Count > 0)
            {
                int _random_index = GlobalFunctions.randomInt(0, _candidates.Count - 1);

                _Team_target = _candidates[_random_index];
                _unassigned_targets.Remove(_Team_target);
            }

            _Team.assassin_target_ID.value = _Team_target;
        }
    }


    public static void createBlizards(int _target_n_blizards)
    {

    }


}
