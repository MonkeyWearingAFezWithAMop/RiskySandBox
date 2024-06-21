using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_LevelEditor_EditTileBehaviour : MonoBehaviour
{
    [SerializeField] bool debugging;

    [SerializeField] GameObject tile_properties_UI;

    [SerializeField] ObservableBool enable_behaviour;
    [SerializeField] ObservableBool selected_Tile_doesnt_equal_null;

    RiskySandBox_Tile selected_Tile
    {
        get { return PRIVATE_selected_Tile; }
        set
        {
            if (PRIVATE_selected_Tile != null)
                PRIVATE_selected_Tile.show_level_editor_ui.value = false;
            PRIVATE_selected_Tile = value;
            if (value != null)
                value.show_level_editor_ui.value = true;

            selected_Tile_doesnt_equal_null.value = value != null;
        }
    }

    [SerializeField] RiskySandBox_Tile PRIVATE_selected_Tile;

    void EventReceiver_OnVariableUpdate_enable_behaviour(ObservableBool _enable_behaviour)
    {
        if (_enable_behaviour.value == false)
            this.selected_Tile = null;
    }



    private void Awake()
    {
        this.enable_behaviour.OnUpdate += EventReceiver_OnVariableUpdate_enable_behaviour;

        RiskySandBox_LevelEditor.Ondisable += RiskySandBox_LevelEditorEventReceiver_Ondisable;
    }

    void RiskySandBox_LevelEditorEventReceiver_Ondisable()
    {
        this.enable_behaviour.value = false;
        if (this.selected_Tile != null)
            this.selected_Tile = null;
    }



    private void Update()
    {
        if (enable_behaviour == false)
            return;


        if(this.selected_Tile == null)
        {
            //if we click on a tile?
            //select it...
            if(Input.GetMouseButtonDown(0))
            {
                this.selected_Tile = RiskySandBox_CameraControls.current_hovering_Tile;
            }
        }

        if (this.selected_Tile == null)
            return;







    }



}
