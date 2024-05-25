using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

using Photon.Pun;

public partial class RiskySandBox_HumanPlayer
{


    public RiskySandBox_Tile attack_target
    {
        get { return PRIVATE_attack_target; }
        set
        {
            if (this.PRIVATE_attack_target != null)
                PRIVATE_attack_target.is_attack_target.value = false;


            //do some basic checks...

            if (value == null)
            {
                PRIVATE_attack_target = null;
                OnVariableUpdate_attack_target?.Invoke(this);
                return;
            }



            if (value.my_Team == this.my_Team)
                return;

            if(this.selected_Tile != null)
            {
                //make sure there is a connection between selected_tile and value
                if (selected_Tile.graph_connections.Contains(value.ID) == false)
                    return;
            }


            this.PRIVATE_attack_target = value;

            if (value != null)
                value.is_attack_target.value = true;

            OnVariableUpdate_attack_target?.Invoke(this);

        }
    }
    [SerializeField] RiskySandBox_Tile PRIVATE_attack_target;



    /// <summary>
    /// code that runs when player left clicks in the "attack" state...
    /// </summary>
    void handleLeftClick_attack()
    {
        RiskySandBox_Tile _current_Tile = RiskySandBox_CameraControls.current_hovering_Tile;
        if (_current_Tile == null)
            return;

        if (_current_Tile.my_Team == this.my_Team)
        {
            if (this.attack_target == null)
                if (_current_Tile.num_troops > RiskySandBox_Tile.min_troops_per_Tile)
                    this.selected_Tile = _current_Tile;
        }
        else
        {
            if (this.selected_Tile != null)
            {
                if (this.attack_target == null)
                {
                    if (selected_Tile.graph_connections.Contains(_current_Tile.ID))
                        this.attack_target = _current_Tile;
                }

            }
        }
    }


    public void TRY_attack()
    {
        int _n_troops = this.slider_value.value;

        int _from_ID = selected_Tile.ID;
        int _to_ID = attack_target.ID;

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
        RiskySandBox_MainGame.instance.attack(my_Team, _from, _to, _n_troops, _attack_method);
    }











}
