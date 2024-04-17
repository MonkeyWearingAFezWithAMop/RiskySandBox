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







    [SerializeField] bool debugging;

    [SerializeField] int PRIVATE_ID = null_ID;
    [SerializeField] ObservableInt PRIVATE_num_troops;
    [SerializeField] ObservableBool PRIVATE_selected;
    [SerializeField] ObservableBool PRIVATE_is_attack_target;
    [SerializeField] ObservableBool PRIVATE_is_fortify_target;



    [SerializeField] UnityEngine.UI.Text PRIVATE_num_troops_Text;



    [SerializeField] MeshRenderer my_MeshRenderer { get { return GetComponent<MeshRenderer>(); } }
    
    


    private void Start()
    {
        updateVisuals();
    }


    private void updateVisuals()
    {
        if(my_MeshRenderer != null)
        {
            Material _Material = null;
            if (my_Team != null)
                _Material = my_Team.my_Material;
            my_MeshRenderer.material = _Material;
        }
        if (PRIVATE_num_troops_Text != null)
            PRIVATE_num_troops_Text.text = "" + this.num_troops;

    }

    public void overrideVisuals(int _num_troops)
    {
        this.PRIVATE_num_troops_Text.text = "" + _num_troops;
    }






    public static void resetVisuals()
    {
        List<RiskySandBox_Tile> _dirty_Tiles = RiskySandBox_Tile.all_instances;
        foreach (RiskySandBox_Tile _Tile in _dirty_Tiles)
        {
            _Tile.updateVisuals();
        }
    }






    public static void destroyAllTiles()
    {
        foreach(RiskySandBox_Tile _Tile in new List<RiskySandBox_Tile>(RiskySandBox_Tile.all_instances))
        {
            UnityEngine.Object.Destroy(_Tile.gameObject, 0f);

        }

        CACHE_GET_RiskySandBox_Tile.Clear();
        CACHE_GET_RiskySandBox_Tile_Colliders.Clear();
    }








}
