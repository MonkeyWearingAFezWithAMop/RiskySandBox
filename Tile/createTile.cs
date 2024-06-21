using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_Tile
{
    public static event Action<RiskySandBox_Tile> OncreateTile;




    public static RiskySandBox_Tile createTile(int _id, Vector3 _centre, Quaternion _rotation, Vector3 _scale, List<Vector3> _mesh_verts, List<int> _mesh_tris)
    {
        GameObject _GameObject = UnityEngine.Object.Instantiate(RiskySandBox_Resources.tile_prefab,RiskySandBox_MainGame.instance.tile_parent_Transform);

        RiskySandBox_Tile _new_Tile_script = _GameObject.GetComponent<RiskySandBox_Tile>();

        //make sure there isnt a tile with that id...
        _new_Tile_script.ID.value = _id;


        //create the mesh for this tile so players can see / interact with the Tile...

        Mesh _new_Mesh = new Mesh();
        _new_Mesh.vertices = _mesh_verts.ToArray();
        _new_Mesh.triangles = _mesh_tris.ToArray();
        _new_Mesh.RecalculateNormals();

        MeshFilter _MeshFilter = _GameObject.GetComponent<MeshFilter>();
        _MeshFilter.sharedMesh = _new_Mesh;

        MeshCollider _Collider = _GameObject.GetComponent<MeshCollider>();
        _Collider.sharedMesh = _new_Mesh;

        _new_Tile_script.mesh_points_2D = _mesh_verts.ToList();

        _GameObject.transform.position = _centre;
        _GameObject.transform.rotation = _rotation;
        _GameObject.transform.localScale = _scale;

        



        OncreateTile?.Invoke(_new_Tile_script);

        return _new_Tile_script;




    }









}
