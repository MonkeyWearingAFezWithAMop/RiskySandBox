using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

using Photon.Pun;

public partial class RiskySandBox_HumanPlayer
{






    public void TRY_attack(RiskySandBox_Tile _from,RiskySandBox_Tile _to,int _n_troops)
    {
        //TODO - add if(debuggig) statements...
        if (_from == null)
            return;

        if (_to == null)
            return;

        if (this.my_Team.current_turn_state != RiskySandBox_Team.turn_state_attack)
            return;


        int _from_ID = _from.ID;
        int _to_ID = _to.ID;

        if (this.debugging)
            GlobalFunctions.print("asking server to attack... _from_ID = " + _from_ID + ", _to_ID =  " + _to_ID + ", _n_Troops = " + _n_troops, this);


        my_PhotonView.RPC("ClientInvokedRPC_attack", RpcTarget.MasterClient, (int)_from_ID, (int)_to_ID, _n_troops, "NULLVALUEJUSTIGNOREFORNOW!!!83hvp2y");
    }


    [PunRPC]
    void ClientInvokedRPC_attack(int _from_ID, int _to_ID, int _n_troops, string _attack_method, PhotonMessageInfo _PhotonMessageInfo)
    {
        if (PrototypingAssets.run_server_code.value == false || _PhotonMessageInfo.Sender != this.my_PhotonView.Owner)
            return;

        RiskySandBox_Tile _from = RiskySandBox_Tile.GET_RiskySandBox_Tile(_from_ID);
        RiskySandBox_Tile _to = RiskySandBox_Tile.GET_RiskySandBox_Tile(_to_ID);
        RiskySandBox_MainGame.instance.attack(_from, _to, _n_troops, _attack_method);
    }











}
