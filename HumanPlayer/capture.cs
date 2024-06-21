using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Photon.Pun;

public partial class RiskySandBox_HumanPlayer
{
    public void TRY_capture(int _n_troops)
    {
        if (this.my_Team.current_turn_state != RiskySandBox_Team.turn_state_capture)
            return;

        my_PhotonView.RPC("ClientInvokedRPC_capture", RpcTarget.MasterClient, _n_troops);
    }






    [PunRPC]
    void ClientInvokedRPC_capture(int _n_troops, PhotonMessageInfo _PhotonMessageInfo)
    {
        if (PrototypingAssets.run_server_code.value == false || _PhotonMessageInfo.Sender != this.my_PhotonView.Owner)
            return;

        RiskySandBox_MainGame.instance.capture(my_Team, _n_troops);
    }



}
