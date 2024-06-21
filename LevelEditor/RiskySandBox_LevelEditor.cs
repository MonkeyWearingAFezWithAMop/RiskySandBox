using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using System.IO;

public partial class RiskySandBox_LevelEditor : MonoBehaviour
{
    public static bool is_enabled;
    public static ObservableBool is_editing_bonus { get { return instance.PRIVATE_is_editing_bonus; } }
    public static ObservableBool is_graph_mode { get { return instance.PRIVATE_is_graph_mode; } }

    public static RiskySandBox_LevelEditor instance;


    [SerializeField] bool debugging;


    [SerializeField] ObservableBool PRIVATE_is_editing_bonus;
    [SerializeField] ObservableBool PRIVATE_is_graph_mode;

    [SerializeField] bool block_Update = false;



    [SerializeField] GameObject ui_root;



    [SerializeField] UnityEngine.UI.RawImage backround_Image;


    public static event Action Onenable;
    public static event Action Ondisable;


    [SerializeField] ObservableBool show_escape_menu;






    public void save()
    {
        //ask the map to save to the current time stamp...

        DateTime currentTime = DateTime.Now;

        string _map_name = string.Format("LevelEditorOutput_{0}_{1}_{2}_{3}_{4}_{5}", currentTime.Day, currentTime.Month, currentTime.Year, currentTime.Hour, currentTime.Minute, currentTime.Second);
        _map_name = RiskySandBox_MainGame.instance.map_ID;
        
        RiskySandBox_MainGame.instance.saveMap(_map_name);
        
    }


    public void load()
    {
        RiskySandBox_MainGame.instance.loadMap(RiskySandBox_MainGame.instance.map_ID);
    }


    


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        disable();
    }

    









    private void Update()
    {
        if (block_Update == true)
            return;



        RiskySandBox_Tile _current_Tile = RiskySandBox_CameraControls.current_hovering_Tile;


   
        //if the "delete_tile" observablebool is true... why because accidently deleting a tile would be really annoying...

        if(_current_Tile != null && Input.GetKeyDown(KeyCode.P))
        {
            if (this.debugging)
                GlobalFunctions.print("p key pressed - deleting a tile!",this);
            //lets delete the tile...
            RiskySandBox_Tile.destroyTile(_current_Tile);
            
        }

        if(Input.GetKeyDown(KeyCode.Escape) == true)
        {
            if (this.debugging)
                GlobalFunctions.print("escape key pressed - opening escape menu!",this);
            //open the escape menu...
            this.show_escape_menu.value = !this.show_escape_menu;
        }
        



    }

    public void enable()
    {
        ui_root.SetActive(true);
        block_Update = false;

        RiskySandBox_LevelEditor.is_enabled = true;
        Onenable?.Invoke();
    }


    public void disable()
    {
        ui_root.SetActive(false);
        block_Update = true;
        RiskySandBox_LevelEditor.is_enabled = false;
        Ondisable?.Invoke();

    }



}
