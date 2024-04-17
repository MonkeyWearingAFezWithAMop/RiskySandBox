using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_Tile
{
    public static event Action<RiskySandBox_Tile> OncreateTile;




    public static RiskySandBox_Tile createTile(int _id, Vector3 _centre, Quaternion _rotation, Vector3 _scale, List<Vector3> _mesh_verts, List<int> _mesh_tris)
    {
        GameObject _GameObject = UnityEngine.Object.Instantiate(RiskySandBox_Resources.tile_prefab,RiskySandBox_MainGame.instance.tile_parent_Transform);

        RiskySandBox_Tile _new_Tile_script = _GameObject.GetComponent<RiskySandBox_Tile>();

        _new_Tile_script.ID = _id;

        RiskySandBox_Tile.CACHE_GET_RiskySandBox_Tile[_id] = _new_Tile_script;



        //create the mesh for this tile so players can see / interact with the Tile...

        Mesh _new_Mesh = new Mesh();
        _new_Mesh.vertices = _mesh_verts.ToArray();
        _new_Mesh.triangles = _mesh_tris.ToArray();

        MeshFilter _MeshFilter = _GameObject.GetComponent<MeshFilter>();
        _MeshFilter.sharedMesh = _new_Mesh;

        MeshCollider _Collider = _GameObject.GetComponent<MeshCollider>();
        _Collider.sharedMesh = _new_Mesh;


        RiskySandBox_Tile.CACHE_GET_RiskySandBox_Tile_Colliders[_Collider] = _new_Tile_script;





        _GameObject.transform.position = _centre;
        _GameObject.transform.rotation = _rotation;
        _GameObject.transform.localScale = _scale;

        _GameObject.name = "RiskySandBox_Tile - ID = " + _id;



        OncreateTile?.Invoke(_new_Tile_script);

        return _new_Tile_script;




    }









}
