using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Photon.Pun;

public partial class RiskySandBox_HumanPlayer
{





    public void TRY_deploy(RiskySandBox_Tile _Tile, int _n_troops)
    {
        //TODO - add if(debugging) statemenets...
        if (_Tile == null)
            return;

        if (this.my_Team.current_turn_state != RiskySandBox_Team.turn_state_deploy)
            return;


        if (this.debugging)
            GlobalFunctions.print("asking server to deploy to " + _n_troops + " to the Tile with ID = " + _Tile.ID, this);

        my_PhotonView.RPC("ClientInvokedRPC_deploy", RpcTarget.MasterClient, (int)_Tile.ID, (int)_n_troops);//ask server to deploy...
    }





    [PunRPC]
    void ClientInvokedRPC_deploy(int _Tile_ID, int _n_troops, PhotonMessageInfo _PhotonMessageInfo)
    {

        if (PrototypingAssets.run_server_code.value == false || _PhotonMessageInfo.Sender != this.my_PhotonView.Owner)
            return;


        if (this.debugging)
            GlobalFunctions.print("received a deploy request from a client...", this, _Tile_ID, _n_troops, _PhotonMessageInfo);

        RiskySandBox_Tile _Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(_Tile_ID);
        RiskySandBox_MainGame.instance.deploy(my_Team, _Tile, _n_troops);
    }




}
