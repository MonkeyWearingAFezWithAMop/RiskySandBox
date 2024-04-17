using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_Team : MonoBehaviour
{
    public static int null_ID = -1;
    public static ObservableList<RiskySandBox_Team> all_instances = new ObservableList<RiskySandBox_Team>();
    public static List<RiskySandBox_Team> undefeated_Teams { get { return all_instances.Where(x => x.defeated == false).ToList(); } }

    public static event Action<RiskySandBox_Team> OnVariableUpdate_defeated_STATIC;

    public static readonly string turn_state_deploy = "deploy";
    public static readonly string turn_state_attack = "attack";
    public static readonly string turn_state_fortify = "fortify";
    public static readonly string turn_state_capture = "capture";
    public static readonly string turn_state_waiting = "waiting";






    [SerializeField] bool debugging;

    public GameObject my_UI { get { return PRIVATE_my_UI; } }
    [SerializeField] GameObject PRIVATE_my_UI;



    public Material my_Material { get { return PrototypingAssets.Team_Materials[this.ID.value]; } }
    public Color my_Color { get { return PrototypingAssets.Team_Colors[this.ID.value]; } }


    public UnityEngine.UI.Image UI_background_Image;

    
    public ObservableInt ID
    {
        get { return PRIVATE_ID; }
    }
    public ObservableBool defeated { get { return PRIVATE_defeated; } }


    public RiskySandBox_Tile capture_start { get { return RiskySandBox_Tile.GET_RiskySandBox_Tile(capture_start_ID); } }
    public RiskySandBox_Tile capture_target { get { return RiskySandBox_Tile.GET_RiskySandBox_Tile(capture_end_ID); } }

    public ObservableInt capture_start_ID { get { return this.PRIVATE_capture_start_ID; } }
    public ObservableInt capture_end_ID { get { return this.PRIVATE_capture_end_ID; } }

    [SerializeField] ObservableInt PRIVATE_capture_start_ID;
    [SerializeField] ObservableInt PRIVATE_capture_end_ID;


    [SerializeField] private ObservableInt PRIVATE_ID;
    [SerializeField] private ObservableBool PRIVATE_defeated;

    public ObservableInt n_Tiles { get { return PRIVATE_n_Tiles; } }

    [SerializeField] private ObservableInt PRIVATE_n_Tiles;



    private void Awake()
    {
        all_instances.Add(this);
        RiskySandBox_MainGame.OnSET_my_Team += RiskySandBox_TileEventReceiver_OnSET_my_Team_STATIC;
        this.defeated.OnUpdate += EventReceiver_OnVariableUpdate_defeated;
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

    public void EventReceiver_OnVariableUpdate_ID(ObservableInt _ID)
    {
        UI_background_Image.color = my_Color;
    }

    public void EventReceiver_OnVariableUpdate_defeated(ObservableBool _defeated)
    {
        //if defreated is now true?
        //fantastic! - lets tell everyone that a team is now defeated...
        if(defeated.value == true)
            RiskySandBox_Team.OnVariableUpdate_defeated_STATIC?.Invoke(this);
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
