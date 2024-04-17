using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_MainGame
{
    public static event Action<EventInfo_Oncapture> Oncapture;


    public struct EventInfo_Oncapture
    {
        public EventInfo_Oncapture(RiskySandBox_Team _Team, RiskySandBox_Tile _Tile, int _n_troops)
        {
            this.Team = _Team;
            this.Tile = _Tile;
            this.n_troops = _n_troops;
        }

        public readonly RiskySandBox_Team Team;
        public readonly RiskySandBox_Tile Tile;
        public readonly int n_troops;
    }

    public void invokeEvent_Oncapture(RiskySandBox_Team _Team, RiskySandBox_Tile _Tile, int _n_troops)
    {
        EventInfo_Oncapture _EventInfo = new EventInfo_Oncapture(_Team, _Tile, _n_troops);
        Oncapture?.Invoke(_EventInfo);
    }





    public void capture(RiskySandBox_Team _Team, int _n_troops)
    {

        if (_Team.current_turn_state != RiskySandBox_Team.turn_state_capture)
        {
            if (debugging)
                GlobalFunctions.print("not in the capture state...", _Team);
            return;
        }

        RiskySandBox_Tile _from = RiskySandBox_Tile.GET_RiskySandBox_Tile(_Team.capture_start_ID.value);
        RiskySandBox_Tile _Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(_Team.capture_end_ID.value);



        //TODO - make sure there is atleast <1> troop left on the captureing tile...
        if (_from.num_troops - _n_troops < RiskySandBox_Tile.min_troops_per_Tile)
        {
            if (debugging)
                GlobalFunctions.print("not enough troops", this);
            return;
        }

        RiskySandBox_MainGame.instance.SET_num_troops(_from.ID, _from.num_troops - _n_troops);
        RiskySandBox_MainGame.instance.SET_num_troops(_Tile.ID, _Tile.num_troops + _n_troops);

        this.invokeEvent_Oncapture(_Team, _Tile, _n_troops);

        _Team.current_turn_state.value = RiskySandBox_Team.turn_state_attack;
    }
}
