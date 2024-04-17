using System.Collections;using System.Collections.Generic;using System.Linq;using System;


public partial class RiskySandBox_Tile
{ 
    static Dictionary<int, RiskySandBox_Tile> CACHE_GET_RiskySandBox_Tile = new Dictionary<int, RiskySandBox_Tile>();

    public static List<RiskySandBox_Tile> all_instances { get { return RiskySandBox_Tile.CACHE_GET_RiskySandBox_Tile.Values.ToList(); } }



    public static readonly int null_ID = -1;
    public static readonly int min_troops_per_Tile = 1;


    public int ID
    {
        get { return PRIVATE_ID; }
        set
        {
            if (this.ID != null_ID)
            {
                GlobalFunctions.printError("the ID for this Tile has already been set!", this);
                return;
            }
            //TODO - make sure there isnt already a Tile with this ID...
            this.PRIVATE_ID = value;
            RiskySandBox_Tile.CACHE_GET_RiskySandBox_Tile[value] = this;
        }
    }

    public RiskySandBox_Team my_Team;
    public RiskySandBox_Team previous_my_Team;



    /// <summary>
    /// is the tile selected (by the current player)
    /// </summary>
    public ObservableBool selected { get { return PRIVATE_selected; } }
    public ObservableBool is_attack_target { get { return PRIVATE_is_attack_target; } }
    public ObservableBool is_fortify_target { get { return PRIVATE_is_fortify_target; } }
    public ObservableInt num_troops { get { return PRIVATE_num_troops; } }



    



    public static RiskySandBox_Tile GET_RiskySandBox_Tile(int _ID)
    {
        RiskySandBox_Tile _return_value = null;
        RiskySandBox_Tile.CACHE_GET_RiskySandBox_Tile.TryGetValue(_ID, out _return_value);
        return _return_value;
    }





}
