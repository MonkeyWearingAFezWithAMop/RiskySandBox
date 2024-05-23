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

    public static readonly string turn_state_deploy = "deploy";
    public static readonly string turn_state_attack = "attack";
    public static readonly string turn_state_fortify = "fortify";
    public static readonly string turn_state_capture = "capture";
    public static readonly string turn_state_waiting = "waiting";
    public static readonly string turn_state_placing_capital = "capital placement";
    public static readonly string turn_state_force_trade_in = "force trade in";
    //TODO - other extra turn states that other people want... maybe we dump this into another file?
    //this is more than enough for now!


   
    public List<int> ally_ids = new List<int>();


    /// <summary>
    /// invoked whenever a teams turn has ran out of time...
    /// </summary>
    public static event Action<RiskySandBox_Team> OnturnTimerReachedZero;


    

    public ObservableFloat end_turn_time_stamp { get { return this.PRIVATE_end_turn_time_stamp; } }
    

    public ObservableBool is_my_turn { get { return this.PRIVATE_is_my_turn; } }
    
    public GameObject my_UI { get { return PRIVATE_my_UI; } }


    [SerializeField] List<Material> Team_Materials = new List<Material>();
    [SerializeField] List<Material> Bonus_Materials = new List<Material>();



    public Material my_Material { get { return this.Team_Materials[this.ID.value]; } }

    public Material my_Bonus_Material { get { return this.Bonus_Materials[this.ID.value]; } }

    public Color my_Color { get { return Team_Materials.Select(x => x.color).ToList()[this.ID.value]; } }


    public Color complementary_Color
    {
        get
        {
            float r = 1f - my_Color.r;
            float g = 1f - my_Color.g;
            float b = 1f - my_Color.b;

            return new Color(r, g, b);
        }
    }


    public int num_cards { get { return 0; } }//TODO - nope return the actual number of cards....


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

    /// <summary>
    /// the team who killed this Team (took the last terrotory...
    /// </summary>
    public ObservableInt killer_ID { get { return this.PRIVATE_killer_ID; } }
    



    private void Awake()
    {
        all_instances.Add(this);
        RiskySandBox_MainGame.OnSET_my_Team += RiskySandBox_TileEventReceiver_OnSET_my_Team_STATIC;
        this.defeated.OnUpdate += EventReceiver_OnVariableUpdate_defeated;
        this.ID.OnUpdate += delegate { OnVariableUpdate_ID_STATIC?.Invoke(this); };

        //tell everyone my killer id has just changed! - this is important for assassin mode & to transfer the terrotory cards!
        this.killer_ID.OnUpdate += delegate { RiskySandBox_Team.OnVariableUpdate_killer_ID_STATIC?.Invoke(this); };
        
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



    public void EventReceiver_OnVariableUpdate_defeated(ObservableBool _defeated)
    {
        //if defreated is now true?
        //fantastic! - lets tell everyone that a team is now defeated...
        if(defeated.value == true)
            RiskySandBox_Team.OnVariableUpdate_defeated_STATIC?.Invoke(this);
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


}
