using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_MainGame
{

    public static event Action<EventInfo_Ondeploy> Ondeploy_MultiplayerBridge;
    public static event Action<EventInfo_Ondeploy> Ondeploy;


    public struct EventInfo_Ondeploy
    {
        public EventInfo_Ondeploy(RiskySandBox_Team _Team, RiskySandBox_Tile _Tile, int _n_troops)
        {
            this.Team = _Team;
            this.Tile = _Tile;
            this.n_troops = _n_troops;
        }
        public readonly RiskySandBox_Team Team;
        public readonly RiskySandBox_Tile Tile;
        public readonly int n_troops;
        public int Tile_ID { get { return this.Tile.ID; } }
        public int Team_ID { get { return this.Team.ID; } }
    }

    public void invokeEvent_Ondeploy(RiskySandBox_Team _Team, RiskySandBox_Tile _Tile, int _n_troops, bool _alert_MultiplayerBridge)
    {
        EventInfo_Ondeploy _EventInfo = new EventInfo_Ondeploy(_Team, _Tile, _n_troops);
        if (_alert_MultiplayerBridge)
            Ondeploy_MultiplayerBridge?.Invoke(_EventInfo);
        Ondeploy?.Invoke(_EventInfo);
    }




    public virtual void deploy(RiskySandBox_Team _Team, RiskySandBox_Tile _Tile, int _n_troops)
    {
        string _current_turn_state = _Team.current_turn_state;
        int _deployable_troops = _Team.deployable_troops;

        if (_current_turn_state != RiskySandBox_Team.turn_state_deploy)//if the Team is not in the deploy state...
        {
            if (debugging)
                GlobalFunctions.print("not in the deploy state...", this);
            return;
        }

        if (_deployable_troops < _n_troops)//if the Team doesnt have enough troops...
        {
            if (debugging)
                GlobalFunctions.print("can't deploy troops as we don't have enough troops... returning - _n_troops = " + _n_troops, this);
            return;
        }

        if (_Tile.my_Team != _Team)//if the Tile doesn't belong to the Team...
        {
            if (debugging)
                GlobalFunctions.print("can't deploy troops to this Tile as the Tile does not belong to this Team... returning", this);
            return;
        }

        RiskySandBox_MainGame.instance.SET_num_troops(_Tile.ID, _Tile.num_troops + _n_troops);
        _Team.deployable_troops.value -= _n_troops;

        invokeEvent_Ondeploy(_Team,_Tile, _n_troops, _alert_MultiplayerBridge: true);//tell everyone about the deploy...


        if (_Team.deployable_troops <= 0)
        {
            _Team.current_turn_state.value = RiskySandBox_Team.turn_state_attack;
        }

    }
}
