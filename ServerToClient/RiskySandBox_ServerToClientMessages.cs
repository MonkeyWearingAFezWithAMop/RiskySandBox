using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Photon.Pun;

public partial class RiskySandBox_ServerToClientMessages : MonoBehaviour
{
    public static RiskySandBox_ServerToClientMessages instance;

    [SerializeField] bool debugging;

    PhotonView my_PhotonView;

    //TODO - include the debug statements this is very important server and client!

    void Awake()
    {
        instance = this;
        my_PhotonView = GetComponent<PhotonView>();

        //Tile events... TODO - we need to move these events over to the RiskySandBox_Tile class???
        RiskySandBox_MainGame.OnSET_num_troops_MultiplayerBridge += RiskySandBox_TileEventReceiver_OnSET_num_troops_MultiplayerBridge;
        RiskySandBox_MainGame.OnSET_my_Team_MultiplayerBridge += RiskySandBox_TileEventReceiver_OnSET_my_Team_MultiplayerBridge;
        RiskySandBox_MainGame.OnSET_has_capital_MultiplayerBridge += RiskySandBox_TileEventReceiver_OnSET_has_capital_MultiplayerBridge;


        RiskySandBox_MainGame.OnstartGame_MultiplayerBridge += EventReceiver_OnstartGame_MultiplayerBridge;
        RiskySandBox_MainGame.OnendGame_MultiplayerBridge += EventReceiver_OnendGame_MultiplayerBridge;

        
        RiskySandBox_MainGame.Ondeploy_MultiplayerBridge += RiskySandBox_TeamEventReceiver_Ondeploy_MultiplayerBridge;
        RiskySandBox_MainGame.Onattack_MultiplayerBridge += EventReceiver_Onattack_MultiplayerBridge;
        RiskySandBox_MainGame.Onfortify_MultiplayerBridge += EventReceiver_Onfortify_MultiplayerBridge;
        RiskySandBox_MainGame.Oncapture_MultiplayerBridge += EventReceiver_Oncapture_MultiplayerBridge;



    }


    void RiskySandBox_TileEventReceiver_OnSET_has_capital_MultiplayerBridge(int _tile_ID,bool _new_value)
    {
        if (PrototypingAssets.run_server_code == false)//if we are not the server???
            return;//ok this client donsn't care... TODO - WTF?!?!?!?!?!

        foreach(Photon.Realtime.Player _Player in PhotonNetwork.PlayerListOthers)
        {
            //TODO - if FOW hides this information... continue...
            my_PhotonView.RPC("ServerInvokedRPC_SET_has_capital", _Player, _tile_ID, _new_value);
        }

    }

    [PunRPC]
    void ServerInvokedRPC_SET_has_capital(int _tile_ID,bool _value,PhotonMessageInfo _PhotonMessageInfo)
    {
        if (_PhotonMessageInfo.Sender.IsMasterClient == false)//only listen to the server!
            return;

        RiskySandBox_Tile _Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(_tile_ID);
        RiskySandBox_MainGame.instance.SET_has_capital(_Tile, _value);

    }


    void EventReceiver_OnstartGame_MultiplayerBridge()
    {

    }


    void RiskySandBox_TileEventReceiver_OnSET_num_troops_MultiplayerBridge(int _Tile_ID,int _num_troops)
    {
        if (PrototypingAssets.run_server_code.value == false)//if we are not the server...
            return;//don't care... TODO - DEBUG WTF?!?!?!?!

        foreach(Photon.Realtime.Player _Player in PhotonNetwork.PlayerListOthers)
        {
            //TODO - do not tell the player if the fog of war hides this information...
            my_PhotonView.RPC("ServerInvokedRPC_SET_num_troops", _Player, _Tile_ID, _num_troops);
        }
    }

    void RiskySandBox_TileEventReceiver_OnSET_my_Team_MultiplayerBridge(int _Tile_ID, int _new_Team_ID)
    {
        if (PrototypingAssets.run_server_code.value == false)//if we are not the server?
            return;//TODO - DEBUG - WTF?!?!?!?

        foreach (Photon.Realtime.Player _Player in PhotonNetwork.PlayerListOthers)
        {
            //TODO - do not tell the player if the fog of war hides this infomation...
            my_PhotonView.RPC("ServerInvokedRPC_SET_my_Team", _Player, _Tile_ID, _new_Team_ID);
            
        }
    }

    void EventReceiver_OnendGame_MultiplayerBridge()
    {
        if (PrototypingAssets.run_server_code.value == false)//if we are not the server?
            return;//TODO - debug WTF?!?!?!?!?!

        my_PhotonView.RPC("ServerInvokedRPC_endGame", RpcTarget.Others);//tell everyone to end the game (disconnect and display endGame screen with the "winner")

    }




    void RiskySandBox_TeamEventReceiver_Ondeploy_MultiplayerBridge(RiskySandBox_MainGame.EventInfo_Ondeploy _EventInfo)
    {
        if (PrototypingAssets.run_server_code == false)
            return;//TODO - WTF>!!?!?!?!?!?

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
            return;//just dont worry... TODO - debug WTF?!?!?!?

        foreach(Photon.Realtime.Player _Player in PhotonNetwork.PlayerListOthers)
        {
            //TODO - make sure the fog of war doesnt hide this event for the player...

            my_PhotonView.RPC("ServerInvokedRPC_Onattack", _Player, (int)_EventInfo.attacking_Team.ID, (int)_EventInfo.defending_Team.ID, _EventInfo.start_Tile.ID.value, _EventInfo.target_Tile.ID.value, _EventInfo.attacker_deaths, _EventInfo.defender_deaths, _EventInfo.capture_flag, _EventInfo.attack_method);
        }
    }

    void EventReceiver_Oncapture_MultiplayerBridge(RiskySandBox_MainGame.EventInfo_Oncapture _EventInfo)
    {
        if (PrototypingAssets.run_server_code.value == false)//if we are not the server???
            return;//ok something weird is happening... but ok lets just supress this for now... TODO - add debug WTF??!?!?! statement?

        foreach(Photon.Realtime.Player _Player in PhotonNetwork.PlayerListOthers)
        {
            //TODO - if the fog of war hides this event? - continue
            my_PhotonView.RPC("ServerInvokedRPC_Oncapture", _Player, (int) _EventInfo.Team.ID, (int)_EventInfo.Tile.ID, _EventInfo.n_troops);
        }
               
    }

    void EventReceiver_Onfortify_MultiplayerBridge(RiskySandBox_MainGame.EventInfo_Onfortify _EventInfo)
    {
        if (PrototypingAssets.run_server_code.value == false)//if we are not the server????
            return;//ok something weird is happening... but fine lets just supress for now... TODO - debug WTF??!?!?!??!!?

        foreach(Photon.Realtime.Player _Player in PhotonNetwork.PlayerListOthers)
        {
            //TODO - if the fog of war hides this event - continue;
            //TODO we also may want to tell the player  the "path" that the fortify took??
            //but dont tell them about the route if they cant see the tiles
            my_PhotonView.RPC("ServerInvokedRPC_Onfortify", _Player, (int) _EventInfo.Team.ID, (int)_EventInfo.from.ID, (int)_EventInfo.to.ID, (int)_EventInfo.n_troops);
        }

    }

    [PunRPC]
    void ServerInvokedRPC_Onfortify(int _Team_ID,int _from_ID,int _to_ID,int _n_troops, PhotonMessageInfo _PhotonMessageInfo)
    {
        if (_PhotonMessageInfo.Sender.IsMasterClient == false)//ONLY LISTEND TO THE SERVER
            return;//TODO - report this player as a cheater? - or something is going wrong...

        RiskySandBox_Team _Team = RiskySandBox_Team.GET_RiskySandBox_Team(_Team_ID);
        RiskySandBox_Tile _from = RiskySandBox_Tile.GET_RiskySandBox_Tile(_from_ID);
        RiskySandBox_Tile _to = RiskySandBox_Tile.GET_RiskySandBox_Tile(_to_ID);

        RiskySandBox_MainGame.instance.invokeEvent_Onfortify(_Team, _from, _to, _n_troops,_alert_MultiplayerBridge:false);


    }


    [PunRPC]
    void ServerInvokedRPC_Oncapture(int _Team_ID,int _Tile_ID,int _n_troops,PhotonMessageInfo _PhotonMessageInfo)
    {
        if (_PhotonMessageInfo.Sender.IsMasterClient == false)//ONLY LISTEND TO THE SERVER
            return;//TODO - report this player as a cheater? - or something is going wrong...

        //ok! - the server just told us a team captured...
        RiskySandBox_Team _Team = RiskySandBox_Team.GET_RiskySandBox_Team(_Team_ID);
        RiskySandBox_Tile _Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(_Tile_ID);

        RiskySandBox_MainGame.instance.invokeEvent_Oncapture(_Team, _Tile, _n_troops, _alert_MultiplayerBridge:false);//dont tell the multiplayer bridge since this is the client...

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
    void ServerInvokedRPC_Onattack(int _attacker_Team_ID,int _defender_Team_ID,int _start_Tile_id,int _target_Tile_id,int _attacker_deaths,int _defender_deaths,bool _capture_flag,string _attack_method,PhotonMessageInfo _PhotonMessageInfo)
    {
        if (_PhotonMessageInfo.Sender != PhotonNetwork.MasterClient)//only listen to the server!
            return;

        RiskySandBox_Team _Team = RiskySandBox_Team.GET_RiskySandBox_Team(_attacker_Team_ID);
        RiskySandBox_Tile _start_Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(_start_Tile_id);
        RiskySandBox_Tile _target_Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(_target_Tile_id);
        RiskySandBox_Team _defender_Team = RiskySandBox_Team.GET_RiskySandBox_Team(_defender_Team_ID);

        RiskySandBox_MainGame.instance.invokeEvent_Onattack(_Team,_defender_Team, _start_Tile, _target_Tile, _attacker_deaths, _defender_deaths, _capture_flag, _attack_method, _alert_MultiplayerBridge: false);
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
