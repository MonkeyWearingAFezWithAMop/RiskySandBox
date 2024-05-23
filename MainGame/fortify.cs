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

    //risksandbox_data/streamingassets/maps


    public static List<int> CalculateRoute(int start, int end,RiskySandBox_Team _Team)
    {
        
        return CalculateRoute(start, end, new HashSet<int>(), new List<int>(),_Team);
    }

    private static List<int> CalculateRoute(int current, int end, HashSet<int> visited, List<int> path,RiskySandBox_Team _Team)
    {
        visited.Add(current);
        path.Add(current);

        if (current == end)
        {
            return new List<int>(path); // Return a copy of the path
        }

        foreach (int neighbor in RiskySandBox_Tile.GET_RiskySandBox_Tile(current).graph_connections)
        {
            if (visited.Contains(neighbor))
                continue;

            RiskySandBox_Tile neighbor_Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(neighbor);

            //TODO - if this neighbour is null? - there is a problem with the graph... we should correct this?
            //TODO - what happens if the tile is "blocked" or inactive in some way... maybe someone has temporarly stopped the connection?

            if (neighbor_Tile.my_Team != _Team)
            {
                if (RiskySandBox_MainGame.instance.enable_alliances == false)
                    continue;
                //check if the neighbour is an ally of _Team

                bool _is_ally = _Team.ally_ids.Contains(neighbor_Tile.my_Team_ID);
                if (_is_ally == false || RiskySandBox_MainGame.instance.allow_fortify_through_ally_Tiles.value == false)
                    continue;
                
            }
                

            List<int> newPath = CalculateRoute(neighbor, end, visited, path,_Team);
            if (newPath != null)
            {
                return newPath;
            }
            
        }

        path.RemoveAt(path.Count - 1); // Backtrack
        return null;
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

        if(_from.num_troops <= _n_troops)//TODO - need to also think about RiskySandBox_Tile.min_troops_per_tile...
        {
            if (debugging)
                GlobalFunctions.print("unable to fortify as _from.num_troops <= _n_troops",this);
            return false;
        }



        List<int> _path = CalculateRoute(_from.ID, _to.ID,_Team);

        if(_path == null)
        {
            if (debugging)
                GlobalFunctions.print("unable to find a route from " + _from + " _to " + _to+" returning",this);
            return false;
        }


        print("path: " + string.Join(",", _path));



        RiskySandBox_MainGame.instance.SET_num_troops(_from.ID, _from.num_troops - _n_troops);//reduce troops from _from
        RiskySandBox_MainGame.instance.SET_num_troops(_to.ID, _to.num_troops + _n_troops);//give troops to _to


        invokeEvent_Onfortify(_Team, _from, _to, _n_troops, _alert_MultiplayerBridge: true);

        return true;
    }
}
