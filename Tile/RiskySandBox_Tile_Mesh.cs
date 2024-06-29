using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]

public partial class RiskySandBox_Tile_Mesh : MonoBehaviour
{
    [SerializeField] bool debugging;
    [SerializeField] RiskySandBox_Tile my_Tile;


    MeshFilter my_MeshFilter { get { return GetComponent<MeshFilter>(); } }
    MeshCollider my_MeshCollider { get { return GetComponent<MeshCollider>(); } }

    [SerializeField] ObservableFloat extrusion_height;

    List<Vector3> mesh_points_2D { get { return this.my_Tile.mesh_points_2D.ToList(); } }




    private void Awake()
    {
        RiskySandBox_Tile.CACHE_GET_RiskySandBox_Tile_Colliders[my_MeshCollider] = this.my_Tile;


        my_Tile.mesh_points_2D.OnUpdate += EventReceiver_OnVariableUpdate_mesh_points_2D;
        extrusion_height.OnUpdate += EventReceiver_OnVariableUpdate_extrusion_height;
    }


    private void Start()
    {
        updateMesh();
    }


    void EventReceiver_OnVariableUpdate_mesh_points_2D()
    {
        //update mesh...
        updateMesh();
    }


    void updateMesh()
    {
        if(this.extrusion_height == 0)//if extrusion height == 0?
        {
            my_MeshFilter.mesh = ShapeCreator.createMesh(this.mesh_points_2D);//simply just the 2d case...
        }
        else
        {
            if (this.my_MeshFilter.mesh.vertices.Count() <  2 * this.mesh_points_2D.Count())
                this.my_MeshFilter.mesh = ShapeCreator.createMesh(this.mesh_points_2D, true);

            List<Vector3> _new_verts = new List<Vector3>(this.mesh_points_2D);
            _new_verts.AddRange(_new_verts);

            for (int _i = 0; _i < _new_verts.Count() / 2; _i += 1)
            {
                _new_verts[_i] = new Vector3(_new_verts[_i].x, this.extrusion_height.value, _new_verts[_i].z);
            }
            this.my_MeshFilter.mesh.vertices = _new_verts.ToArray();

        }

        my_MeshCollider.sharedMesh = my_MeshFilter.mesh;

    }

    void EventReceiver_OnVariableUpdate_extrusion_height(ObservableFloat _extrusion_height)
    {
        if (_extrusion_height.delta_value == 0)
            return;

        updateMesh();
    }



}
