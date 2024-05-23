using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_MainGame
{

    public ObservableBool enable_alliances { get { return PRIVATE_enable_alliances; } }

    [SerializeField] ObservableBool PRIVATE_enable_alliances;



    /// <summary>
    /// when true... if a team "wins" the allies of the team also win!
    /// </summary>
    public ObservableBool shared_victory { get { return this.PRIVATE_shared_victory; } }

    [SerializeField] ObservableBool PRIVATE_shared_victory;

    /// <summary>
    /// when true a Team can deploy to their ally(s) Tile(s)
    /// </summary>
    public ObservableBool allow_deploy_to_ally_Tiles { get { return this.PRIVATE_allow_deploy_to_ally_Tiles; } }

    [SerializeField] ObservableBool PRIVATE_allow_deploy_to_ally_Tiles;


    /// <summary>
    /// when true a Team can attack their ally(s) Tile(s)
    /// </summary>
    public ObservableBool allow_attack_ally_Tiles { get { return this.PRIVATE_allow_attack_ally_Tiles; } }

    [SerializeField] ObservableBool PRIVATE_allow_attack_ally_Tiles;

    /// <summary>
    /// when true a Team can foritfy through their ally(s) Tile(s)
    /// </summary>
    public ObservableBool allow_fortify_through_ally_Tiles { get { return this.PRIVATE_fortify_through_ally_Tiles; } }

    [SerializeField] ObservableBool PRIVATE_fortify_through_ally_Tiles;



    /// <summary>
    /// when true a Teams will "share FOW" - not implemented yet
    /// </summary>
    public ObservableBool share_FOW { get { return this.PRIVATE_share_FOW; } }

    [SerializeField] ObservableBool PRIVATE_share_FOW;



}
