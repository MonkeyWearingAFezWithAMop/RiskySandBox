using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_HumanPlayer_AttackState : MonoBehaviour
{
    [SerializeField] bool debugging;

    [SerializeField] RiskySandBox_HumanPlayer my_HumanPlayer;

    RiskySandBox_Tile attack_start { get { return my_HumanPlayer.attack_start; } set { my_HumanPlayer.attack_target = value; } }
   
    [SerializeField] ObservableInt attack_value;

    RiskySandBox_Team my_Team { get { return my_HumanPlayer.my_Team; } }

    [SerializeField] List<GameObject> attack_input_elements = new List<GameObject>();


    [SerializeField] GameObject UI_canvas;


    private void Awake()
    {
        RiskySandBox_MainGame.Oncapture += EventReceiver_Oncapture;
        RiskySandBox_MainGame.Onattack += EventReceiver_Onattack;

        RiskySandBox_Team.OnVariableUpdate_current_turn_state_STATIC += EventReceiver_OnVariableUpdate_current_turn_state_STATIC;

        RiskySandBox_HumanPlayer.OnVariableUpdate_attack_target_STATIC += EventReceiver_OnVariableUpdate_attack_target_STATIC;
        RiskySandBox_HumanPlayer.OnleftClick += handleLeftClick;
        RiskySandBox_HumanPlayer.OnspaceKey += EventReceiver_OnspaceKey;

    }

    private void OnDestroy()
    {
        RiskySandBox_MainGame.Oncapture -= EventReceiver_Oncapture;
        RiskySandBox_MainGame.Onattack -= EventReceiver_Onattack;

        RiskySandBox_Team.OnVariableUpdate_current_turn_state_STATIC -= EventReceiver_OnVariableUpdate_current_turn_state_STATIC;

        RiskySandBox_HumanPlayer.OnleftClick -= handleLeftClick;
        RiskySandBox_HumanPlayer.OnspaceKey -= EventReceiver_OnspaceKey;

        
    }

    void updateTileUIs()
    {
        ObservableInt.resetAllUIs();//TODO - nope! - we only want to override the Tile.num_troops uis...
        
    }

    private void Start()
    {
        this.UI_canvas.SetActive(this.my_Team != null && this.my_Team.current_turn_state == RiskySandBox_Team.turn_state_attack);
        foreach (GameObject _GameObject in this.attack_input_elements.Where(x => x != null))
        {
            _GameObject.SetActive(this.my_HumanPlayer.attack_target != null);
        }
    }


    void EventReceiver_OnVariableUpdate_attack_target_STATIC(RiskySandBox_HumanPlayer _HumanPlayer)
    {
        if (_HumanPlayer != this.my_HumanPlayer)
            return;

        if (_HumanPlayer.attack_target != null)
        {
            this.attack_value.min_value = 1;
            this.attack_value.max_value = this.my_HumanPlayer.attack_start.num_troops - RiskySandBox_Tile.min_troops_per_Tile;

            //TODO - default to 100% chance to take??? - WTF?!?!?! does this even mean someone needs to explain this...
            this.attack_value.value = this.my_HumanPlayer.attack_start.num_troops - RiskySandBox_Tile.min_troops_per_Tile;

            //enable the confirm, cancel and attack slider...
        }

        foreach(GameObject _GameObject in this.attack_input_elements.Where(x => x != null))
        {
            _GameObject.SetActive(_HumanPlayer.attack_target != null);
        }

    }



    /// <summary>
    /// code that runs when player left clicks in the "attack" state...
    /// </summary>
    void handleLeftClick(RiskySandBox_HumanPlayer _HumanPlayer,string _current_turn_state)
    {
        if (_HumanPlayer != my_HumanPlayer)
            return;

        if (_current_turn_state != RiskySandBox_Team.turn_state_attack)
            return;


        RiskySandBox_Tile _current_Tile = RiskySandBox_CameraControls.current_hovering_Tile;
        if (_current_Tile == null)
            return;

        if (_current_Tile.my_Team == this.my_Team)
        {
            if (this.my_HumanPlayer.attack_target == null)
                this.my_HumanPlayer.attack_start = _current_Tile;
        }
        else
        {
            if (this.my_HumanPlayer.attack_start != null)
            {
                if (this.my_HumanPlayer.attack_target == null)
                    this.my_HumanPlayer.attack_target = _current_Tile;

            }
        }
    }

    void EventReceiver_OnspaceKey(RiskySandBox_HumanPlayer _HumanPlayer,string _current_turn_state)
    {
        if (_HumanPlayer != this.my_HumanPlayer)
            return;

        if (_current_turn_state != RiskySandBox_Team.turn_state_attack)
            return;

        if (this.my_HumanPlayer.attack_target != null)
            my_HumanPlayer.TRY_attack(this.my_HumanPlayer.attack_start,this.my_HumanPlayer.attack_target,this.attack_value);

        else
        {
            my_HumanPlayer.TRY_attack(this.my_HumanPlayer.attack_start,RiskySandBox_CameraControls.current_hovering_Tile,this.attack_value);
        }
    }

    void EventReceiver_OnVariableUpdate_current_turn_state_STATIC(RiskySandBox_Team _Team)
    {
        if (_Team != this.my_Team)
            return;

        this.UI_canvas.SetActive(_Team.current_turn_state == RiskySandBox_Team.turn_state_attack);
        this.my_HumanPlayer.attack_start = null;
        this.my_HumanPlayer.attack_target = null;
    }



    void EventReceiver_Onattack(RiskySandBox_MainGame.EventInfo_Onattack _EventInfo)
    {
        if (_EventInfo.attacking_Team != this.my_Team)
            return;


        if (_EventInfo.capture_flag == false)//if we didnt capture....
        {
            if (_EventInfo.start_Tile.num_troops <= RiskySandBox_Tile.min_troops_per_Tile)//if the tile doesn't have enough troops to allow attacking to continue...
            {
                this.my_HumanPlayer.attack_start = null;
                this.my_HumanPlayer.attack_target = null;
                updateTileUIs();
            }
        }
    }
    void EventReceiver_Oncapture(RiskySandBox_MainGame.EventInfo_Oncapture _EventInfo)
    {
        if (_EventInfo.Team != this.my_Team)
            return;


        //TODO - make this a setting that the player can enable something like "enable_smart_capture"

        //try to help the player select the tile they are likely to want....
        if (my_Team.capture_target.num_troops > RiskySandBox_Tile.min_troops_per_Tile)//if they placed more than <1> troop on the capture target?
        {
            //well what happens if there is noone to attack from there?
            this.my_HumanPlayer.attack_start = my_Team.capture_target;
        }
        else if (my_Team.capture_start.num_troops > RiskySandBox_Tile.min_troops_per_Tile)
        {
            //TODO - what happens if there are no attackable targets? - maybe we just go to null?
            this.my_HumanPlayer.attack_start = my_Team.capture_start;
        }
        else
        {
            this.my_HumanPlayer.attack_start = null;
        }
        this.my_HumanPlayer.attack_target = null;
            
        
    }

    public void EventReceiver_OnconfirmFromUI()
    {
        this.my_HumanPlayer.TRY_attack(this.my_HumanPlayer.attack_start, this.my_HumanPlayer.attack_target,this.attack_value);

    }
}
