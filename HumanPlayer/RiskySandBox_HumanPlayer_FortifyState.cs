using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_HumanPlayer_FortifyState : MonoBehaviour
{
    [SerializeField] bool debugging;

    [SerializeField] RiskySandBox_HumanPlayer my_HumanPlayer;
    [SerializeField] ObservableInt fortify_value;

    RiskySandBox_Tile fortify_start { get { return my_HumanPlayer.fortify_start; } set { this.my_HumanPlayer.fortify_start = value; } }
    RiskySandBox_Tile fortify_target { get { return my_HumanPlayer.fortify_target; } set { this.my_HumanPlayer.fortify_target = value; } }


    RiskySandBox_Team my_Team { get { return my_HumanPlayer.my_Team; } }

    [SerializeField] GameObject root_UI;

    [SerializeField] List<GameObject> fortify_input_elements = new List<GameObject>();




    private void Awake()
    {
        RiskySandBox_MainGame.Onfortify += EventReceiver_Onfortify;

        RiskySandBox_Team.OnVariableUpdate_current_turn_state_STATIC += EventReceiver_OnVariableUpdate_current_turn_state_STATIC;

        RiskySandBox_HumanPlayer.OnleftClick += EventReceiver_OnleftClick;
        RiskySandBox_HumanPlayer.OnspaceKey += EventReceiver_OnspaceKey;
        RiskySandBox_HumanPlayer.OnVariableUpdate_fortify_target_STATIC += EventReceiver_OnVariableUpdate_fortify_target;

        this.fortify_value.OnUpdate += delegate { updateTileUIs(); };
    }

    private void OnDestroy()
    {
        RiskySandBox_MainGame.Onfortify -= EventReceiver_Onfortify;

        RiskySandBox_Team.OnVariableUpdate_current_turn_state_STATIC -= EventReceiver_OnVariableUpdate_current_turn_state_STATIC;

        RiskySandBox_HumanPlayer.OnleftClick -= EventReceiver_OnleftClick;
        RiskySandBox_HumanPlayer.OnspaceKey -= EventReceiver_OnspaceKey;
        RiskySandBox_HumanPlayer.OnVariableUpdate_fortify_target_STATIC -= EventReceiver_OnVariableUpdate_fortify_target;


        this.fortify_value.OnUpdate += delegate { updateTileUIs(); };
    }

    private void Start()
    {
        root_UI.gameObject.SetActive(this.my_Team != null && this.my_Team.current_turn_state == RiskySandBox_Team.turn_state_fortify);
        foreach (GameObject _GameObject in this.fortify_input_elements.Where(x => x != null))
        {
            _GameObject.SetActive(this.my_HumanPlayer.fortify_target != null);
        }
    }

    void updateTileUIs()
    {
        if (my_Team.current_turn_state != RiskySandBox_Team.turn_state_fortify)
            return;

        ObservableInt.resetAllUIs();
        if(this.fortify_start != null && this.fortify_target != null)
        {
            this.fortify_start.num_troops.overrideUI(this.fortify_start.num_troops.value - this.fortify_value.value);
            this.my_HumanPlayer.fortify_target.num_troops.overrideUI(this.fortify_value.value + this.my_HumanPlayer.fortify_target.num_troops.value);
        }
    }



    void EventReceiver_OnVariableUpdate_fortify_target(RiskySandBox_HumanPlayer _HumanPlayer)
    {
        if (_HumanPlayer != this.my_HumanPlayer)
            return;


        if (this.my_HumanPlayer.fortify_target != null)
        {
            this.fortify_value.min_value = RiskySandBox_Tile.min_troops_per_Tile;
            this.fortify_value.max_value = this.fortify_start.num_troops - RiskySandBox_Tile.min_troops_per_Tile;
            this.fortify_value.value = this.fortify_start.num_troops - RiskySandBox_Tile.min_troops_per_Tile;
        }

        updateTileUIs();

        foreach(GameObject _GameObject in this.fortify_input_elements.Where(x => x != null))
        {
            _GameObject.SetActive(this.my_HumanPlayer.fortify_target != null);
        }


    }


    void EventReceiver_OnleftClick(RiskySandBox_HumanPlayer _HumanPlayer,string _current_turn_state)
    {
        if (_HumanPlayer != this.my_HumanPlayer)
            return;

        if (_current_turn_state != RiskySandBox_Team.turn_state_fortify)
            return;

        RiskySandBox_Tile _current_Tile = RiskySandBox_CameraControls.current_hovering_Tile;
        if (_current_Tile == null)
            return;

        if (this.fortify_start == null)
        {
            if (_current_Tile.my_Team == this.my_Team)
            {
                this.fortify_start = _current_Tile;
            }
        }
        else if(this.fortify_target == null && _current_Tile != this.fortify_start)
        {
            //note this may not go through since the human player will check if it is valid...
            this.my_HumanPlayer.fortify_target = _current_Tile;
        }
    }

    void EventReceiver_OnspaceKey(RiskySandBox_HumanPlayer _HumanPlayer,string _turn_state)
    {
        if (_HumanPlayer != this.my_HumanPlayer)
            return;

        if (_turn_state != RiskySandBox_Team.turn_state_fortify)
            return;

        this.my_HumanPlayer.TRY_fortify(this.fortify_start, this.my_HumanPlayer.fortify_target,this.fortify_value);
    }

    void EventReceiver_OnVariableUpdate_current_turn_state_STATIC(RiskySandBox_Team _Team)
    {
        if (_Team != this.my_Team)
            return;

        this.fortify_start = null;
        this.fortify_target = null;
        this.root_UI.gameObject.SetActive(_Team.current_turn_state == RiskySandBox_Team.turn_state_fortify);
    }



    void EventReceiver_Onfortify(RiskySandBox_MainGame.EventInfo_Onfortify _EventInfo)
    {
        if (_EventInfo.Team != my_Team)
            return;

        this.fortify_start = null;
        this.my_HumanPlayer.fortify_target = null;

    }

    

    //public as we must UiButtons must have public functions :(....
    public void EventReceiver_OnconfirmFromUI()
    {
        this.my_HumanPlayer.TRY_fortify(this.fortify_start, this.my_HumanPlayer.fortify_target, fortify_value);
    }

}
