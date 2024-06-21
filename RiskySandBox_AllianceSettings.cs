using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_AllianceSettings : MonoBehaviour
{

    public static RiskySandBox_AllianceSettings instance;

    [SerializeField] bool debugging;


    public static ObservableBool enable_alliances { get { return instance.PRIVATE_enable_alliances; } }

    [SerializeField] ObservableBool PRIVATE_enable_alliances;



    /// <summary>
    /// when true... if a team "wins" the alliy(s) of the team also win!
    /// </summary>
    public static ObservableBool shared_victory { get { return instance.PRIVATE_shared_victory; } }

    [SerializeField] ObservableBool PRIVATE_shared_victory;

    /// <summary>
    /// when true a Team can deploy to their ally(s) Tile(s)
    /// </summary>
    public static ObservableBool allow_deploy_to_ally_Tiles { get { return instance.PRIVATE_allow_deploy_to_ally_Tiles; } }

    [SerializeField] ObservableBool PRIVATE_allow_deploy_to_ally_Tiles;


    /// <summary>
    /// when true a Team can attack their ally(s) Tile(s)
    /// </summary>
    public static ObservableBool allow_attack_ally_Tiles { get { return instance.PRIVATE_allow_attack_ally_Tiles; } }

    [SerializeField] ObservableBool PRIVATE_allow_attack_ally_Tiles;

    /// <summary>
    /// when true a Team can move troops through their ally(s) Tile(s) e.g. when fortifying
    /// </summary>
    public static ObservableBool allow_move_through_ally_Tiles { get { return instance.PRIVATE_allow_move_through_ally_Tiles; } }

    [SerializeField] ObservableBool PRIVATE_allow_move_through_ally_Tiles;

    /// <summary>
    /// when true... a Team can fortify troops onto ally(s) Tile(s)
    /// </summary>
    public static ObservableBool allow_fortify_to_ally_Tiles { get { return instance.PRIVATE_allow_fortify_to_ally_Tiles; } }
    [SerializeField] ObservableBool PRIVATE_allow_fortify_to_ally_Tiles;


    /// <summary>
    /// when true a Teams will "share FOW" - not implemented yet
    /// </summary>
    public static ObservableBool share_FOW { get { return instance.PRIVATE_share_FOW; } }

    [SerializeField] ObservableBool PRIVATE_share_FOW;


    //e.g. 1.2,3.4,5.6.7.8 would mean it is teams:    (1 and 2) vs (3 and 4) vs (5 and 6 and 7 and 8)
    public static ObservableString alliance_string { get { return instance.PRIVATE_alliance_string; } }
    [SerializeField] ObservableString PRIVATE_alliance_string;


    //todo create other allinace options e.g. 2v2 mode


    private void Awake()
    {
        if (debugging)
            GlobalFunctions.print("", this);
        instance = this;
    }


}
