using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Photon.Pun;

public partial class RiskySandBox_HumanPlayer
{
    public void TRY_capture()
    {
        int _n_troops = this.slider_value.value;
        my_PhotonView.RPC("ClientInvokedRPC_capture", RpcTarget.MasterClient, _n_troops);
    }






    [PunRPC]
    void ClientInvokedRPC_capture(int _n_troops, PhotonMessageInfo _PhotonMessageInfo)
    {
        if (PrototypingAssets.run_server_code.value == false || _PhotonMessageInfo.Sender != this.my_PhotonView.Owner)
            return;

        RiskySandBox_MainGame.instance.capture(my_Team, _n_troops);
    }


    void updateCaptureSlider()
    {
        if (this.my_Team.current_turn_state != RiskySandBox_Team.turn_state_capture) //if we aren't in the caputre state????
            return;

       int _new_slider_value = 0;

        if (this.my_Team.capture_target != null)
        {
            if (this.my_Team.capture_start != null)
            {
                _new_slider_value = this.my_Team.capture_start.num_troops - RiskySandBox_Tile.min_troops_per_Tile;
            }
        }


        this.slider_value.value = _new_slider_value;


    }


    void EventReceiver_OnVariableUpdate_capture_end_ID_STATIC(RiskySandBox_Team _Team)
    {
        if (_Team != my_Team)
            return;

        updateCaptureSlider();
    }

}
