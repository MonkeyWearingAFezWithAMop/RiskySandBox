using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

using Photon.Pun;

public partial class RiskySandBox_HumanPlayer
{



    public RiskySandBox_Tile fortify_target
    {
        get { return PRIVATE_fortify_target; }
        set
        {
            if (this.PRIVATE_fortify_target != null)
                this.PRIVATE_fortify_target.is_fortify_target.value = false;

            this.PRIVATE_fortify_target = value;

            if (this.PRIVATE_fortify_target != null)
                value.is_fortify_target.value = true;

            OnVariableUpdate_fortify_target?.Invoke(this);
        }
    }
    [SerializeField] RiskySandBox_Tile PRIVATE_fortify_target;

    /// <summary>
    /// code that runs when player left clicks in the "fortify" state...
    /// </summary>
    void handleLeftClick_fortify()
    {
        RiskySandBox_Tile _current_Tile = RiskySandBox_CameraControls.current_hovering_Tile;
        if (_current_Tile == null)
            return;

        if (_current_Tile.my_Team == this.my_Team)
        {
            if (this.selected_Tile == null)
            {

                //TODO - make sure there is atleast 1 Tile that can be fortified to...
                //TODO - make sure the player can "cancel" even when there is no fortify_target...

                if (_current_Tile.num_troops > RiskySandBox_Tile.min_troops_per_Tile)
                    this.selected_Tile = _current_Tile;

            }

            else if (_current_Tile != this.selected_Tile && this.fortify_target == null)
            {
                List<int> _path = RiskySandBox_MainGame.CalculateRoute(this.selected_Tile.ID, _current_Tile.ID, this.my_Team);

                if (_path == null)
                {
                    if (this.debugging)
                        GlobalFunctions.print("_path was null... not setting the the fortify_target", this);
                    return;
                }

                this.fortify_target = _current_Tile;
            }

        }
    }


    public void TRY_fortify()
    {
        my_PhotonView.RPC("ClientInvokedRPC_fortify", RpcTarget.MasterClient, (int)selected_Tile.ID, (int)fortify_target.ID, this.slider_value.value);
    }





    [PunRPC]
    void ClientInvokedRPC_fortify(int _from_ID, int _to_ID, int _n_troops, PhotonMessageInfo _PhotonMessageInfo)
    {
        if (PrototypingAssets.run_server_code.value == false || _PhotonMessageInfo.Sender != this.my_PhotonView.Owner)
            return;

        RiskySandBox_Tile _from = RiskySandBox_Tile.GET_RiskySandBox_Tile(_from_ID);
        RiskySandBox_Tile _to = RiskySandBox_Tile.GET_RiskySandBox_Tile(_to_ID);
        bool _fortified = RiskySandBox_MainGame.instance.fortify(my_Team, _from, _to, _n_troops);

        if (_fortified == true)//if we successfully fortified....
            RiskySandBox_MainGame.instance.goToNextTurnState(my_Team);
    }
}
