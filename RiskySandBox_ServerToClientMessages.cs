using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Photon.Pun;

public partial class RiskySandBox_ServerToClientMessages : MonoBehaviour
{
    [SerializeField] bool debugging;

    PhotonView my_PhotonView;
	

    void Awake()
    {
        my_PhotonView = GetComponent<PhotonView>();
        RiskySandBox_MainGame.OnSET_num_troops_MultiplayerBridge += RiskySandBox_TileEventReceiver_OnSET_num_troops_MultiplayerBridge;
        RiskySandBox_MainGame.OnSET_my_Team_MultiplayerBridge += RiskySandBox_TileEventReceiver_OnSET_my_Team_MultiplayerBridge;
        RiskySandBox_MainGame.OnendGame_MultiplayerBridge += EventReceiver_OnendGame_MultiplayerBridge;

        RiskySandBox_MainGame.Ondeploy_MultiplayerBridge += RiskySandBox_TeamEventReceiver_Ondeploy_MultiplayerBridge;
        RiskySandBox_MainGame.Onattack_MultiplayerBridge += EventReceiver_Onattack_MultiplayerBridge;
    }




    void RiskySandBox_TileEventReceiver_OnSET_num_troops_MultiplayerBridge(int _Tile_ID,int _num_troops)
    {
        if (PrototypingAssets.run_server_code.value == false)//if we are not the server...
            return;//don't care...

        foreach(Photon.Realtime.Player _Player in PhotonNetwork.PlayerListOthers)
        {
            //TODO - do not tell the player if the fog of war hides this information...
            my_PhotonView.RPC("ServerInvokedRPC_SET_num_troops", _Player, _Tile_ID, _num_troops);
        }
    }

    void RiskySandBox_TileEventReceiver_OnSET_my_Team_MultiplayerBridge(int _Tile_ID, int _new_Team_ID)
    {
        if (PrototypingAssets.run_server_code.value == false)//if we are not the server?
            return;

        foreach (Photon.Realtime.Player _Player in PhotonNetwork.PlayerListOthers)
        {
            //TODO - do not tell the player if the fog of war hides this infomation...
            my_PhotonView.RPC("ServerInvokedRPC_SET_my_Team", _Player, _Tile_ID, _new_Team_ID);
            
        }
    }

    void EventReceiver_OnendGame_MultiplayerBridge()
    {
        if (PrototypingAssets.run_server_code.value == false)//if we are not the server?
            return;

        my_PhotonView.RPC("ServerInvokedRPC_endGame", RpcTarget.Others);//tell everyone to end the game (disconnect and display endGame screen with the "winner")

    }




    void RiskySandBox_TeamEventReceiver_Ondeploy_MultiplayerBridge(RiskySandBox_MainGame.EventInfo_Ondeploy _EventInfo)
    {
        if (PrototypingAssets.run_server_code == false)
            return;

        //ok great... lets tell all? connected clients this has happened...
        foreach(Photon.Realtime.Player _Player in PhotonNetwork.PlayerListOthers)
        {
            //TODO - make sure the fog of war doesnt hide this infomation...
            my_PhotonView.RPC("ServerInvokedRPC_Ondeploy", _Player, _EventInfo.Team_ID, _EventInfo.Tile_ID,_EventInfo.n_troops);
        }
    }


    void EventReceiver_Onattack_MultiplayerBridge(RiskySandBox_MainGame.EventInfo_Onattack _EventInfo)
    {
        if (PrototypingAssets.run_server_code.value == false)//we are not the server...
            return;//just dont worry...

        foreach(Photon.Realtime.Player _Player in PhotonNetwork.PlayerListOthers)
        {
            //TODO - make sure the fog of war doesnt hide this event for the player...

            my_PhotonView.RPC("ServerInvokedRPC_Onattack", _Player, _EventInfo.Team.ID, _EventInfo.start_Tile.ID, _EventInfo.target_Tile.ID, _EventInfo.attacker_deaths, _EventInfo.defender_deaths, _EventInfo.capture_flag, _EventInfo.attack_method);
        }
    }



    //this is the actual rpc that get called on the clients...

    [PunRPC]
    void ServerInvokedRPC_SET_num_troops(int _Tile_ID, int _new_value, PhotonMessageInfo _PhotonMessageInfo)
    {
        if (_PhotonMessageInfo.Sender.IsMasterClient == false)//only listen to the server!
            return;//TODO - report the sender (to server?/anti cheat...)

        RiskySandBox_MainGame.instance.SET_num_troops(_Tile_ID, _new_value);
    }

    [PunRPC]
    void ServerInvokedRPC_SET_my_Team(int _Tile_ID, int _new_value_ID, PhotonMessageInfo _PhotonMessageInfo)
    {
        if (_PhotonMessageInfo.Sender.IsMasterClient == false)//only listen to the server...
            return;//TODO - report the sender to server?/anti cheat...


        RiskySandBox_MainGame.instance.SET_my_Team(_Tile_ID, _new_value_ID);
    }


    [PunRPC]
    void ServerInvokedRPC_Ondeploy(int _TeamID, int _Tile_ID, int _n_troops, PhotonMessageInfo _PhotonMessageInfo)
    {
        if (_PhotonMessageInfo.Sender != PhotonNetwork.MasterClient)//only listen to the server!
            return;

        //add troops to the Tile...
        RiskySandBox_Team _Team = RiskySandBox_Team.GET_RiskySandBox_Team(_TeamID);
        RiskySandBox_Tile _Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(_Tile_ID);
        RiskySandBox_MainGame.instance.invokeEvent_Ondeploy(_Team, _Tile, _n_troops, _alert_MultiplayerBridge: false);
    }

    [PunRPC]
    void ServerInvokedRPC_Onattack(int _attacker_Team_ID,int _start_Tile_id,int _target_Tile_id,int _attacker_deaths,int _defender_deaths,bool _capture_flag,string _attack_method,PhotonMessageInfo _PhotonMessageInfo)
    {
        if (_PhotonMessageInfo.Sender != PhotonNetwork.MasterClient)//only listen to the server!
            return;

        RiskySandBox_Team _Team = RiskySandBox_Team.GET_RiskySandBox_Team(_attacker_Team_ID);
        RiskySandBox_Tile _start_Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(_start_Tile_id);
        RiskySandBox_Tile _target_Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(_target_Tile_id);

        RiskySandBox_MainGame.instance.invokeEvent_Onattack(_Team, _start_Tile, _target_Tile, _attacker_deaths, _defender_deaths, _capture_flag, _attack_method, _alert_MultiplayerBridge: false);
    }

    [PunRPC]
    void ServerInvokedRPC_endGame(PhotonMessageInfo _PhotonMessageInfo)
    {
        //disconnect...
        if (_PhotonMessageInfo.Sender != PhotonNetwork.MasterClient)//only listen to the server!
            return;


        RiskySandBox_MainGame.instance.endGame();
    }







}
