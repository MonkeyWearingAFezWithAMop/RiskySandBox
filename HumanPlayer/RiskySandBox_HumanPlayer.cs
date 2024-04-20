using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Photon.Pun;

public partial class RiskySandBox_HumanPlayer : MonoBehaviour
{
    public static ObservableList<RiskySandBox_HumanPlayer> all_instances = new ObservableList<RiskySandBox_HumanPlayer>();

    public event Action<RiskySandBox_HumanPlayer> OnVariableUpdate_selected_Tile;
    public event Action<RiskySandBox_HumanPlayer> OnVariableUpdate_attack_target;
    public event Action<RiskySandBox_HumanPlayer> OnVariableUpdate_fortify_target;


    [SerializeField] bool debugging;

    [SerializeField] Photon.Pun.PhotonView my_PhotonView;

    public bool is_mine { get { return my_PhotonView.IsMine; } }

    public RiskySandBox_Team my_Team { get { return RiskySandBox_Team.GET_RiskySandBox_Team(PRIVATE_my_Team_ID.value); } }
    public ObservableInt my_Team_ID { get { return PRIVATE_my_Team_ID; } }
    [SerializeField] ObservableInt PRIVATE_my_Team_ID;


    [SerializeField] PrototypingAssets_RTSController my_RTSController;



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


            RiskySandBox_Tile.resetVisuals();

        }
    }

    [SerializeField] RiskySandBox_Tile PRIVATE_selected_Tile;

    public RiskySandBox_Tile attack_target
    {
        get { return PRIVATE_attack_target; }
        set
        {
            if (this.PRIVATE_attack_target != null)
                PRIVATE_attack_target.is_attack_target.value = false;

            this.PRIVATE_attack_target = value;

            if (value != null)
                value.is_attack_target.value = true;

            OnVariableUpdate_attack_target?.Invoke(this);

        }
    }
    [SerializeField] RiskySandBox_Tile PRIVATE_attack_target;



    public RiskySandBox_Tile fortify_target
    {
        get { return PRIVATE_fortify_target; }
        set
        {
            if (this.PRIVATE_fortify_target != null)
                this.PRIVATE_fortify_target.is_fortify_target.value = false;

            this.PRIVATE_fortify_target = value;

            if (this.PRIVATE_fortify_target != null)
                value.is_fortify_target.value = true;

            OnVariableUpdate_fortify_target?.Invoke(this);
        }
    }
    RiskySandBox_Tile PRIVATE_fortify_target;






    


    private void Awake()
    {
        RiskySandBox_HumanPlayer.all_instances.Add(this);
        my_PhotonView = GetComponent<Photon.Pun.PhotonView>();

        my_RTSController.enabled = this.my_PhotonView.IsMine;


        RiskySandBox_Team.OnVariableUpdate_current_turn_state_STATIC += EventReceiver_OnVariableUpdate_current_turn_state_STATIC;

        RiskySandBox_MainGame.Ondeploy += EventReceiver_Ondeploy;
        RiskySandBox_MainGame.Onattack += EventReceiver_Onattack;
        RiskySandBox_MainGame.Onfortify += EventReceiver_Onfortify;
        RiskySandBox_MainGame.Oncapture += EventReceiver_Oncapture;
    }


    private void OnDestroy()
    {
        RiskySandBox_HumanPlayer.all_instances.Remove(this);

        RiskySandBox_Team.OnVariableUpdate_current_turn_state_STATIC -= EventReceiver_OnVariableUpdate_current_turn_state_STATIC;

        RiskySandBox_MainGame.Ondeploy -= EventReceiver_Ondeploy;
        RiskySandBox_MainGame.Onattack -= EventReceiver_Onattack;
        RiskySandBox_MainGame.Onfortify -= EventReceiver_Onfortify;
        RiskySandBox_MainGame.Oncapture -= EventReceiver_Oncapture;
    }



    


    private void Update()
    {
        if (my_PhotonView.IsMine == false)
            return;

        //if we left click on a tile?
        if(Input.GetMouseButtonDown(0))
        {
            if (my_Team == null)
                return;
            this.handleLeftClick(my_Team.current_turn_state);
        }
    }


    /// <summary>
    /// code that runs when player left clicks in the "deploy" state...
    /// </summary>
    void handleLeftClick_deploy(RiskySandBox_Tile _Tile)
    {
        if (_Tile != null && _Tile.my_Team == this.my_Team)
        {
            if (this.debugging)
                GlobalFunctions.print("left clicked in the deploy state... _Tile was part of my_Team! - selecting it...", this, _Tile);
            this.selected_Tile = _Tile;
        }
    }

    /// <summary>
    /// code that runs when player left clicks in the "attack" state...
    /// </summary>
    void handleLeftClick_attack(RiskySandBox_Tile _Tile)
    {
        if (_Tile.my_Team == this.my_Team)
        {
            this.selected_Tile = _Tile;
        }
        else
        {
            if (this.selected_Tile != null)
            {
                this.attack_target = _Tile;
            }
        }
    }

    /// <summary>
    /// code that runs when player left clicks in the "fortify" state...
    /// </summary>
    void handleLeftClick_fortify(RiskySandBox_Tile _Tile)
    {
        if (_Tile.my_Team == this.my_Team)
        {
            if (this.selected_Tile == null)
                this.selected_Tile = _Tile;
            else if (_Tile != this.selected_Tile)
                this.fortify_target = _Tile;
        }
    }


    void handleLeftClick(string _current_state)
    {

        if (_current_state == RiskySandBox_Team.turn_state_waiting)
        {
            if (this.debugging)
                GlobalFunctions.print("currently in the waiting state... returning", my_Team.current_turn_state);
            return;
        }

        RaycastHit hit = my_RTSController.doRaycast();

        RiskySandBox_Tile _current_Tile = null;

        if(hit.collider != null)
            _current_Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(hit.collider); // Get the component attached to the hit object
        


        if (_current_state == RiskySandBox_Team.turn_state_deploy)
        {
            handleLeftClick_deploy(_current_Tile);
        }

        else if (_current_state == RiskySandBox_Team.turn_state_attack)
        {
            handleLeftClick_attack(_current_Tile);
        }

        else if(_current_state == RiskySandBox_Team.turn_state_fortify)
        {
            handleLeftClick_fortify(_current_Tile);
        }


    }

    public void cancelFromUI()
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

        this.selected_Tile = null;
        this.attack_target = null;
        this.fortify_target = null;
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


    public void TRY_deploy()
    {
        int _n_Troops = this.slider_value.value;
        RiskySandBox_Tile _Tile = this.selected_Tile;

        if (this.debugging)
            GlobalFunctions.print("asking server to deploy to " + _n_Troops + " to the Tile with ID = " + _Tile.ID,this);
        

        my_PhotonView.RPC("ClientInvokedRPC_deploy", RpcTarget.MasterClient, _Tile.ID, _n_Troops);//ask server to deploy...

    }

    public void TRY_attack()
    {
        int _n_troops = this.slider_value.value;

        int _from_ID = selected_Tile.ID;
        int _to_ID = attack_target.ID;

        if (this.debugging)
            GlobalFunctions.print("asking server to attack... _from_ID = " + _from_ID + ", _to_ID =  "+_to_ID +", _n_Troops = "+_n_troops,this);


        my_PhotonView.RPC("ClientInvokedRPC_attack", RpcTarget.MasterClient, _from_ID, _to_ID, _n_troops,"NULLVALUEJUSTIGNOREFORNOW!!!83hvp2y");
    }
    public void TRY_capture()
    {
        int _n_troops = this.slider_value.value ;
        my_PhotonView.RPC("ClientInvokedRPC_capture", RpcTarget.MasterClient, _n_troops);
    }

    public void TRY_fortify()
    {
        my_PhotonView.RPC("ClientInvokedRPC_fortify", RpcTarget.MasterClient, selected_Tile.ID, fortify_target.ID, this.slider_value.value);
    }

    public void TRY_nextState()
    {
        my_PhotonView.RPC("ClientInvokedRPC_goToNextTurnState", RpcTarget.MasterClient);//ask the MasterClient to put me into the next turn state....
    }






    [PunRPC]
    void ClientInvokedRPC_deploy(int _Tile_ID,int _n_troops,PhotonMessageInfo _PhotonMessageInfo)
    {
       
        if (PrototypingAssets.run_server_code.value == false || _PhotonMessageInfo.Sender != this.my_PhotonView.Owner)
            return;


        if (this.debugging)
            GlobalFunctions.print("received a deploy request from a client...", this,_Tile_ID,_n_troops,_PhotonMessageInfo);

        RiskySandBox_Tile _Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(_Tile_ID);
        RiskySandBox_MainGame.instance.deploy(my_Team, _Tile, _n_troops);
    }

    [PunRPC]
    void ClientInvokedRPC_attack(int _from_ID,int _to_ID, int _n_troops,string _attack_method, PhotonMessageInfo _PhotonMessageInfo)
    {
        if (PrototypingAssets.run_server_code.value == false || _PhotonMessageInfo.Sender != this.my_PhotonView.Owner)
            return;

        RiskySandBox_Tile _from = RiskySandBox_Tile.GET_RiskySandBox_Tile(_from_ID);
        RiskySandBox_Tile _to = RiskySandBox_Tile.GET_RiskySandBox_Tile(_to_ID);
        RiskySandBox_MainGame.instance.attack(my_Team, _from, _to, _n_troops, _attack_method);
    }

    [PunRPC]
    void ClientInvokedRPC_capture(int _n_troops, PhotonMessageInfo _PhotonMessageInfo)
    {
        if (PrototypingAssets.run_server_code.value == false || _PhotonMessageInfo.Sender != this.my_PhotonView.Owner)
            return;

        RiskySandBox_MainGame.instance.capture(my_Team, _n_troops);
    }

    [PunRPC]
    void ClientInvokedRPC_fortify(int _from_ID,int _to_ID, int _n_troops, PhotonMessageInfo _PhotonMessageInfo)
    {
        if (PrototypingAssets.run_server_code.value == false || _PhotonMessageInfo.Sender != this.my_PhotonView.Owner)
            return;

        RiskySandBox_Tile _from = RiskySandBox_Tile.GET_RiskySandBox_Tile(_from_ID);
        RiskySandBox_Tile _to = RiskySandBox_Tile.GET_RiskySandBox_Tile(_to_ID);
        bool _fortified = RiskySandBox_MainGame.instance.fortify(my_Team, _from, _to, _n_troops);

        if (_fortified == true)//if we successfully fortified....
            RiskySandBox_MainGame.instance.goToNextTurnState(my_Team);
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
