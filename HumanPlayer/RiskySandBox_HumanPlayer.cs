using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Photon.Pun;

public partial class RiskySandBox_HumanPlayer : MonoBehaviour
{
    public static ObservableList<RiskySandBox_HumanPlayer> all_instances = new ObservableList<RiskySandBox_HumanPlayer>();

    public event Action<RiskySandBox_HumanPlayer> OnVariableUpdate_selected_Tile;
    public event Action<RiskySandBox_HumanPlayer> OnVariableUpdate_attack_target;
    public event Action<RiskySandBox_HumanPlayer> OnVariableUpdate_fortify_target;

    public static RiskySandBox_HumanPlayer local_player
    {
        get
        {
            foreach(RiskySandBox_HumanPlayer _Player in RiskySandBox_HumanPlayer.all_instances)
            {
                if (_Player.my_PhotonView.IsMine == true)
                    return _Player;
            }
            return null;
        }
    }


    [SerializeField] bool debugging;

    [SerializeField] Photon.Pun.PhotonView my_PhotonView;

    public bool is_mine { get { return my_PhotonView.IsMine; } }

    public RiskySandBox_Team my_Team { get { return RiskySandBox_Team.GET_RiskySandBox_Team(PRIVATE_my_Team_ID.value); } }
    public ObservableInt my_Team_ID { get { return PRIVATE_my_Team_ID; } }
    [SerializeField] ObservableInt PRIVATE_my_Team_ID;


    //note sometimes you want to absolutely rampage across the map...
    //e.g. at the end of the game you  want to just blast straight through anything in your path and send all surviving troops into the newly captured region...
    [SerializeField] ObservableString control_scheme;




    [SerializeField] ObservableInt slider_value;


    public RiskySandBox_Tile selected_Tile
    {
        get { return PRIVATE_selected_Tile; }
        private set
        {
            if (this.PRIVATE_selected_Tile != null)
               this.PRIVATE_selected_Tile.selected.value = false;

            this.PRIVATE_selected_Tile = value;

            if(value != null)
                value.selected.value = true;
            
            OnVariableUpdate_selected_Tile?.Invoke(this);


            ObservableInt.resetAllUIs();
        }
    }

    [SerializeField] RiskySandBox_Tile PRIVATE_selected_Tile;


    private void Awake()
    {
        RiskySandBox_HumanPlayer.all_instances.Add(this);
        my_PhotonView = GetComponent<Photon.Pun.PhotonView>();



        RiskySandBox_Team.OnVariableUpdate_current_turn_state_STATIC += EventReceiver_OnVariableUpdate_current_turn_state_STATIC;
        RiskySandBox_Team.OnVariableUpdate_capture_end_ID_STATIC += EventReceiver_OnVariableUpdate_capture_end_ID_STATIC;

        RiskySandBox_MainGame.Ondeploy += EventReceiver_Ondeploy;
        RiskySandBox_MainGame.Onattack += EventReceiver_Onattack;
        RiskySandBox_MainGame.Onfortify += EventReceiver_Onfortify;
        RiskySandBox_MainGame.Oncapture += EventReceiver_Oncapture;
    }


    private void OnDestroy()
    {
        RiskySandBox_HumanPlayer.all_instances.Remove(this);

        RiskySandBox_Team.OnVariableUpdate_current_turn_state_STATIC -= EventReceiver_OnVariableUpdate_current_turn_state_STATIC;
        RiskySandBox_Team.OnVariableUpdate_capture_end_ID_STATIC -= EventReceiver_OnVariableUpdate_capture_end_ID_STATIC;

        RiskySandBox_MainGame.Ondeploy -= EventReceiver_Ondeploy;
        RiskySandBox_MainGame.Onattack -= EventReceiver_Onattack;
        RiskySandBox_MainGame.Onfortify -= EventReceiver_Onfortify;
        RiskySandBox_MainGame.Oncapture -= EventReceiver_Oncapture;
    }



    


    private void Update()
    {
        if (my_PhotonView.IsMine == false)
            return;

        RiskySandBox_Tile _current_Tile = RiskySandBox_CameraControls.current_hovering_Tile;





        //if we left click on a tile?
        if (Input.GetMouseButtonDown(0))
        {
            if (my_Team == null)
                return;

            string _current_state = my_Team.current_turn_state;

            if (_current_state == RiskySandBox_Team.turn_state_waiting)
            {
                if (this.debugging)
                    GlobalFunctions.print("currently in the waiting state... returning", my_Team.current_turn_state);
                return;
            }

            else if (_current_state == RiskySandBox_Team.turn_state_deploy)
            {
                handleLeftClick_deploy();
            }

            else if (_current_state == RiskySandBox_Team.turn_state_attack)
            {
                handleLeftClick_attack();
            }

            else if (_current_state == RiskySandBox_Team.turn_state_fortify)
            {
                handleLeftClick_fortify();
            }

            else if (_current_state == RiskySandBox_Team.turn_state_placing_capital)
            {
                handleLeftClick_placeCapital();
            }
        }


        if(Input.GetKeyDown(KeyCode.Escape) == true)
        {
            //if the main game has started?
            //ok! - lets cancel everything...
            if(this.selected_Tile == null)
            {
                //open up the "quit ui"

            }

            cancel();
        }

        if(Input.GetKeyDown(KeyCode.Space) == true)
        {
            //"confirm the current action"
            
            if(my_Team.current_turn_state == RiskySandBox_Team.turn_state_deploy && this.selected_Tile != null)//so if we are in deploy mode? and we have a tile selected?
            {
                TRY_deploy();
            }

            else if(my_Team.current_turn_state == RiskySandBox_Team.turn_state_attack && this.selected_Tile != null)
            {
                if (this.attack_target != null)
                    TRY_attack();
                
                else
                {
                    this.attack_target = RiskySandBox_CameraControls.current_hovering_Tile;
                    TRY_attack();
                }

            
                
            }

            else if(my_Team.current_turn_state == RiskySandBox_Team.turn_state_capture)
            {
                TRY_capture();
            }
            else if(my_Team.current_turn_state == RiskySandBox_Team.turn_state_fortify && this.selected_Tile != null && this.fortify_target != null)
            {
                TRY_fortify();
            }


        }


    }


    public void cancelFromUI()
    {
        cancel();

    }

    public void cancel()
    {
        this.selected_Tile = null;
        this.attack_target = null;
        this.fortify_target = null;
    }

    public void confirmFromUI()
    {
        if (debugging)
            GlobalFunctions.print("current_turn_state = " + my_Team.current_turn_state,this);
        if(this.my_Team.current_turn_state == RiskySandBox_Team.turn_state_deploy)
        {
            TRY_deploy();
        }
        else if(this.my_Team.current_turn_state == RiskySandBox_Team.turn_state_attack)
        {
            TRY_attack();
        }
        else if(this.my_Team.current_turn_state == RiskySandBox_Team.turn_state_fortify)
        {
            TRY_fortify();
        }
        else if(this.my_Team.current_turn_state == RiskySandBox_Team.turn_state_capture)
        {
            TRY_capture();
        }

    }

    void EventReceiver_OnVariableUpdate_current_turn_state_STATIC(RiskySandBox_Team _Team)
    {
        if (_Team != my_Team)
            return;


        if(_Team.current_turn_state.previous_value == RiskySandBox_Team.turn_state_capture && _Team.current_turn_state == RiskySandBox_Team.turn_state_attack)
        {
            //we are going to select the new tile...
            this.selected_Tile = _Team.capture_target;
            this.attack_target = null;
            this.fortify_target = null;
        }
        else
        {
            this.selected_Tile = null;
            this.attack_target = null;
            this.fortify_target = null;

        }


        updateCaptureSlider();
    }

    void EventReceiver_Ondeploy(RiskySandBox_MainGame.EventInfo_Ondeploy _EventInfo)
    {
        if (_EventInfo.Team != my_Team)
            return;
        selected_Tile = null;
        

    }

    void EventReceiver_Onattack(RiskySandBox_MainGame.EventInfo_Onattack _EventInfo)
    {
        //someone just attacked...
        if (_EventInfo.Team != this.my_Team)
            return;
    }



    void EventReceiver_Onfortify(RiskySandBox_MainGame.EventInfo_Onfortify _EventInfo)
    {
        if (_EventInfo.Team != my_Team)
            return;

        

    }


    void EventReceiver_Oncapture(RiskySandBox_MainGame.EventInfo_Oncapture _EventInfo)
    {
        if (_EventInfo.Team != my_Team)
            return;
        this.selected_Tile = _EventInfo.Tile;
        
        
    }







    public void TRY_nextState()
    {
        my_PhotonView.RPC("ClientInvokedRPC_goToNextTurnState", RpcTarget.MasterClient);//ask the MasterClient to put me into the next turn state....
    }









    //rpc to allow the player to move into the next "turn state"
    [PunRPC]
    void ClientInvokedRPC_goToNextTurnState(PhotonMessageInfo _PhotonMessageInfo)
    {
        if (PrototypingAssets.run_server_code.value == false || _PhotonMessageInfo.Sender != this.my_PhotonView.Owner)
            return;

        RiskySandBox_MainGame.instance.goToNextTurnState(my_Team);
    }










}
