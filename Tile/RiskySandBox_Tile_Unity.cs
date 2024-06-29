using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;


public partial class RiskySandBox_Tile : MonoBehaviour
{

    public static Dictionary<Collider, RiskySandBox_Tile> CACHE_GET_RiskySandBox_Tile_Colliders = new Dictionary<Collider, RiskySandBox_Tile>();
    public static RiskySandBox_Tile GET_RiskySandBox_Tile(Collider _Collider)
    {
        RiskySandBox_Tile _return_value = null;
        RiskySandBox_Tile.CACHE_GET_RiskySandBox_Tile_Colliders.TryGetValue(_Collider, out _return_value);
        return _return_value;
    }



    public Material my_LevelEditor_Material
    {
        get { return this.PRIVATE_my_LevelEditor_Material; }
        set
        {
            this.PRIVATE_my_LevelEditor_Material = value;
            updateVisuals();
        }
    }
    [SerializeField] Material PRIVATE_my_LevelEditor_Material;

    //ok so this is needed for extruding the mesh 
    public ObservableList<Vector3> mesh_points_2D = new ObservableList<Vector3>();





    public ObservableFloat UI_scale_factor { get { return this.PRIVATE_ui_scale_factor; } }



    public ObservableVector3 UI_position {get { return this.PRIVATE_ui_position; }}

    [SerializeField] ObservableVector3 PRIVATE_ui_position;



    [SerializeField] RectTransform ui_Canvas;

    [SerializeField] UnityEngine.UI.Text PRIVATE_ID_Text;


    public ObservableFloat extrusion_height { get { return this.PRIVATE_extrusion_height; } }
    [SerializeField] ObservableFloat PRIVATE_extrusion_height;




    [SerializeField] bool debugging;

    [SerializeField] ObservableInt PRIVATE_ID;
    [SerializeField] ObservableInt PRIVATE_num_troops;

    [SerializeField] ObservableBool PRIVATE_is_deploy_target;

    [SerializeField] ObservableBool PRIVATE_is_attack_start;
    [SerializeField] ObservableBool PRIVATE_is_attack_target;

    [SerializeField] ObservableBool PRIVATE_is_fortify_start;
    [SerializeField] ObservableBool PRIVATE_is_fortify_target;


    [SerializeField] ObservableBool PRIVATE_show_level_editor_ui;
    [SerializeField] ObservableInt PRIVATE_my_Team_ID;
    [SerializeField] ObservableFloat PRIVATE_ui_scale_factor;
    [SerializeField] ObservableBool PRIVATE_has_capital;//is there a "capital" on this tile?
    [SerializeField] ObservableVector3 PRIVATE_capital_icon_local_position;
    [SerializeField] ObservableFloat PRIVATE_capital_icon_local_scale_factor;
    [SerializeField] ObservableBool PRIVATE_has_stable_portal;
    [SerializeField] ObservableBool PRIVATE_has_blizard;
    [SerializeField] ObservableBool PRIVATE_has_unstable_portal;
    [SerializeField] ObservableString PRIVATE_name;


    [SerializeField] UnityEngine.UI.Text PRIVATE_num_troops_Text;



    [SerializeField] MeshRenderer my_MeshRenderer;


    [SerializeField] GameObject PRIVATE_has_capital_icon;

    public Material my_Material;


    private void OnEnable()
    {
        my_Material = new Material(Shader.Find("Standard"));


        this.my_Team_ID.OnUpdate += delegate { updateVisuals(); };
        this.PRIVATE_has_capital.OnUpdate += delegate { updateVisuals(); };
        this.PRIVATE_ID.OnUpdate += delegate { this.gameObject.name = "RiskySandBox_Tile ID = " + this.PRIVATE_ID.value; };

        this.extrusion_height.OnUpdate += delegate { updateVisuals(); };



        RiskySandBox_MainGame.instance.display_bonuses.OnUpdate += EventReceiver_OnVariableUpdate_display_bonuses;
        RiskySandBox_MainGame.OnsaveMap += EventReceiver_OnsaveMap;
       
        RiskySandBox_Tile.all_instances.Add(this);

    }





    private void OnDisable()
    {
        Collider _my_Collider = GetComponent<MeshCollider>();
        CACHE_GET_RiskySandBox_Tile_Colliders.Remove(_my_Collider);

        RiskySandBox_Tile.all_instances.Remove(this);
        if (RiskySandBox_Tile.CACHE_GET_RiskySandBox_Tile.TryGetValue(this.ID,out RiskySandBox_Tile _Tile))
        {
            if(_Tile == this)
                RiskySandBox_Tile.CACHE_GET_RiskySandBox_Tile.Remove(this.ID);
        }

        RiskySandBox_MainGame.instance.display_bonuses.OnUpdate -= EventReceiver_OnVariableUpdate_display_bonuses;
        RiskySandBox_MainGame.OnsaveMap -= EventReceiver_OnsaveMap;
    }

    void EventReceiver_OnsaveMap(string _directory)
    {
        string _file = System.IO.Path.Combine(_directory, string.Format("Tile_{0}.txt", this.ID.value));

        System.IO.StreamWriter _writer = new System.IO.StreamWriter(_file);

        //TODO - allow multiple meshes by adding in a '|' character???? s- might be nice for visual settings? for a tile e.g. its 2 small islands connected to each other  (japan has 2 islands and you may wish to represent as 2 seperate meshes)

        _writer.WriteLine("verts:"+string.Join(",", this.mesh_points_2D.Select(v => string.Format("{0},{1},{2}",v.x,v.y,v.z))));

        _writer.WriteLine("ID:" + this.ID);
        _writer.WriteLine(save_load_key_num_troops + ":" + this.num_troops.value);
        _writer.WriteLine("team:" + this.my_Team_ID.value);

        _writer.WriteLine(string.Format("position:{0},{1},{2}", this.transform.position.x, this.transform.position.y, this.transform.position.z));//useful for "minor" adjustments to the tiles location...
        _writer.WriteLine("UI_scale_factor:" + this.UI_scale_factor);
        _writer.WriteLine("UI_position:" + this.UI_position.x + "," + this.UI_position.y + "," + this.UI_position.z);

        _writer.WriteLine("capital_icon_local_position:" + this.capital_icon_local_position.x + "," + this.capital_icon_local_position.y + "," + this.capital_icon_local_position.z);
        _writer.WriteLine("capital_icon_scale_factor:" + this.capital_icon_scale_factor.value);

        _writer.Close();
    }





    private void Start()
    {
        updateVisuals();
    }


    void EventReceiver_OnVariableUpdate_display_bonuses(ObservableBool _display_bonuses)
    {
        updateVisuals();
    }




    public void updateVisuals()
    {
        if (this.debugging)
            GlobalFunctions.print("updating visuals! my_Team is "+this.my_Team,this);


        if (RiskySandBox_LevelEditor.is_enabled)//if we are in LevelEditor mode...
        {
            my_MeshRenderer.material = this.my_LevelEditor_Material;
        }

        else
        {
            //ok this is much much more complicated....
            //so step one lets first set the meshrenderer material to match my_Material...
            my_MeshRenderer.material = my_Material;

            if (my_Team != null)
            {
                my_Material.color = my_Team.my_Material.color;
            }

            //ok now we have a problem because it depends on several settings...

            
            if(RiskySandBox_MainGame.instance.display_bonuses == true)
            {
                //ok! lets set the material color match the color of that bonus...
                RiskySandBox_Bonus _my_Bonus = RiskySandBox_Bonus.GET_RiskySandBox_Bonus(this);

                if(_my_Bonus != null)
                {
                    my_Material.color = _my_Bonus.my_Color;
                }

            }

        }


            

        if (my_Team != null)
            this.PRIVATE_num_troops_Text.color = my_Team.text_Color;
        else
            this.PRIVATE_num_troops_Text.color = Color.black;

        PRIVATE_num_troops_Text.gameObject.SetActive(RiskySandBox_LevelEditor.is_enabled == false);

        bool _enable_capital_icon = this.has_capital.value == true || (RiskySandBox_LevelEditor.is_editing_bonus == false && RiskySandBox_LevelEditor.is_enabled && RiskySandBox_LevelEditor.is_graph_mode == false);

        PRIVATE_has_capital_icon.gameObject.SetActive(_enable_capital_icon);
        PRIVATE_ID_Text.gameObject.SetActive(RiskySandBox_LevelEditor.is_enabled);

        this.PRIVATE_ui_position.value = new Vector3(this.PRIVATE_ui_position.x, this.extrusion_height + 0.001f, this.PRIVATE_ui_position.z);
           


    }


    public static void destroyTile(RiskySandBox_Tile _Tile)
    {
        UnityEngine.Object.Destroy(_Tile.gameObject);
    }



    public static void destroyAllTiles()
    {
        foreach(RiskySandBox_Tile _Tile in new List<RiskySandBox_Tile>(RiskySandBox_Tile.all_instances))
        {
            UnityEngine.Object.Destroy(_Tile.gameObject, 0f);

        }
    }








}
