using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using UnityEngine.EventSystems;

public partial class RiskySandBox_TerritoryCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static string progressive_mode { get { return "Progressive"; } }
    public static string fixed_mode { get { return "Fixed"; } }


    public static readonly int wildcard_ID = 0;



    public static List<RiskySandBox_TerritoryCard> all_instances = new List<RiskySandBox_TerritoryCard>();

    public static List<RiskySandBox_TerritoryCard> all_instances_is_selected { get { return all_instances.Where(x => x.PRIVATE_is_selected.value == true).ToList(); } }

    public static event Action<RiskySandBox_TerritoryCard> OnVariableUpdate_is_selected_STATIC;



    [SerializeField] bool debugging;

    public int tile_ID_READONLY { get { return tile_ID.value; } }

    [SerializeField] ObservableInt tile_ID;

    [SerializeField] Texture2D wildcard_Texture2D;

    [SerializeField] List<Texture2D> background_Texture2Ds = new List<Texture2D>();

    [SerializeField] UnityEngine.UI.RawImage background_Image;

    [SerializeField] UnityEngine.UI.Text tile_name_Text;

    [SerializeField] ObservableBool PRIVATE_is_selected;

    bool is_hovering = false;


    private void Awake()
    {
        this.tile_ID.OnUpdate += delegate { updateVisuals(); };
        all_instances.Add(this);
        this.PRIVATE_is_selected.OnUpdate += OnVariableUpdate_is_selected;

        
        
    }

    private void OnDestroy()
    {
        
        all_instances.Remove(this);
    }

    void Update()
    {
        if (is_hovering == false)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            PRIVATE_is_selected.value = !PRIVATE_is_selected.value;
        }
    }

    void OnVariableUpdate_is_selected(ObservableBool _is_selected)
    {
        OnVariableUpdate_is_selected_STATIC?.Invoke(this);
        if(_is_selected.previous_value == false && _is_selected.value == true)//from false -> true
        {
            //shift up...
            this.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, this.background_Image.GetComponent<RectTransform>().rect.height * 0.2f);
        }
        else if(_is_selected.previous_value == true && _is_selected.value == false)//from true -> false
        {
            //shift down...
            this.GetComponent<RectTransform>().anchoredPosition -= new Vector2(0, this.background_Image.GetComponent<RectTransform>().rect.height * 0.2f);
        }
    }

    void updateVisuals()
    {
        Texture2D _background_Texture2D = wildcard_Texture2D;
        string _tile_name = "";

        if (this.tile_ID != RiskySandBox_Tile.null_ID)
        {
            if (background_Texture2Ds.Count > 0)
            {
                _background_Texture2D = background_Texture2Ds[this.tile_ID.value % background_Texture2Ds.Count];
            }
            _tile_name = RiskySandBox_Tile.GET_RiskySandBox_Tile(this.tile_ID).name;
        }

        background_Image.texture = _background_Texture2D;
        tile_name_Text.text = _tile_name;
    }



    public static RiskySandBox_TerritoryCard createNew(int _tile_ID, Transform _parent)
    {
        GameObject _new = UnityEngine.Object.Instantiate(RiskySandBox_Resources.territory_card,_parent);
        RiskySandBox_TerritoryCard _card = _new.GetComponent<RiskySandBox_TerritoryCard>();
        _card.tile_ID.value = _tile_ID;
        return _card;
    }

    public static void destroyAllCards()
    {
        foreach(RiskySandBox_TerritoryCard _Card in new List<RiskySandBox_TerritoryCard>(RiskySandBox_TerritoryCard.all_instances))
        {
            UnityEngine.Object.Destroy(_Card.gameObject);
        }
    }

    // This method is called when the pointer enters the UI element
    public void OnPointerEnter(PointerEventData eventData)
    {
        is_hovering = true;
    }

    // This method is called when the pointer exits the UI element
    public void OnPointerExit(PointerEventData eventData)
    {
        is_hovering = false;
    }



    public static int drawRandomCard(List<int> _availible_tile_IDs,int _n_wilds)
    {

        if (_n_wilds > 0)
        {
            //e.g. if you had 3 wilds and 2 territory cards...
            int _random_int = GlobalFunctions.randomInt(0, _n_wilds + _availible_tile_IDs.Count - 1);
            //the random int is between 0 and 4
            //0,1,2 represent a wild... 3,4 represent a _availible card...

            if (_random_int < _n_wilds)
                return RiskySandBox_TerritoryCard.wildcard_ID;
        }

        
        if (_availible_tile_IDs.Count() > 0)
        {
            int _random_index = GlobalFunctions.randomInt(0, _availible_tile_IDs.Count - 1);
            return _availible_tile_IDs[_random_index];
        }

        Debug.LogError("this should not have happened!!!");
        return -99999;


    }
}
