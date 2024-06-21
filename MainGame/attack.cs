using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_MainGame
{
    public static event Action<EventInfo_Onattack> Onattack_MultiplayerBridge;
    public static event Action<EventInfo_Onattack> Onattack;

    public struct EventInfo_Onattack
    {
        public EventInfo_Onattack(RiskySandBox_Team _attacking_Team,RiskySandBox_Team _defending_Team, RiskySandBox_Tile _start_Tile,RiskySandBox_Tile _target_Tile,int _attacker_deaths,int _defender_deaths,bool _capture_flag,string _attack_method)
        {
            this.attacking_Team = _attacking_Team;
            this.defending_Team = _defending_Team;
            this.start_Tile = _start_Tile;
            this.target_Tile = _target_Tile;
            this.attacker_deaths = _attacker_deaths;
            this.defender_deaths = _defender_deaths;
            this.capture_flag = _capture_flag;
            this.attack_method = _attack_method;
        }
        public readonly RiskySandBox_Tile start_Tile;
        public readonly RiskySandBox_Tile target_Tile;
        public readonly RiskySandBox_Team attacking_Team;
        public readonly RiskySandBox_Team defending_Team;
        public readonly int attacker_deaths;
        public readonly int defender_deaths;
        public readonly bool capture_flag;
        public readonly string attack_method;
    }

    public void invokeEvent_Onattack(RiskySandBox_Team _attacking_Team,RiskySandBox_Team _defending_Team, RiskySandBox_Tile _start_Tile,RiskySandBox_Tile _target_Tile,int _attacker_deaths,int _defender_deaths,bool _capture_flag,string _attack_method,bool _alert_MultiplayerBridge)
    {
        EventInfo_Onattack _EventInfo = new EventInfo_Onattack(_attacking_Team,_defending_Team, _start_Tile, _target_Tile, _attacker_deaths, _defender_deaths, _capture_flag, _attack_method);
        if (_alert_MultiplayerBridge == true)
            Onattack_MultiplayerBridge?.Invoke(_EventInfo);
        Onattack?.Invoke(_EventInfo);
    }



    public bool canAttack(RiskySandBox_Tile _from, RiskySandBox_Tile _to)
    {
        if (_from == null || _to == null)
            return false;//TODO - debug WTF?!?!?!

        //TODO - add in if(debugging) statements....
        if (_from.my_Team.current_turn_state != RiskySandBox_Team.turn_state_attack)
        {
            return false;
        }

        if (_from.my_Team == _to.my_Team)//cant attack self... (TODO - unless you can???)
        {
            return false;
        }

        if ((int)_from.num_troops <= RiskySandBox_Tile.min_troops_per_Tile)
        {
            return false;
        }


        if (_from.graph_connections.Contains(_to) == false)
        {
            return false;
        }

        if (RiskySandBox_AllianceSettings.enable_alliances == true)//if alliances are enabled?
        {
            bool _is_ally = _from.my_Team.ally_ids.Contains(_to.my_Team.ID);

            if (_is_ally && RiskySandBox_AllianceSettings.allow_attack_ally_Tiles.value == false)
            {
                //if the end tile is an ally???  AND we can't attack allies...
                if (debugging)
                    GlobalFunctions.print("can't attack an allies Tile!", this);
                return false;
            }
        }




        return true;
            


    }


    public void attack(RiskySandBox_Tile _start_Tile, RiskySandBox_Tile _target_Tile,int _n_troops, string _attack_method)
    {
        RiskySandBox_Team _attacking_Team = _start_Tile.my_Team;

        if(_attacking_Team.current_turn_state != RiskySandBox_Team.turn_state_attack)
        {
            if (this.debugging)
                GlobalFunctions.print("team isnt in the attack state...", this);
            return;
        }

        bool _can_attack = this.canAttack(_start_Tile, _target_Tile);

        if(_can_attack == false)
        {
            if (this.debugging)
                GlobalFunctions.print("canAttack returned false...", this);
            return;
        }




        if (_start_Tile.num_troops < _n_troops + RiskySandBox_Tile.min_troops_per_Tile)
        {
            if (this.debugging)
                GlobalFunctions.print("not enough troops!... returning", this);
            return;
        }

        int _attacker_deaths;
        int _defender_deaths;

        if(_target_Tile.has_capital == true)
            RiskySandBox_AttackSimulations.doBattle(_n_troops, _target_Tile.num_troops.value, RiskySandBox_AttackSimulations.capitals_mode_string, out _attacker_deaths, out _defender_deaths);
        else
            RiskySandBox_AttackSimulations.doBattle(_n_troops, _target_Tile.num_troops.value, _attack_method,out _attacker_deaths, out _defender_deaths);

        
        

        RiskySandBox_MainGame.instance.SET_num_troops(_start_Tile.ID, _start_Tile.num_troops.value - _attacker_deaths);
        RiskySandBox_MainGame.instance.SET_num_troops(_target_Tile.ID, _target_Tile.num_troops.value - _defender_deaths);


        bool _capture_flag = _target_Tile.num_troops.value <= 0;

        RiskySandBox_Team _defender_Team = _target_Tile.my_Team ;

        if (_capture_flag)
        {
            RiskySandBox_MainGame.instance.SET_my_Team(_target_Tile.ID, _attacking_Team.ID);
            _attacking_Team.capture_start_ID.value = _start_Tile.ID;
            _attacking_Team.capture_end_ID.value = _target_Tile.ID;
            _attacking_Team.current_turn_state.value = "capture";
        }


        this.invokeEvent_Onattack(_attacking_Team, _defender_Team, _start_Tile, _target_Tile, _attacker_deaths, _defender_deaths, _capture_flag, _attack_method, _alert_MultiplayerBridge: true);


    }




}
