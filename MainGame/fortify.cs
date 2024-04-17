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


    public virtual bool fortify(RiskySandBox_Team _Team, RiskySandBox_Tile _from, RiskySandBox_Tile _to, int _n_troops)
    {
        if (_Team.current_turn_state != RiskySandBox_Team.turn_state_fortify)
        {
            if (debugging)
                GlobalFunctions.print("not in the fortify state...", this);
            return false;
        }
        //make sure there are enough troops on _from and that _from and _to both belong to the same team...
        if (_from.my_Team != _Team)
        {
            if (debugging)
                GlobalFunctions.print("unable to fortify as _from doesnt belong to this Team...", this);
            return false;
        }
        if (_to.my_Team != _Team)
        {
            if (debugging)
                GlobalFunctions.print("unable to fortify as _to doesnt belong to this Team...", this);
            return false;
        }
        //TODO - make sure there is a "path" _from -> _to....
        //make sure there will be atleast RiskySandBox_Tile.min_troops_per_Tile troops left on _from...



        RiskySandBox_MainGame.instance.SET_num_troops(_from.ID, _from.num_troops - _n_troops);//reduce troops from _from
        RiskySandBox_MainGame.instance.SET_num_troops(_to.ID, _to.num_troops + _n_troops);//give troops to _to


        invokeEvent_Onfortify(_Team, _from, _to, _n_troops, _alert_MultiplayerBridge: true);

        return true;
    }
}
