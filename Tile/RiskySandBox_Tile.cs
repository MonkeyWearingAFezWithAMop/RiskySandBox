using System.Collections;using System.Collections.Generic;using System.Linq;using System;


public partial class RiskySandBox_Tile
{

    public static string save_load_key_num_troops { get { return "num_troops"; } }
    public static string save_load_key_ID { get { return "ID"; } }



    static Dictionary<int, RiskySandBox_Tile> CACHE_GET_RiskySandBox_Tile = new Dictionary<int, RiskySandBox_Tile>();

    public static List<RiskySandBox_Tile> all_instances = new List<RiskySandBox_Tile>();

    /// <summary>
    /// all the tiles that have a "stable portal" (needed for graph_connections...)
    /// </summary>
    public static List<RiskySandBox_Tile> all_instances_with_stable_portal {get { return RiskySandBox_Tile.all_instances.Where(x => x != null && (x.PRIVATE_has_stable_portal.value == true)).ToList(); }}

    /// <summary>
    /// all the tiles that have a "unstable portal" (needed for graph_connections...)
    /// </summary>
    public static List<RiskySandBox_Tile> all_instances_with_unstable_portal { get { return RiskySandBox_Tile.all_instances.Where(x => x != null && (x.PRIVATE_has_unstable_portal == true)).ToList(); } }



    public static readonly int null_ID = -1;
    public static readonly int min_troops_per_Tile = 1;


    new public ObservableString name { get { return PRIVATE_name; } }

    public List<RiskySandBox_Tile> graph_connections
    {
        get
        {
            HashSet<RiskySandBox_Tile> _return_value = new HashSet<RiskySandBox_Tile>();
            //foreach id...
            //add it...
            //foreach "stable portal" - add all stable portals...

            foreach (int _id in this.graph_connections_IDs)
            {
                //add it
                RiskySandBox_Tile _Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(_id);
                if (_Tile != null)
                    _return_value.Add(_Tile);
            }

            if (this.PRIVATE_has_stable_portal.value == true)
            {
                //get all the tiles that have a stable portal...
                foreach (RiskySandBox_Tile _Tile in all_instances_with_stable_portal)
                {
                    _return_value.Add(_Tile);
                }
            }

            if (this.PRIVATE_has_unstable_portal == true)
            {
                if(RiskySandBox_UnstablePortals.are_active == true)
                {
                    foreach(RiskySandBox_Tile _Tile in all_instances_with_unstable_portal)
                    {
                        _return_value.Add(_Tile);
                    }
                }
            }

            foreach(RiskySandBox_Tile _Tile in _return_value)
            {
                if(_Tile.has_blizard == true)
                {
                    //remove...
                    _return_value.Remove(_Tile);
                }
            }

            _return_value.Remove(this);
            return _return_value.ToList();
        }
    }

    public ObservableList<int> graph_connections_IDs = new ObservableList<int>();
   

    public RiskySandBox_Team my_Team { get { return RiskySandBox_Team.GET_RiskySandBox_Team(this.my_Team_ID.value); } }
    public RiskySandBox_Team previous_my_Team { get { return RiskySandBox_Team.GET_RiskySandBox_Team(this.my_Team_ID.previous_value); } }


    public ObservableInt ID { get { return this.PRIVATE_ID; } }

    /// <summary>
    /// is the tile selected (by the current player)
    /// </summary>
    public ObservableBool is_deploy_target { get { return PRIVATE_is_deploy_target; } }

    public ObservableBool is_attack_start { get { return PRIVATE_is_attack_start; } }
    public ObservableBool is_attack_target { get { return PRIVATE_is_attack_target; } }

    public ObservableBool is_fortify_start { get { return PRIVATE_is_fortify_start; } }
    public ObservableBool is_fortify_target { get { return PRIVATE_is_fortify_target; } }

    public ObservableInt num_troops { get { return PRIVATE_num_troops; } }
    public ObservableBool show_level_editor_ui { get { return PRIVATE_show_level_editor_ui; } }
    public ObservableInt my_Team_ID { get { return PRIVATE_my_Team_ID; } }
    /// <summary>
    /// is there a "capital" on this tile???
    /// </summary>
    public ObservableBool has_capital { get { return this.PRIVATE_has_capital; } }

    public ObservableVector3 capital_icon_local_position { get { return this.PRIVATE_capital_icon_local_position; } }
    public ObservableFloat capital_icon_scale_factor { get { return this.PRIVATE_capital_icon_local_scale_factor; } }

    public ObservableBool has_stable_portal { get { return this.PRIVATE_has_stable_portal; } }
    public ObservableBool has_blizard { get { return this.PRIVATE_has_blizard; } }

    public ObservableBool has_unstable_portal { get { return this.PRIVATE_has_unstable_portal; } }


    //TODO - we may have some settings like if a tile has a capital it can't take attrition... put in all the conditions here...
    public bool immune_to_attrition { get { return false; } }




    public static RiskySandBox_Tile GET_RiskySandBox_Tile(int _ID)
    {
        //TODO - this is a big one to try and fix using like a dictionary or some form of faster lookup method... this is already set up... but just to try and stop any stupid bugs creeping in while developing...
        foreach(RiskySandBox_Tile _Tile in all_instances)
        {
            if (_Tile.ID.value == _ID)
                return _Tile;
        }
        return null;
        RiskySandBox_Tile _return_value = null;
        RiskySandBox_Tile.CACHE_GET_RiskySandBox_Tile.TryGetValue(_ID, out _return_value);
        return _return_value;
    }




    public static RiskySandBox_Tile loadTile(string[] _lines)
    {
        List<UnityEngine.Vector3> _verts = new List<UnityEngine.Vector3>();

        Dictionary<string, string> _data = new Dictionary<string, string>();


        foreach(string _line in _lines)
        {
            _data[_line.Split(":")[0]] = _line.Split(":")[1];
        }

        if (_data.ContainsKey("verts") == true)
        {
            float[] _vert_values = _data["verts"].Split(",").Select(x => float.Parse(x)).ToArray();
            for (int i = 0; i < _vert_values.Count() / 3; i += 1)
            {
                _verts.Add(new UnityEngine.Vector3(_vert_values[i * 3], _vert_values[i * 3 + 1], _vert_values[i * 3 + 2]));
            }
        }



        UnityEngine.Vector3 _Tile_position = new UnityEngine.Vector3(0, 0, 0);

        if (_data.ContainsKey("position"))
        {
            List<float> _position_values = _data["position"].Split(",").Select(x => float.Parse(x)).ToList();//  position:x,y,z  is how this should look internally...
            _Tile_position = new UnityEngine.Vector3(_position_values[0], _position_values[1], _position_values[2]);
        }


        RiskySandBox_Tile _Tile = RiskySandBox_Tile.createTile(int.Parse(_data["ID"]), _verts);

        if (_data.ContainsKey("UI_scale_factor"))
        {
            _Tile.UI_scale_factor.value = float.Parse(_data["UI_scale_factor"]);
        }


        if (_data.ContainsKey("UI_position"))
        {
            List<float> _float_values = _data["UI_position"].Split(",").Select(x => float.Parse(x)).ToList();
            _Tile.UI_position.value = new UnityEngine.Vector3(_float_values[0], _float_values[1], _float_values[2]);
        }

        if(_data.ContainsKey("capital_icon_local_position"))
        {
            List<float> _float_values = _data["capital_icon_local_position"].Split(",").Select(x => float.Parse(x)).ToList();
            _Tile.capital_icon_local_position.value = new UnityEngine.Vector3(_float_values[0], _float_values[1], _float_values[2]);
        }

        if (_data.ContainsKey("capital_icon_scale_factor") == true)
        {
            _Tile.capital_icon_scale_factor.value = float.Parse(_data["capital_icon_scale_factor"]);
        }


        return _Tile;
    }

}
