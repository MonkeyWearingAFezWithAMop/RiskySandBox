using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Photon.Pun;

public partial class RiskySandBox_HumanPlayer
{


    public void TRY_placeCapital(RiskySandBox_Tile _Tile)
    {
        if (_Tile == null)
            return;
        my_PhotonView.RPC("ClientInvokedRPC_placeCapital", RpcTarget.MasterClient, (int)_Tile.ID);
    }


    [PunRPC]
    void ClientInvokedRPC_placeCapital(int _tile_ID, PhotonMessageInfo _PhotonMessageInfo)
    {
        if (PrototypingAssets.run_server_code.value == false || _PhotonMessageInfo.Sender != this.my_PhotonView.Owner)
            return;
        RiskySandBox_MainGame.instance.TRY_placeCapital(RiskySandBox_Tile.GET_RiskySandBox_Tile(_tile_ID));
    }
}
