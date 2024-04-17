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
        //TODO - run the simulation using the _attack_method...

        int _n_kills = 0;
        int _n_deaths = 0;

        for (int _i = 0; _i < _n_troops; _i += 1)
        {
            if ((_start_Tile.num_troops - _n_deaths) <= RiskySandBox_Tile.min_troops_per_Tile)
                break;

            if ((_target_Tile.num_troops - _n_kills) <= 0)
                break;

            int _RNG = GlobalFunctions.randomInt(0, 1);

            if (_RNG == 0)
                _n_kills += 1;
            else
                _n_deaths += 1;
        }

        RiskySandBox_MainGame.instance.SET_num_troops(_start_Tile.ID, _start_Tile.num_troops - _n_deaths);
        RiskySandBox_MainGame.instance.SET_num_troops(_target_Tile.ID, _target_Tile.num_troops - _n_kills);


        bool _capture_flag = _target_Tile.num_troops <= 0;



        if (_capture_flag)
        {
            RiskySandBox_MainGame.instance.SET_my_Team(_target_Tile.ID, _Team.ID);
            _Team.capture_start_ID.value = _start_Tile.ID;
            _Team.capture_end_ID.value = _target_Tile.ID;
            _Team.current_turn_state.value = "capture";
        }
    }




}
