using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_LevelEditor_GraphMode : MonoBehaviour
{
    [SerializeField] bool debugging;
    [SerializeField] ObservableBool enable_behaviour;


    RiskySandBox_Tile selected_Tile
    {
        get { return PRIVATE_selected_Tile; }
        set
        {
            PRIVATE_selected_Tile = value;
            updateTileMaterials();
        }
    }
    [SerializeField] private RiskySandBox_Tile PRIVATE_selected_Tile;


    private void Awake()
    {
        this.enable_behaviour.OnUpdate += EventReceiver_OnVariableUpdate_enable_behaviour;
    }


    void EventReceiver_OnVariableUpdate_enable_behaviour(ObservableBool _enable_behaviour)
    {
        if(_enable_behaviour == false)
        {
            this.selected_Tile = null;
        }
    }

    private void Update()
    {
        if (this.enable_behaviour == false)
            return;

        RiskySandBox_Tile _current_Tile = RiskySandBox_CameraControls.current_hovering_Tile;

        //if we click on  a tile?
        if (this.selected_Tile != null)
        {
            if(Input.GetKeyDown(KeyCode.Space))//if we press the "space" key
            {
                


                if(_current_Tile != null && _current_Tile != this.selected_Tile)
                {
                    //toggle the connection state!
                    if (this.selected_Tile.graph_connections.Contains(_current_Tile.ID))
                        this.selected_Tile.graph_connections.Remove(_current_Tile.ID);
                    else
                        this.selected_Tile.graph_connections.Add(_current_Tile.ID);

                    updateTileMaterials();

                }


            }
        }
        
            

        if(Input.GetMouseButtonDown(0) == true)//if left click...
        {
            //select that tile...
            
            this.selected_Tile = _current_Tile;
        }
        


    }

    void updateTileMaterials()
    {
        foreach(RiskySandBox_Tile _Tile in RiskySandBox_Tile.all_instances)
        {
            _Tile.my_LevelEditor_Material = null;
        }

        if (this.selected_Tile == null)
            return;

        this.selected_Tile.GetComponent<MeshRenderer>().material = PrototypingAssets_Materials.blue;
        foreach(int _connection in this.selected_Tile.graph_connections)
        {
            RiskySandBox_Tile _connection_Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(_connection);
            if(_connection_Tile != null)
                _connection_Tile.my_LevelEditor_Material = PrototypingAssets_Materials.red;
            else
            {
                //the tile has not been created? or has been deleted in some way? this is kinda an error so we need to somehow tell the user this is going wrong?
            }
        }

    }
}
