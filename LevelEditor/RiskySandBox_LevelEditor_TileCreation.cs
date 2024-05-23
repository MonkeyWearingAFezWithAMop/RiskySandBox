using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_LevelEditor_TileCreation : MonoBehaviour
{
    [SerializeField] bool debugging;

    [SerializeField] ObservableBool enable_behaviour;



    [SerializeField] ShapeCreator my_ShapeCreator;

    [SerializeField] ObservableFloat handle_radius;

    [SerializeField] GameObject ui_root;
    [SerializeField] LineRenderer my_LineRenderer;


    [SerializeField] ObservableBool PRIVATE_enable_Mesh;
    [SerializeField] ObservableBool PRIVATE_enable_LineRenderer;




    public ObservableFloat tile_ui_scale_factor;



    



    private void Awake()
    {

        this.PRIVATE_enable_LineRenderer.OnUpdate += delegate { if (this.my_LineRenderer != null) this.my_LineRenderer.enabled = this.PRIVATE_enable_LineRenderer.value; };
        this.PRIVATE_enable_Mesh.OnUpdate += delegate
        {
            if (this.my_ShapeCreator != null && this.my_ShapeCreator.GetComponent<MeshRenderer>() != null)
            {
                this.my_ShapeCreator.GetComponent<MeshRenderer>().enabled = this.PRIVATE_enable_Mesh;
            }
             
        };
        this.handle_radius.OnUpdate += delegate { fullUpdate(); };

        RiskySandBox_LevelEditorHandle.all_instances.OnUpdate += delegate { fullUpdate(); };
        RiskySandBox_LevelEditorHandle.OnUpdate_position_STATIC += delegate { fullUpdate(); };
    }









    public void fullUpdate()
    {
        this.my_ShapeCreator.shapes[0].points.Clear();

        if (this.enable_behaviour == false)
            return;

        foreach (RiskySandBox_LevelEditorHandle _Handle in RiskySandBox_LevelEditorHandle.all_instances.Where(x => x != null))
        {
            this.my_ShapeCreator.shapes[0].points.Add(_Handle.transform.position);
            _Handle.transform.localScale = new Vector3(this.handle_radius, this.handle_radius, this.handle_radius);
        }

        try
        {
            my_ShapeCreator.UpdateMeshDisplay();//DO NOT LET THE PROGRAM CRASH if this goes wrong...
        }
        catch
        {

        }

        this.my_LineRenderer.startWidth = this.handle_radius;
        this.my_LineRenderer.endWidth = this.handle_radius;

        this.my_LineRenderer.positionCount = this.my_ShapeCreator.shapes[0].points.Count;
        this.my_LineRenderer.SetPositions(this.my_ShapeCreator.shapes[0].points.ToArray());

    }







    // Update is called once per frame
    void Update()
    {
        if (this.enable_behaviour == false)
        {
            if (this.debugging)
                GlobalFunctions.print("enable behaviour is false... returning",this);
            return;
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            if (this.debugging)
                GlobalFunctions.print("detected a 'c' key press - creating a new Tile! (and exiting draw mode...)",this);


            Mesh _Mesh = my_ShapeCreator.my_MeshFilter.mesh;

            //make sure the mesh has atleast 3? points otherwise generating a mesh is impossible...
            if(_Mesh.vertices.Count() >= 3)
            {
                int _creation_ID = 1;
                if (RiskySandBox_Tile.all_instances.Count > 0)
                    _creation_ID = RiskySandBox_Tile.all_instances.Max(x => x.ID.value) + 1;//give a unique id to the tile...
                

                RiskySandBox_Tile _Tile = RiskySandBox_Tile.createTile(_creation_ID, new Vector3(0, 0, 0), Quaternion.identity, new Vector3(1, 1, 1), _Mesh.vertices.ToList(), _Mesh.triangles.ToList());
                
                _Tile.UI_scale_factor.value = this.tile_ui_scale_factor;
                _Tile.UI_position = new Vector3(_Mesh.vertices.Sum(v => v.x) / _Mesh.vertices.Count(), 0, _Mesh.vertices.Sum(v => v.z) / _Mesh.vertices.Count());

            }

            RiskySandBox_LevelEditorHandle.destroyAllHandles();
            this.enable_behaviour.value = false;
        }

        if(Input.GetKeyDown(KeyCode.M))
        {
            if (this.debugging)
                GlobalFunctions.print("'m' key pressed! - inverting PRIVATE_enable_Mesh...",this);
            this.PRIVATE_enable_Mesh.value = !this.PRIVATE_enable_Mesh;
        }
        if(Input.GetKeyDown(KeyCode.L))
        {
            if (this.debugging)
                GlobalFunctions.print("'l' key pressed! - inverting PRIVATE_enable_Mesh...", this);
            this.PRIVATE_enable_LineRenderer.value = !this.PRIVATE_enable_LineRenderer;
        }

        RiskySandBox_Tile _current_Tile = RiskySandBox_CameraControls.current_hovering_Tile;

        if (_current_Tile != null && Input.GetKeyDown(KeyCode.F))//if we are hovering over a tile and hit the 'f' button...
        {
            if (this.debugging)
                GlobalFunctions.print("'f' key pressed - placing the ui onto the tile with id" + _current_Tile.ID.value,this);
            _current_Tile.UI_position = RiskySandBox_CameraControls.current_hit_point;//place the ui for the tile at the hit point
            _current_Tile.UI_scale_factor.value = this.tile_ui_scale_factor;//update the scale factor for the ui...

        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (this.debugging)
                GlobalFunctions.print("'space' key pressed - creating a new handle!",this);
            RiskySandBox_LevelEditorHandle.createHandle(RiskySandBox_CameraControls.mouse_position,this.handle_radius);
        }

        






        



    }



}
