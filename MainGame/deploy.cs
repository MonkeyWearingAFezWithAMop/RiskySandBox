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


    public bool canDeploy(RiskySandBox_Team _Team,RiskySandBox_Tile _Tile,int _n_troops)
    {
        if (_Team == null || _Tile == null)
        {
            if (this.debugging)
                GlobalFunctions.print("_Team was null or _Tile was null... returning false _Team = "+_Team+" _Tile = "+_Tile, this);
            return false;
        }
            

        if (_Team.current_turn_state != RiskySandBox_Team.turn_state_deploy)//if the Team is not in the deploy state...
        {
            if (debugging)
                GlobalFunctions.print("not in the deploy state...", this);
            return false;
        }

        if (_Team.deployable_troops < _n_troops)//if the Team doesnt have enough troops...
        {
            if (debugging)
                GlobalFunctions.print("can't deploy troops as we don't have enough troops... returning - _n_troops = " + _n_troops, this);
            return false;
        }

        if (_Tile.my_Team == _Team)//TODO - what if we made it so there is a setting that means you cant deploy to your own tiles??? maybe in a team game you can only deploy to your teams tiles?
            //this is probs a bad idea however so maybe lets not care for now...
            return true;

        
        if (RiskySandBox_AllianceSettings.enable_alliances == true)
        {
            bool _is_ally = _Team.ally_ids.Contains(_Tile.my_Team.ID);

            if (_is_ally == false || RiskySandBox_AllianceSettings.allow_deploy_to_ally_Tiles == false)//if it ISNT an ally OR we are not allowed to deploy to ally Tiles
                return false ;//dont allow this...
        }

        else
        {
            if (debugging)
                GlobalFunctions.print("can't deploy troops to this Tile as the Tile does not belong to this Team... returning", this);
            return false;
        }

        

        return true;
    }


    public virtual bool deploy(RiskySandBox_Team _Team, RiskySandBox_Tile _Tile, int _n_troops)
    {

        bool _can_deploy = RiskySandBox_MainGame.instance.canDeploy(_Team, _Tile, _n_troops);

        if (_can_deploy == false)
            return false;


        RiskySandBox_MainGame.instance.SET_num_troops(_Tile.ID, _Tile.num_troops + _n_troops);
        _Team.deployable_troops.value -= _n_troops;

        invokeEvent_Ondeploy(_Team,_Tile, _n_troops, _alert_MultiplayerBridge: true);//tell everyone about the deploy...


        if (_Team.deployable_troops <= 0)
        {
            _Team.current_turn_state.value = RiskySandBox_Team.turn_state_attack;
        }

        return true;

    }
}
