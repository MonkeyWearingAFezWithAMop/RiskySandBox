using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

using Photon.Pun;

public partial class RiskySandBox_HumanPlayer
{





 


    public void TRY_fortify(RiskySandBox_Tile _from,RiskySandBox_Tile _to, int _n_troops)
    {
        //TODO - add if(debugging) statemenets...
        if (_from == null)
            return;
        if (_to == null)
            return;

        if (this.my_Team.current_turn_state != RiskySandBox_Team.turn_state_fortify)
            return;


        my_PhotonView.RPC("ClientInvokedRPC_fortify", RpcTarget.MasterClient, (int)_from.ID, (int)_to.ID, (int) _n_troops);
    }





    [PunRPC]
    void ClientInvokedRPC_fortify(int _from_ID, int _to_ID, int _n_troops, PhotonMessageInfo _PhotonMessageInfo)
    {
        if (PrototypingAssets.run_server_code.value == false || _PhotonMessageInfo.Sender != this.my_PhotonView.Owner)
            return;

       

        RiskySandBox_Tile _from = RiskySandBox_Tile.GET_RiskySandBox_Tile(_from_ID);

        if(_from.my_Team != this.my_Team)
        {
            return;
        }


        RiskySandBox_Tile _to = RiskySandBox_Tile.GET_RiskySandBox_Tile(_to_ID);
        bool _fortified = RiskySandBox_MainGame.instance.fortify(_from, _to, _n_troops);

        if (_fortified == true)//if we successfully fortified....
            RiskySandBox_MainGame.instance.goToNextTurnState(my_Team);
    }
}
