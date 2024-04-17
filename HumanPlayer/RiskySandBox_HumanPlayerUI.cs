using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_HumanPlayerUI : MonoBehaviour
{
    [SerializeField] bool debugging;

    [SerializeField] RiskySandBox_HumanPlayer my_HumanPlayer;



    RiskySandBox_Team my_Team { get { return my_HumanPlayer.my_Team; } }
    string current_turn_state { get { return my_Team.current_turn_state.value; } }


    [SerializeField] ObservableInt slider_value;

    [SerializeField] UnityEngine.UI.Text current_turn_state_Text;


    [SerializeField] List<GameObject> input_elements = new List<GameObject>();
    [SerializeField] List<GameObject> permanent_deploy_elements = new List<GameObject>();
    [SerializeField] List<GameObject> permanent_attack_elements = new List<GameObject>();
    [SerializeField] List<GameObject> permanent_fortify_elements = new List<GameObject>();
    [SerializeField] List<GameObject> permanent_capture_elements = new List<GameObject>();



    private void Awake()
    {
        my_HumanPlayer.OnVariableUpdate_selected_Tile += EventReceiver_OnVariableUpdate_selected_Tile;
        my_HumanPlayer.OnVariableUpdate_attack_target += EventReceiver_OnVariableUpdate_attack_target;
        my_HumanPlayer.OnVariableUpdate_fortify_target += EventReceiver_OnVariableUpdate_fortify_target;

        RiskySandBox_Team.OnVariableUpdate_current_turn_state_STATIC += EventReceiver_OnVariableUpdate_current_turn_state;
    }

    private void OnDestroy()
    {
        RiskySandBox_Team.OnVariableUpdate_current_turn_state_STATIC -= EventReceiver_OnVariableUpdate_current_turn_state;
    }


    private void Start()
    {
        setGameObjectStates(input_elements, false);
        current_turn_state_Text.gameObject.SetActive(my_Team != null);

        setGameObjectStates(this.permanent_deploy_elements, my_HumanPlayer.is_mine && my_Team != null && current_turn_state == RiskySandBox_Team.turn_state_deploy);
        setGameObjectStates(this.permanent_attack_elements, my_HumanPlayer.is_mine && my_Team != null &&  current_turn_state == RiskySandBox_Team.turn_state_attack);
        setGameObjectStates(this.permanent_fortify_elements, my_HumanPlayer.is_mine && my_Team != null && current_turn_state == RiskySandBox_Team.turn_state_fortify);
        setGameObjectStates(this.permanent_capture_elements, my_HumanPlayer.is_mine && my_Team != null && current_turn_state == RiskySandBox_Team.turn_state_capture);
    }

    void setGameObjectStates(List<GameObject> _GameObjects, bool _state)
    {
        foreach(GameObject _GameObject in _GameObjects)
        {
            if (_GameObject == null)
                continue;

            _GameObject.SetActive(_state);
        }
    }

    


    void EventReceiver_OnVariableUpdate_current_turn_state(RiskySandBox_Team _Team)
    {
        if (_Team != my_HumanPlayer.my_Team)
            return;

        if (current_turn_state_Text != null)
        {
            current_turn_state_Text.gameObject.SetActive(my_HumanPlayer.is_mine);
            this.current_turn_state_Text.text = _Team.current_turn_state;
        }

        if(current_turn_state == RiskySandBox_Team.turn_state_capture)
        {
            slider_value.min_value = RiskySandBox_Tile.min_troops_per_Tile;
            slider_value.max_value = _Team.capture_start.num_troops - RiskySandBox_Tile.min_troops_per_Tile;
        }

        if(current_turn_state == RiskySandBox_Team.turn_state_waiting)
        {
            setGameObjectStates(input_elements, false);
        }

        setGameObjectStates(this.permanent_deploy_elements, my_HumanPlayer.is_mine && current_turn_state == RiskySandBox_Team.turn_state_deploy);
        setGameObjectStates(this.permanent_attack_elements, my_HumanPlayer.is_mine && current_turn_state == RiskySandBox_Team.turn_state_attack);
        setGameObjectStates(this.permanent_fortify_elements, my_HumanPlayer.is_mine && current_turn_state == RiskySandBox_Team.turn_state_fortify);
        setGameObjectStates(this.permanent_capture_elements, my_HumanPlayer.is_mine && current_turn_state == RiskySandBox_Team.turn_state_capture);

    }

    void EventReceiver_OnVariableUpdate_deployable_troops(RiskySandBox_Team _Team)
    {
        if (_Team != my_Team)
            return;

        if (current_turn_state == RiskySandBox_Team.turn_state_deploy)
        {
            slider_value.max_value = my_Team.deployable_troops;
        }

    }

    void EventReceiver_OnVariableUpdate_selected_Tile(RiskySandBox_HumanPlayer _HumanPlayer)
    {

        //if we are in the deploy state...
        if (_HumanPlayer != this.my_HumanPlayer)
            return;

        if (current_turn_state == RiskySandBox_Team.turn_state_deploy)
        {
            setGameObjectStates(input_elements, _HumanPlayer.selected_Tile != null);
            slider_value.min_value = 1;
            slider_value.max_value = my_Team.deployable_troops;
            slider_value.value = my_Team.deployable_troops;
        }
        else if (current_turn_state == RiskySandBox_Team.turn_state_attack)
        {
            setGameObjectStates(input_elements, this.my_HumanPlayer.attack_target != null);
        }
    }

    void EventReceiver_OnVariableUpdate_attack_target(RiskySandBox_HumanPlayer _HumanPlayer)
    {
        if (_HumanPlayer != my_HumanPlayer)
            return;

        if (current_turn_state != RiskySandBox_Team.turn_state_attack)
            return;

        setGameObjectStates(input_elements, _HumanPlayer.attack_target != null);

        if (_HumanPlayer.attack_target != null)
        {
            slider_value.min_value = 1;
            slider_value.max_value = _HumanPlayer.selected_Tile.num_troops - RiskySandBox_Tile.min_troops_per_Tile;
        }
        
    }

    void EventReceiver_OnVariableUpdate_fortify_target(RiskySandBox_HumanPlayer _HumanPlayer)
    {
        if (_HumanPlayer != my_HumanPlayer)
            return;

        if (current_turn_state != RiskySandBox_Team.turn_state_fortify)
            return;

        setGameObjectStates(input_elements, my_HumanPlayer.fortify_target != null);

        if(my_HumanPlayer.fortify_target != null)
        {
            slider_value.min_value = 1;
            slider_value.max_value = _HumanPlayer.selected_Tile.num_troops - RiskySandBox_Tile.min_troops_per_Tile;
        }

    }


}
