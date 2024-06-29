using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_Tile
{
    public static event Action<RiskySandBox_Tile> OncreateTile;




    public static RiskySandBox_Tile createTile(int _id, IEnumerable<Vector3> _mesh_verts)
    {
        //for now... only the server can create a tile...



        if (PrototypingAssets.run_server_code.value == false)
        {
            GlobalFunctions.printError("ONLY THE SERVER CAN CREATE TILES (for now)", null);
            return null;
        }


        GameObject _GameObject = Photon.Pun.PhotonNetwork.InstantiateRoomObject(RiskySandBox_Resources.tile_prefab.name, new Vector3(0, 0, 0), Quaternion.identity);


        //GameObject _GameObject = UnityEngine.Object.Instantiate(RiskySandBox_Resources.tile_prefab,RiskySandBox_MainGame.instance.tile_parent_Transform);

        RiskySandBox_Tile _new_Tile_script = _GameObject.GetComponent<RiskySandBox_Tile>();

        //TODO - make sure there isnt a tile with that id...
        _new_Tile_script.ID.value = _id;



        _new_Tile_script.mesh_points_2D.AddRange(_mesh_verts.ToList());



        



        OncreateTile?.Invoke(_new_Tile_script);

        return _new_Tile_script;




    }









}
