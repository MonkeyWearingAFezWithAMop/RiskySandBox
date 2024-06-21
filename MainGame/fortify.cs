using System.Collections;using System.Collections.Generic;using System.Linq;using System;


public partial class RiskySandBox_MainGame
{
    public static event Action<EventInfo_Onfortify> Onfortify_MultiplayerBridge;
    public static event Action<EventInfo_Onfortify> Onfortify;


    public struct EventInfo_Onfortify
    {
        public EventInfo_Onfortify(RiskySandBox_Team _Team, RiskySandBox_Tile _from, RiskySandBox_Tile _to, int _n_troops)
        {
            this.Team = _Team;
            this.from = _from;
            this.to = _to;
            this.n_troops = _n_troops;
        }
        public readonly RiskySandBox_Team Team;
        public readonly RiskySandBox_Tile from;
        public readonly RiskySandBox_Tile to;
        public readonly int n_troops;

    }

    public void invokeEvent_Onfortify(RiskySandBox_Team _Team, RiskySandBox_Tile _from, RiskySandBox_Tile _to, int _n_troops,bool _alert_MultiplayerBridge)
    {
        EventInfo_Onfortify _EventInfo = new EventInfo_Onfortify(_Team, _from, _to, _n_troops);
        if (_alert_MultiplayerBridge == true)
            Onfortify_MultiplayerBridge?.Invoke(_EventInfo);
        Onfortify?.Invoke(_EventInfo);

    }


    public bool canFortify(RiskySandBox_Tile _from,RiskySandBox_Tile _to,int _n_troops)
    {
        if (_from == null || _to == null)
        {
            if (this.debugging)
                GlobalFunctions.print("_from or _to was null _from = " + _from + " _to = " + _to, this);
            return false;
        }

        RiskySandBox_Team _Team = _from.my_Team;

        if (_Team.current_turn_state != RiskySandBox_Team.turn_state_fortify)
        {
            if (debugging)
                GlobalFunctions.print("not in the fortify state...", this);
            return false;
        }

       
        if (_to.my_Team != _Team)
        {
            if (RiskySandBox_AllianceSettings.allow_fortify_to_ally_Tiles.value == false)
            {
                if (debugging)
                    GlobalFunctions.print("unable to fortify as _to doesnt belong to this Team and allow_fortify_to_ally_Tiles.value == false returning...", this);
                return false;

            }

            bool _is_ally = _from.my_Team.ally_ids.Contains(_to.my_Team_ID);
            if (_is_ally == false)
            {
                if (debugging)
                    GlobalFunctions.print("unable to fortify as _to doesnt belong to this Team and it's not an ally returning...", this);
                return false;
            }
        }

        if (_from.num_troops <= _n_troops)//TODO - need to also think about RiskySandBox_Tile.min_troops_per_tile...
        {
            if (debugging)
                GlobalFunctions.print("unable to fortify as _from.num_troops <= _n_troops", this);
            return false;
        }

        List<RiskySandBox_Tile> _path = RiskySandBox_MainGame.findPath(_from, _to);

        if (_path == null || _path.Count == 0)
        {
            if (debugging)
                GlobalFunctions.print("unable to find a route from " + _from + " _to " + _to + " returning", this);
            return false;
        }

        return true;
    }


    public virtual bool fortify(RiskySandBox_Tile _from, RiskySandBox_Tile _to, int _n_troops)
    {

        bool _can_fortify = RiskySandBox_MainGame.instance.canFortify(_from, _to, _n_troops);

        if (_can_fortify == false)
            return false;


        RiskySandBox_MainGame.instance.SET_num_troops(_from.ID, _from.num_troops - _n_troops);//reduce troops from _from
        RiskySandBox_MainGame.instance.SET_num_troops(_to.ID, _to.num_troops + _n_troops);//give troops to _to


        invokeEvent_Onfortify(_from.my_Team, _from, _to, _n_troops, _alert_MultiplayerBridge: true);

        return true;
    }
}
