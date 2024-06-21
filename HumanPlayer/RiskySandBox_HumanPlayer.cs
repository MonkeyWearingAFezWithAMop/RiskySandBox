using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Photon.Pun;

public partial class RiskySandBox_HumanPlayer : MonoBehaviour
{
    public static ObservableList<RiskySandBox_HumanPlayer> all_instances = new ObservableList<RiskySandBox_HumanPlayer>();


    public static event Action<RiskySandBox_HumanPlayer,string> OnconfirmFromUI;
    public static event Action<RiskySandBox_HumanPlayer,string> OnleftClick;
    
    public static event Action<RiskySandBox_HumanPlayer,string> Oncancel;

    //the space key is a shortcut for "confirm action"
    public static event Action<RiskySandBox_HumanPlayer,string> OnspaceKey;

    public static event Action<RiskySandBox_HumanPlayer> OnVariableUpdate_deploy_target_STATIC;
    public static event Action<RiskySandBox_HumanPlayer> OnVariableUpdate_attack_start_STATIC;
    public static event Action<RiskySandBox_HumanPlayer> OnVariableUpdate_attack_target_STATIC;
    public static event Action<RiskySandBox_HumanPlayer> OnVariableUpdate_fortify_start_STATIC;
    public static event Action<RiskySandBox_HumanPlayer> OnVariableUpdate_fortify_target_STATIC;


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

    /// <summary>
    /// the tile this HumanPlayer is trying to "deploy" to
    /// </summary>
    public RiskySandBox_Tile deploy_target
    {
        get { return this.PRIVATE_deploy_target; }
        set
        {
            if (this.PRIVATE_deploy_target != null)
                this.PRIVATE_deploy_target.is_deploy_target.value = false;

            if(value == null)
            {
                //fine just allow this and return
                this.PRIVATE_deploy_target = null;
                OnVariableUpdate_deploy_target_STATIC?.Invoke(this);
                return;
            }

            bool _can_deploy = RiskySandBox_MainGame.instance.canDeploy(my_Team, value, 1);
            if (_can_deploy == false)
                return;

            this.PRIVATE_deploy_target = value;
            value.is_deploy_target.value = true;

            OnVariableUpdate_deploy_target_STATIC?.Invoke(this);

        }
    }
    [SerializeField] RiskySandBox_Tile PRIVATE_deploy_target;

    public RiskySandBox_Tile attack_start
    {
        get { return PRIVATE_attack_start;}
        set
        {
            if (PRIVATE_attack_start != null)
                PRIVATE_attack_start.is_attack_start.value = false;

            if(value == null)
            {
                this.PRIVATE_attack_start = null;
                OnVariableUpdate_attack_start_STATIC?.Invoke(this);
                return;
            }
            //make sure it is one of my tiles...
            if (value.my_Team != this.my_Team)
                return;

            if (value.num_troops <= RiskySandBox_Tile.min_troops_per_Tile)
                return;

            this.PRIVATE_attack_start = value;
            value.is_attack_start.value = true;

            OnVariableUpdate_attack_start_STATIC?.Invoke(this);
        }
    }
    [SerializeField] RiskySandBox_Tile PRIVATE_attack_start;

    public RiskySandBox_Tile attack_target
    {
        get { return this.PRIVATE_attack_target; }
        set
        {
            if (PRIVATE_attack_target != null)
                PRIVATE_attack_target.is_attack_target.value = false;

            if(value == null)
            {
                this.PRIVATE_attack_target = null;
                OnVariableUpdate_attack_target_STATIC?.Invoke(this);
                return;
            }

            bool _can_attack = RiskySandBox_MainGame.instance.canAttack(this.attack_start, value);
            if (_can_attack == false)
                return;

            this.PRIVATE_attack_target = value;
            value.is_attack_target.value = true;

            OnVariableUpdate_attack_target_STATIC?.Invoke(this);
        }
    }
    [SerializeField] RiskySandBox_Tile PRIVATE_attack_target;

    public RiskySandBox_Tile fortify_start
    {
        get { return PRIVATE_fortify_start; }
        set
        {
            if (PRIVATE_fortify_start != null)
                PRIVATE_fortify_start.is_fortify_start.value = false;

            if(value == null)
            {
                this.PRIVATE_fortify_start = null;
                OnVariableUpdate_fortify_start_STATIC?.Invoke(this);
                return;
            }
            if (value.my_Team != this.my_Team)
                return;

            if (value.num_troops <= RiskySandBox_Tile.min_troops_per_Tile)
                return;

            this.PRIVATE_fortify_start = value;
            value.is_fortify_start.value = true;

            OnVariableUpdate_fortify_start_STATIC?.Invoke(this);
        }
    }
    [SerializeField] RiskySandBox_Tile PRIVATE_fortify_start;

    public RiskySandBox_Tile fortify_target
    {
        get { return this.PRIVATE_fortify_target; }
        set
        {
            if (PRIVATE_fortify_target != null)
                PRIVATE_fortify_target.is_fortify_target.value = false;

            if(value == null)
            {
                this.PRIVATE_fortify_target = null;
                OnVariableUpdate_fortify_target_STATIC?.Invoke(this);
                return;
            }

            bool _can_fortify = RiskySandBox_MainGame.instance.canFortify(this.fortify_start, value, 1);
            if (_can_fortify == false)
                return;

            this.PRIVATE_fortify_target = value;
            value.is_fortify_target.value = true;

            OnVariableUpdate_fortify_target_STATIC?.Invoke(this);
        }
    }
    [SerializeField] RiskySandBox_Tile PRIVATE_fortify_target;





    private void Awake()
    {
        RiskySandBox_HumanPlayer.all_instances.Add(this);
        my_PhotonView = GetComponent<Photon.Pun.PhotonView>();
    }


    private void OnDestroy()
    {
        RiskySandBox_HumanPlayer.all_instances.Remove(this);
    }



    


    private void Update()
    {
        if (my_PhotonView.IsMine == false)
            return;


        if (Input.GetKeyDown(KeyCode.Escape) == true)
        {
            if (this.deploy_target != null || this.attack_start != null || this.attack_target != null || this.fortify_start != null || this.fortify_target != null)
                cancel();
            else
                RiskySandBox_MainGame.instance.show_escape_menu.value = !RiskySandBox_MainGame.instance.show_escape_menu.value;
        }


        //if we left click on a tile?
        if (Input.GetMouseButtonDown(0))
        {
            if (my_Team != null)
                OnleftClick?.Invoke(this, my_Team.current_turn_state);
        }

        if(Input.GetKeyDown(KeyCode.Space) == true)
        {
            if (my_Team != null)
                OnspaceKey?.Invoke(this, my_Team.current_turn_state);
        }


    }

 


    public void cancel()
    {
        this.deploy_target = null;
        this.attack_start = null;
        this.attack_target = null;
        this.fortify_start = null;
        this.fortify_target = null;

        Oncancel?.Invoke(this,this.my_Team.current_turn_state);
    }

    public void confirmFromUI()
    {
        if (debugging)
            GlobalFunctions.print("current_turn_state = " + my_Team.current_turn_state,this);

        OnconfirmFromUI?.Invoke(this,this.my_Team.current_turn_state);

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


    [PunRPC]
    void ClientInvokedRPC_tradeInTerritoryCards(int[] _card_IDs, PhotonMessageInfo _PhotonMessageInfo)
    {
        if (PrototypingAssets.run_server_code.value == false || _PhotonMessageInfo.Sender != this.my_PhotonView.Owner)
        {
            if (this.debugging)
                GlobalFunctions.print("not the server or the sender wasnt the owner of this PhotonView...",this);
            return;
        }
        if (this.debugging)
            GlobalFunctions.print("asking the MainGame to call TRY_tradeInCards...", this);

        //ask the maingame to "trade in" these cards....


        RiskySandBox_MainGame.instance.TRY_tradeInCards(this.my_Team, _card_IDs);

        



    }



    public void TRY_tradeInSelectedTerritoryCards()
    {
        List<RiskySandBox_TerritoryCard> _selected_Cards = new List<RiskySandBox_TerritoryCard>(RiskySandBox_TerritoryCard.all_instances_is_selected);

        int[] _selected_Card_IDs = _selected_Cards.Select(x => x.tile_ID_READONLY).ToArray();

        my_PhotonView.RPC("ClientInvokedRPC_tradeInTerritoryCards", RpcTarget.MasterClient, _selected_Card_IDs);

    }







}
