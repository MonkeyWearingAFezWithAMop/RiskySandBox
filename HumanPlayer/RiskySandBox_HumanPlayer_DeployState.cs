using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_HumanPlayer_DeployState : MonoBehaviour
{
    [SerializeField] bool debugging;

    [SerializeField] RiskySandBox_HumanPlayer my_HumanPlayer;
    [SerializeField] ObservableInt deploy_value;
    RiskySandBox_Tile deploy_target { get { return my_HumanPlayer.deploy_target; } set { this.my_HumanPlayer.deploy_target = value; } }

    RiskySandBox_Team my_Team { get { return my_HumanPlayer.my_Team; } }

    [SerializeField] List<GameObject> deploy_input_elements = new List<GameObject>();

    [SerializeField] GameObject deploy_UI;


    private void Awake()
    {
        RiskySandBox_MainGame.Ondeploy += EventReceiver_Ondeploy;

        RiskySandBox_HumanPlayer.OnleftClick += handleLeftClick;
        RiskySandBox_HumanPlayer.OnspaceKey += EventReceiver_OnspaceKey;
        RiskySandBox_HumanPlayer.OnVariableUpdate_deploy_target_STATIC += EventReceiver_OnVariableUpdate_deploy_target;

        this.deploy_value.OnUpdate += delegate { updateTileUIs(); };
        

    }

    private void OnDestroy()
    {
        RiskySandBox_MainGame.Ondeploy -= EventReceiver_Ondeploy;

        RiskySandBox_Team.OnVariableUpdate_current_turn_state_STATIC += EventReceiver_OnVariableUpdate_current_turn_state_STATIC;

        RiskySandBox_HumanPlayer.OnleftClick -= handleLeftClick;
        RiskySandBox_HumanPlayer.OnspaceKey -= EventReceiver_OnspaceKey;
        RiskySandBox_HumanPlayer.OnVariableUpdate_deploy_target_STATIC -= EventReceiver_OnVariableUpdate_deploy_target;


    }

    private void Start()
    {
        this.deploy_UI.SetActive(this.deploy_target != null);
    }

    void updateTileUIs()
    {
        if (my_Team.current_turn_state != RiskySandBox_Team.turn_state_deploy)
            return;

        ObservableInt.resetAllUIs();
        if(this.deploy_target != null)
        {
            this.deploy_target.num_troops.overrideUI(this.deploy_target.num_troops.value + this.deploy_value);
        }
    }


    /// <summary>
    /// code that runs when player left clicks in the "deploy" state...
    /// </summary>
    void handleLeftClick(RiskySandBox_HumanPlayer _HumanPlayer,string _current_turn_state)
    {
        if (_HumanPlayer != this.my_HumanPlayer)
            return;

        if (_current_turn_state != RiskySandBox_Team.turn_state_deploy)
            return;


        RiskySandBox_Tile _current_Tile = RiskySandBox_CameraControls.current_hovering_Tile;

        if (_current_Tile == null)
            return;

        if (this.my_HumanPlayer.deploy_target != null)
            return;

        this.my_HumanPlayer.deploy_target = _current_Tile;

    }

    void EventReceiver_OnspaceKey(RiskySandBox_HumanPlayer _HumanPlayer,string _current_turn_state)
    {
        if (_HumanPlayer != this.my_HumanPlayer)
            return;

        if (_current_turn_state != RiskySandBox_Team.turn_state_deploy)
            return;

        this.my_HumanPlayer.TRY_deploy(this.my_HumanPlayer.deploy_target, this.deploy_value);

    }

    public void EventReceiver_OnconfirmFromUI()
    {
        my_HumanPlayer.TRY_deploy(this.my_HumanPlayer.deploy_target, this.deploy_value);
    }



    void EventReceiver_OnVariableUpdate_deploy_target(RiskySandBox_HumanPlayer _HumanPlayer)
    {
        if (_HumanPlayer != this.my_HumanPlayer)
            return;

        this.deploy_UI.SetActive(_HumanPlayer.deploy_target != null);

        updateTileUIs();
        

        if (_HumanPlayer.deploy_target == null)
        {
            return;
        }

        this.deploy_value.min_value = 1;
        this.deploy_value.max_value = _HumanPlayer.my_Team.deployable_troops.value;
        this.deploy_value.value = this.deploy_value.max_value;
    }

    void EventReceiver_Ondeploy(RiskySandBox_MainGame.EventInfo_Ondeploy _EventInfo)
    {
        if (_EventInfo.Team != my_Team)
            return;

        this.my_HumanPlayer.deploy_target = null;
    }

    void EventReceiver_OnVariableUpdate_current_turn_state_STATIC(RiskySandBox_Team _Team)
    {
        if (_Team != this.my_Team)
            return;

        this.deploy_target = null;
    }
}
