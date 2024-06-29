using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public partial class RiskySandBox_Tile_MeshSync : MonoBehaviourPunCallbacks
{
    [SerializeField] bool debugging;


    [SerializeField] PhotonView my_PhotonView;

    [SerializeField] RiskySandBox_Tile my_Tile;


    private void Awake()
    {
        my_Tile.mesh_points_2D.OnUpdate += EventReceiver_OnUpdate_mesh_points;
    }



    void EventReceiver_OnUpdate_mesh_points()
    {
        if (PrototypingAssets.run_server_code.value == false)
            return;
        syncMesh();
    }

    void syncMesh()
    {
        if (PrototypingAssets.run_server_code.value == false)
            return;
        my_PhotonView.RPC("ServerInvokedRPC_receiveMeshData", RpcTarget.Others, my_Tile.mesh_points_2D.ToArray());//TODO - sent this as a string (with , and | as seperators...)
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        if (PrototypingAssets.run_server_code.value == false)
            return;
        syncMesh();
    }


    [PunRPC]
    void ServerInvokedRPC_receiveMeshData(Vector3[] _mesh_points,PhotonMessageInfo _PhotonMessageInfo)
    {
        if (_PhotonMessageInfo.Sender.IsMasterClient == false)
            return;
        //TODO - clear existing data????
        my_Tile.mesh_points_2D.AddRange(_mesh_points);
    }

}
