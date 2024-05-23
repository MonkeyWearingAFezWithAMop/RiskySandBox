using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_MainGame
{
    public static event Action<EventInfo_Onattack> Onattack_MultiplayerBridge;
    public static event Action<EventInfo_Onattack> Onattack;

    public struct EventInfo_Onattack
    {
        public EventInfo_Onattack(RiskySandBox_Team _Team, RiskySandBox_Tile _start_Tile,RiskySandBox_Tile _target_Tile,int _attacker_deaths,int _defender_deaths,bool _capture_flag,string _attack_method)
        {
            this.Team = _Team;
            this.start_Tile = _start_Tile;
            this.target_Tile = _target_Tile;
            this.attacker_deaths = _attacker_deaths;
            this.defender_deaths = _defender_deaths;
            this.capture_flag = _capture_flag;
            this.attack_method = _attack_method;
        }
        public readonly RiskySandBox_Team Team;
        public readonly RiskySandBox_Tile start_Tile;
        public readonly RiskySandBox_Tile target_Tile;
        public readonly int attacker_deaths;
        public readonly int defender_deaths;
        public readonly bool capture_flag;
        public readonly string attack_method;
    }

    public void invokeEvent_Onattack(RiskySandBox_Team _Team, RiskySandBox_Tile _start_Tile,RiskySandBox_Tile _target_Tile,int _attacker_deaths,int _defender_deaths,bool _capture_flag,string _attack_method,bool _alert_MultiplayerBridge)
    {
        EventInfo_Onattack _EventInfo = new EventInfo_Onattack(_Team, _start_Tile, _target_Tile, _attacker_deaths, _defender_deaths, _capture_flag, _attack_method);
        if (_alert_MultiplayerBridge == true)
            Onattack_MultiplayerBridge?.Invoke(_EventInfo);
        Onattack?.Invoke(_EventInfo);
    }


    public void attack(RiskySandBox_Team _Team, RiskySandBox_Tile _start_Tile, RiskySandBox_Tile _target_Tile,int _n_troops, string _attack_method)
    {
        if (_Team.current_turn_state != RiskySandBox_Team.turn_state_attack)
        {
            if (debugging)
                GlobalFunctions.print("not in the attack state...", _Team);
            return;
        }
        if (_start_Tile.my_Team != _Team)
        {
            if (debugging)
                GlobalFunctions.print("can't attack as _from.my_Team != this... returning", _Team);
            return;
        }
        if (_target_Tile.my_Team == _Team)
        {
            if (debugging)
                GlobalFunctions.print("can't attack as _to.my_Team == this... returning", _Team);
            return;
        }

        //make sure there is a connection between the start tile and the end tile

        if(_start_Tile.graph_connections.Contains(_target_Tile.ID) == false)
        {
            if (debugging)
                GlobalFunctions.print("cant attack as the start tile ids graph_connections doesnt contain the target_Tile_ID",this);
            return;
        }

        if(RiskySandBox_MainGame.instance.enable_alliances)
        {
            bool _is_ally = _start_Tile.my_Team.ally_ids.Contains(_target_Tile.my_Team.ID);

            if(_is_ally && (RiskySandBox_MainGame.instance.allow_attack_ally_Tiles.value == false))
            {
                //if the end tile is an ally???  AND we can't attack allies...
                if (debugging)
                    GlobalFunctions.print("can't attack an allies Tile!", this);
                return;
            }
        }

        //TODO - run the simulation using the _attack_method...

        int _remaining_attackers;
        int _remaining_defenders;

        if(_target_Tile.has_capital == true)
            RiskySandBox_AttackSimulations.doBattle(_start_Tile.num_troops.value - 1, _target_Tile.num_troops.value, RiskySandBox_AttackSimulations.capitals_mode_string, out _remaining_attackers, out _remaining_defenders);
        else
            RiskySandBox_AttackSimulations.doBattle(_start_Tile.num_troops.value - 1, _target_Tile.num_troops.value, _attack_method,out _remaining_attackers, out _remaining_defenders);

        int _defender_deaths = _target_Tile.num_troops.value - _remaining_defenders;
        int _attacker_deaths = _start_Tile.num_troops.value - _remaining_attackers;
        

        RiskySandBox_MainGame.instance.SET_num_troops(_start_Tile.ID, _remaining_attackers + 1);//attacker must always be left with 1 troop on the tile...
        RiskySandBox_MainGame.instance.SET_num_troops(_target_Tile.ID, _remaining_defenders);


        bool _capture_flag = _remaining_defenders <= 0;


        this.invokeEvent_Onattack(_Team, _start_Tile, _target_Tile, _attacker_deaths, _defender_deaths, _capture_flag, _attack_method, _alert_MultiplayerBridge: true);


        if (_capture_flag)
        {
            RiskySandBox_MainGame.instance.SET_my_Team(_target_Tile.ID, _Team.ID);
            _Team.capture_start_ID.value = _start_Tile.ID;
            _Team.capture_end_ID.value = _target_Tile.ID;
            _Team.current_turn_state.value = "capture";
        }
    }




}
