using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;


public partial class RiskySandBox_Tile : MonoBehaviour
{

    static Dictionary<Collider, RiskySandBox_Tile> CACHE_GET_RiskySandBox_Tile_Colliders = new Dictionary<Collider, RiskySandBox_Tile>();
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

    public Material my_Bonus_Material
    {
        get { return this.PRIVATE_my_Bonus_Material; }
        set
        {
            this.PRIVATE_my_Bonus_Material = value;
            updateVisuals();
        }
    }

    [SerializeField] Material PRIVATE_my_Bonus_Material;



    public ObservableFloat UI_scale_factor { get { return this.PRIVATE_ui_scale_factor; } }



    public Vector3 UI_position
    {
        get { return PRIVATE_ui_position; }
        set
        {
            this.PRIVATE_ui_position = value;
            ui_Canvas.transform.localPosition = UI_position + new Vector3(0,0.001f,0);
        }
    }
    [SerializeField] Vector3 PRIVATE_ui_position;



    [SerializeField] RectTransform ui_Canvas;

    [SerializeField] UnityEngine.UI.Text PRIVATE_ID_Text;





    [SerializeField] bool debugging;

    [SerializeField] ObservableInt PRIVATE_ID;
    [SerializeField] ObservableInt PRIVATE_num_troops;
    [SerializeField] ObservableBool PRIVATE_selected;
    [SerializeField] ObservableBool PRIVATE_is_attack_target;
    [SerializeField] ObservableBool PRIVATE_is_fortify_target;
    [SerializeField] ObservableBool PRIVATE_show_level_editor_ui;
    [SerializeField] ObservableInt PRIVATE_my_Team_ID;
    [SerializeField] ObservableFloat PRIVATE_ui_scale_factor;
    [SerializeField] ObservableBool PRIVATE_has_capital;//is there a "capital" on this tile?
    [SerializeField] ObservableVector3 PRIVATE_capital_icon_local_position;
    [SerializeField] ObservableFloat PRIVATE_capital_icon_local_scale_factor;


    [SerializeField] UnityEngine.UI.Text PRIVATE_num_troops_Text;



    [SerializeField] MeshRenderer my_MeshRenderer { get { return GetComponent<MeshRenderer>(); } }


    [SerializeField] GameObject PRIVATE_has_capital_icon;


    private void OnEnable()
    {
        Collider _my_Collider = GetComponent<MeshCollider>();
        CACHE_GET_RiskySandBox_Tile_Colliders[_my_Collider] = this;

        this.my_Team_ID.OnUpdate += delegate { updateVisuals(); };
        this.PRIVATE_has_capital.OnUpdate += delegate { updateVisuals(); };


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
    }


    private void Start()
    {
        updateVisuals();
    }


    private void updateVisuals()
    {
        if (this.debugging)
            GlobalFunctions.print("updating visuals! my_Team is "+this.my_Team,this);


        Material _MeshRenderer_Material = null;
        if (RiskySandBox_LevelEditor.is_enabled)//if we are in LevelEditor mode...
        {
            _MeshRenderer_Material = this.my_LevelEditor_Material;
        }

        else if (my_Bonus_Material != null)
            _MeshRenderer_Material = my_Bonus_Material;

        else if (my_Team != null)
            _MeshRenderer_Material = my_Team.my_Material;


        if(my_MeshRenderer != null)
            my_MeshRenderer.material = _MeshRenderer_Material;
            


       

        if (my_Team != null)
            this.PRIVATE_num_troops_Text.color = my_Team.complementary_Color;
        else
            this.PRIVATE_num_troops_Text.color = Color.black;

        PRIVATE_num_troops_Text.text = "" + this.num_troops;
        PRIVATE_num_troops_Text.gameObject.SetActive(RiskySandBox_LevelEditor.is_enabled == false);
        bool _enable_capital_icon = this.has_capital.value == true || (RiskySandBox_LevelEditor.is_editing_bonus == false && RiskySandBox_LevelEditor.is_enabled);

        PRIVATE_has_capital_icon.gameObject.SetActive(_enable_capital_icon);
        PRIVATE_ID_Text.gameObject.SetActive(RiskySandBox_LevelEditor.is_enabled);

           


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
