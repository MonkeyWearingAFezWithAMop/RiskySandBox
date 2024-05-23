using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Photon.Pun;

public partial class RiskySandBox_HumanPlayer
{


    /// <summary>
    /// code that runs when player left clicks in the "deploy" state...
    /// </summary>
    void handleLeftClick_deploy()
    {
        RiskySandBox_Tile _current_Tile = RiskySandBox_CameraControls.current_hovering_Tile;

        if (_current_Tile == null)
            return;

        if (this.selected_Tile != null)
            return;


        if(_current_Tile.my_Team == this.my_Team)
        {
            this.selected_Tile = _current_Tile;
            return;
        }

        if (RiskySandBox_MainGame.instance.enable_alliances == false)
            return;

        bool _is_ally = this.my_Team.ally_ids.Contains(_current_Tile.my_Team_ID);

        if (_is_ally == false || RiskySandBox_MainGame.instance.allow_deploy_to_ally_Tiles.value == false)
            return;

        this.selected_Tile = _current_Tile;
        
    }


    public void TRY_deploy()
    {
        int _n_Troops = this.slider_value.value;
        RiskySandBox_Tile _Tile = this.selected_Tile;

        if (this.debugging)
            GlobalFunctions.print("asking server to deploy to " + _n_Troops + " to the Tile with ID = " + _Tile.ID, this);


        my_PhotonView.RPC("ClientInvokedRPC_deploy", RpcTarget.MasterClient, (int)_Tile.ID, (int)_n_Troops);//ask server to deploy...

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
