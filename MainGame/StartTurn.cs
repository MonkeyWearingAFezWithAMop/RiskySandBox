using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_MainGame : MonoBehaviour
{

    protected virtual void startTurn(RiskySandBox_Team _Team)
    {

        _Team.end_turn_time_stamp.value = RiskySandBox.instance.current_time.value + this.turn_length_seconds.value;//TODO - replace with _Team.turn_length_seconds...
        _Team.is_my_turn.value = true;


        _Team.deployable_troops.value += calculateTroopGeneration(_Team);

        





        if (_Team.required_capital_placements > 0)//if the team has to place a capital...
        {
            _Team.current_turn_state.value = RiskySandBox_Team.turn_state_placing_capital;
        }

        else if(_Team.num_cards >= this.max_num_cards)
        {
            if (this.debugging)
                GlobalFunctions.print("putting _Team into the force trade in state... because the have too many cards...", _Team);
            _Team.current_turn_state.value = RiskySandBox_Team.turn_state_force_trade_in;
        }

        else if (_Team.deployable_troops > 0)
        {
            _Team.current_turn_state.value = RiskySandBox_Team.turn_state_deploy;
        }




    }
}
