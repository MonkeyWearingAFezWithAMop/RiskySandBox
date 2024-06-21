using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_HumanPlayer_CaptureState : MonoBehaviour
{
    [SerializeField] bool debugging;
    [SerializeField] RiskySandBox_HumanPlayer my_HumanPlayer;

    RiskySandBox_Team my_Team { get { return my_HumanPlayer.my_Team; } }


    [SerializeField] ObservableInt capture_value;

    [SerializeField] GameObject root_UI;



    private void Awake()
    {
        RiskySandBox_MainGame.Oncapture += EventReceiver_Oncapture;

        RiskySandBox_Team.OnVariableUpdate_current_turn_state_STATIC += EventReceiver_OnVariableUpdate_current_turn_state_STATIC;

        RiskySandBox_HumanPlayer.OnspaceKey += EventReceiver_OnspaceKey;

        this.capture_value.OnUpdate += delegate { updateTileUIs(); };
    }

    private void OnDestroy()
    {
        RiskySandBox_MainGame.Oncapture -= EventReceiver_Oncapture;

        RiskySandBox_Team.OnVariableUpdate_current_turn_state_STATIC -= EventReceiver_OnVariableUpdate_current_turn_state_STATIC;

        RiskySandBox_HumanPlayer.OnspaceKey -= EventReceiver_OnspaceKey;
    }

    void updateTileUIs()
    {
        if (my_Team.current_turn_state != RiskySandBox_Team.turn_state_capture)
            return;

        ObservableInt.resetAllUIs();
        if(my_Team.capture_start != null && my_Team.capture_target != null)
        {
            this.my_Team.capture_start.num_troops.overrideUI(this.my_Team.capture_start.num_troops.value - capture_value.value);
            this.my_Team.capture_target.num_troops.overrideUI(this.my_Team.capture_target.num_troops.value + capture_value.value);
        }
    }


    private void Start()
    {
        this.root_UI.SetActive(my_Team != null && my_Team.current_turn_state == RiskySandBox_Team.turn_state_capture);
    }


    void EventReceiver_Oncapture(RiskySandBox_MainGame.EventInfo_Oncapture _EventInfo)
    {
        if (_EventInfo.Team != my_Team)
            return;
    }


    public void EventReceiver_OnconfirmFromUI()
    {
        this.my_HumanPlayer.TRY_capture(this.capture_value);
    }


    void EventReceiver_OnspaceKey(RiskySandBox_HumanPlayer _HumanPlayer,string _current_turn_state)
    {
        if (_HumanPlayer != this.my_HumanPlayer)
            return;

        if (_current_turn_state != RiskySandBox_Team.turn_state_capture)
            return;

        this.my_HumanPlayer.TRY_capture(this.capture_value);
    }

    void EventReceiver_OnVariableUpdate_current_turn_state_STATIC(RiskySandBox_Team _Team)
    {
        if (_Team != this.my_Team)
            return;

        

        bool _in_capture_state = my_Team.current_turn_state.value == RiskySandBox_Team.turn_state_capture;
        this.root_UI.SetActive(_in_capture_state);

        if(_in_capture_state == true)
        {
            this.capture_value.min_value = RiskySandBox_Tile.min_troops_per_Tile;
            this.capture_value.max_value = this.my_Team.capture_start.num_troops - RiskySandBox_Tile.min_troops_per_Tile;
            this.capture_value.value = this.capture_value.max_value;
            //TODO - if we are capturing into a "deadend" e.g. europe advanced progressive capitals... when you are trying to capture "sevastopal" there is no* stragic reason to send more than the minimum troops into that deadend
            //* in general... sometimes it makes sence but 99.99% of the time there is NO reason to do this you are simply "deactivating" troops for no reason...

        }

    }


}
