using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Photon.Pun;

public partial class RiskySandBox_HumanPlayer
{

    void handleLeftClick_placeCapital()
    {
        TRY_placeCapital();
    }


    public void TRY_placeCapital()
    {
        if (RiskySandBox_CameraControls.current_hovering_Tile == null)
            return;
        my_PhotonView.RPC("ClientInvokedRPC_placeCapital", RpcTarget.MasterClient, (int)RiskySandBox_CameraControls.current_hovering_Tile.ID);
    }


    [PunRPC]
    void ClientInvokedRPC_placeCapital(int _tile_ID, PhotonMessageInfo _PhotonMessageInfo)
    {
        if (PrototypingAssets.run_server_code.value == false || _PhotonMessageInfo.Sender != this.my_PhotonView.Owner)
            return;
        RiskySandBox_MainGame.instance.TRY_placeCapital(RiskySandBox_Tile.GET_RiskySandBox_Tile(_tile_ID));
    }
}
