using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_Team
{
    public static int null_ID = -1;
    public static ObservableList<RiskySandBox_Team> all_instances = new ObservableList<RiskySandBox_Team>();
    public static List<RiskySandBox_Team> undefeated_Teams { get { return all_instances.Where(x => x.defeated == false).ToList(); } }

    public static event Action<RiskySandBox_Team> OnVariableUpdate_defeated_STATIC;


    /// <summary>
    /// should be invoked whenever a Teams "ID" changes...
    /// </summary>
    public static event Action<RiskySandBox_Team> OnVariableUpdate_ID_STATIC;

    public static event Action<RiskySandBox_Team> OnVariableUpdate_killer_ID_STATIC;

    public static event Action<RiskySandBox_Team> OnVariableUpdate_assassin_target_ID_STATIC;

    public static event Action<RiskySandBox_Team> OnVariableUpdate_is_my_turn_STATIC;

    public static event Action<RiskySandBox_Team> OnUpdate_territory_card_IDs_STATIC;

    public static event Action<RiskySandBox_Team> OnUpdate_ally_ids_STATIC;


    /// <summary>
    /// called whenever a RiskySandBox_Team.capture_end_ID value changes...
    /// </summary>
    public static event Action<RiskySandBox_Team> OnVariableUpdate_capture_end_ID_STATIC;

    public static readonly string turn_state_deploy = "deploy";
    public static readonly string turn_state_attack = "attack";
    public static readonly string turn_state_fortify = "fortify";
    public static readonly string turn_state_capture = "capture";
    public static readonly string turn_state_waiting = "waiting";
    public static readonly string turn_state_placing_capital = "capital placement";
    public static readonly string turn_state_force_trade_in = "force trade in";
    //TODO - other extra turn states that other people want... maybe we dump this into another file?
    //this is more than enough for now!


   
    public ObservableList<int> ally_ids = new ObservableList<int>();


    /// <summary>
    /// invoked whenever a teams turn has ran out of time...
    /// </summary>
    public static event Action<RiskySandBox_Team> OnturnTimerReachedZero;


    

    public ObservableFloat end_turn_time_stamp { get { return this.PRIVATE_end_turn_time_stamp; } }
    

    public ObservableBool is_my_turn { get { return this.PRIVATE_is_my_turn; } }
    
    public GameObject my_UI { get { return PRIVATE_my_UI; } }


    [SerializeField] List<Material> Team_Materials = new List<Material>();
    [SerializeField] List<Material> Bonus_Materials = new List<Material>();
    [SerializeField] List<Color> text_Colors = new List<Color>();




    public Material bonus_border_Material { get { return this.my_Material; } }
    public Material my_Material { get { return this.Team_Materials[this.ID.value]; } }

    public Material my_Bonus_Material { get { return this.Bonus_Materials[this.ID.value]; } }

    public Color my_Color { get { return Team_Materials.Select(x => x.color).ToList()[this.ID.value]; } }

    public Color text_Color { get { return text_Colors[this.ID.value]; } }



    /// <summary>
    /// how many territory cards does the team have... (this.territory_card_IDs.Count()...)
    /// </summary>
    public int num_cards { get { return this.territory_card_IDs.Count(); } }


    public ObservableInt ID {get { return PRIVATE_ID; }}
    public ObservableBool defeated { get { return PRIVATE_defeated; } }

    public RiskySandBox_Tile capture_start { get { return RiskySandBox_Tile.GET_RiskySandBox_Tile(capture_start_ID); } }
    public RiskySandBox_Tile capture_target { get { return RiskySandBox_Tile.GET_RiskySandBox_Tile(capture_end_ID); } }

    public ObservableInt capture_start_ID { get { return this.PRIVATE_capture_start_ID; } }
    public ObservableInt capture_end_ID { get { return this.PRIVATE_capture_end_ID; } }

    public ObservableInt n_Tiles { get { return PRIVATE_n_Tiles; } }
    public ObservableInt n_capitals { get { return PRIVATE_n_capitals; } }
    
    public ObservableInt required_capital_placements { get { return this.PRIVATE_required_capital_placements; } }


    public ObservableInt assassin_target_ID { get { return this.PRIVATE_assassin_target_ID; } }
    public RiskySandBox_Team assassin_target
    {
        get
        {
            if (this.assassin_target_ID.value == null_ID)
                return null;
            return RiskySandBox_Team.GET_RiskySandBox_Team(assassin_target_ID);
        }
    }

    public ObservableBool show_assassin_target_indicator { get { return this.PRIVATE_show_assassin_target_indicator; } }

    /// <summary>
    /// the team who killed this Team (took the last terrotory...
    /// </summary>
    public ObservableInt killer_ID { get { return this.PRIVATE_killer_ID; } }


    public List<RiskySandBox_Tile> my_Tiles { get { return RiskySandBox_Tile.all_instances.Where(x => x.my_Team_ID.value == this.ID.value).ToList(); } }

    public ObservableList<int> territory_card_IDs = new ObservableList<int>();

    /// <summary>
    /// has the team "caputred" a Tile on the turn (required for giving "territory cards")
    /// </summary>
    public ObservableBool has_captured_Tile { get { return this.PRIVATE_has_captured_Tile; } }


    


    private void Awake()
    {
        all_instances.Add(this);
        RiskySandBox_MainGame.OnSET_my_Team += RiskySandBox_TileEventReceiver_OnSET_my_Team_STATIC;

        this.defeated.OnUpdate += delegate { RiskySandBox_Team.OnVariableUpdate_defeated_STATIC?.Invoke(this); };
        this.ID.OnUpdate += delegate { RiskySandBox_Team.OnVariableUpdate_ID_STATIC?.Invoke(this); };
        this.ID.OnUpdate += delegate { this.gameObject.name = "RiskySandBox_Team with id = " + this.ID; };
        this.capture_end_ID.OnUpdate += delegate { RiskySandBox_Team.OnVariableUpdate_capture_end_ID_STATIC?.Invoke(this); };
        this.assassin_target_ID.OnUpdate += delegate { RiskySandBox_Team.OnVariableUpdate_assassin_target_ID_STATIC?.Invoke(this); };
        this.is_my_turn.OnUpdate += delegate { RiskySandBox_Team.OnVariableUpdate_is_my_turn_STATIC?.Invoke(this); };
        this.killer_ID.OnUpdate += delegate { RiskySandBox_Team.OnVariableUpdate_killer_ID_STATIC?.Invoke(this); };
        this.territory_card_IDs.OnUpdate += delegate { RiskySandBox_Team.OnUpdate_territory_card_IDs_STATIC?.Invoke(this); };
        this.ally_ids.OnUpdate += delegate { RiskySandBox_Team.OnUpdate_ally_ids_STATIC?.Invoke(this); };

        
        
    }

    private void OnDestroy()
    {
        all_instances.Remove(this);
        RiskySandBox_MainGame.OnSET_my_Team -= RiskySandBox_TileEventReceiver_OnSET_my_Team_STATIC;
    }

    public static RiskySandBox_Team GET_RiskySandBox_Team(int _ID)
    {
        foreach(RiskySandBox_Team _Team in RiskySandBox_Team.all_instances)
        {
            if (_Team.ID == _ID)
                return _Team;
        }
        return null;
    }


    private void Update()
    {
        if (this.PRIVATE_is_my_turn.value == false)
            return;

        float _remaining_turn_length = end_turn_time_stamp - RiskySandBox.instance.current_time;

        if(_remaining_turn_length <= 0 && PrototypingAssets.run_server_code.value == true)
        {
            RiskySandBox_Team.OnturnTimerReachedZero?.Invoke(this);
        }


    }

    void RiskySandBox_TileEventReceiver_OnSET_my_Team_STATIC(RiskySandBox_Tile _Tile)
    {
        if (_Tile.previous_my_Team == this)
        {
            this.n_Tiles.value -= 1;

            if (this.n_Tiles <= 0)
            {
                //i have been defeated... as i have no tiles left...
                this.defeated.value = true;
            }

        }


        if (_Tile.my_Team == this)
            this.n_Tiles.value += 1;
    }


    public void addTerritoryCard(int _ID)
    {
        Debug.LogWarning("WARNING - need to fire a multiplayer event!");
        this.territory_card_IDs.Add(_ID);
        //add the terrotiry card to this teams list!
        //tell the multiplayer messages that this has occured...
    }

    public void removeTerritoryCard(int _ID)
    {
        Debug.LogWarning("WARNING - need to fire a multiplayer event!");
        this.territory_card_IDs.Remove(_ID);
        //add the terrotiry card to this teams list!
        //tell the multipalyer messages that this has occured....
    }


    public void createAlliance(RiskySandBox_Team _other)
    {
        Debug.LogWarning("WARNING - need to fire a multiplayer event!");

        if (this.ally_ids.Contains(_other.ID) == false)
            this.ally_ids.Add(_other.ID);

        //tell everyone my allies have changed?

    }

    public void breakAlliance(RiskySandBox_Team _other)
    {
        Debug.LogWarning("need to fire a multiplayer event!");
        if(this.ally_ids.Contains(_other.ID) == true)
            this.ally_ids.Remove(_other.ID);

        //tell everyone my allies have changed?

    }




}
