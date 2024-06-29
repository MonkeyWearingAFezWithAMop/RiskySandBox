using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_Bonus : MonoBehaviour
{
    public static ObservableList<RiskySandBox_Bonus> all_instances = new ObservableList<RiskySandBox_Bonus>();



    [SerializeField] bool debugging;

    [SerializeField] ObservableFloat PRIVATE_border_width;

    [SerializeField] List<LineRenderer> border_LineRenderers = new List<LineRenderer>();

    [SerializeField] LineRenderer current_LineRenderer;



    public ObservableInt generation { get { return PRIVATE_generation; } }
    [SerializeField] ObservableInt PRIVATE_generation;

    [SerializeField] ObservableString PRIVATE_name;//the name of the bonus ("north america" or "south america" etc)

    public ObservableList<int> tile_IDs = new ObservableList<int>();

    public ObservableBool show_level_editor_ui { get { return PRIVATE_show_level_editor_ui; } }
    [SerializeField] ObservableBool PRIVATE_show_level_editor_ui;


    new public ObservableString name { get { return PRIVATE_name; } }


    public Color my_Color { get { return new Color(my_Color_r / 255f, my_Color_g / 255f, my_Color_b / 255f); } }
    [SerializeField] ObservableInt my_Color_r;
    [SerializeField] ObservableInt my_Color_g;
    [SerializeField] ObservableInt my_Color_b;



    [SerializeField] ObservableVector3 ui_scale;
    [SerializeField] ObservableVector3 ui_position;



    



    private void Awake()
    {
        all_instances.Add(this);

        RiskySandBox_LevelEditorHandle.all_instances.OnUpdate += LevelEditorHandleEventReceiver_OnListUpdate_all_instances;

        this.PRIVATE_border_width.OnUpdate += delegate
        {
            float _value = PRIVATE_border_width;
            foreach (LineRenderer _LineRenderer in this.border_LineRenderers)
            {
                _LineRenderer.startWidth = _value;
                _LineRenderer.endWidth = _value;
            }
            foreach (RiskySandBox_LevelEditorHandle _handle in RiskySandBox_LevelEditorHandle.all_instances)
            {
                _handle.transform.localScale = new Vector3(_value, _value, _value);
            }
        };

        this.name.OnUpdate += delegate { this.gameObject.name = "RiskySandBox_Bonus: '" + this.name.value+"'"; };


        this.tile_IDs.OnUpdate += updateVisuals;

        this.show_level_editor_ui.OnUpdate += delegate { updateVisuals(); };

        RiskySandBox_MainGame.OnSET_my_Team += EventReceiver_OnSET_my_Team;
        RiskySandBox_MainGame.OnsaveMap += EventReceiver_OnsaveMap;



    }


    private void OnDestroy()
    {
        RiskySandBox_LevelEditorHandle.all_instances.OnUpdate -= LevelEditorHandleEventReceiver_OnListUpdate_all_instances;
        all_instances.Remove(this);

        RiskySandBox_MainGame.OnSET_my_Team -= EventReceiver_OnSET_my_Team;
        RiskySandBox_MainGame.OnsaveMap -= EventReceiver_OnsaveMap;
    }






    void EventReceiver_OnSET_my_Team(RiskySandBox_Tile _Tile)
    {
        updateVisuals();
    }



    private void Update()
    {
        if (RiskySandBox_LevelEditor.is_enabled == false || this.show_level_editor_ui == false)
            return;

        
        if(Input.GetKeyDown(KeyCode.Space))
        {
            RiskySandBox_Tile _Tile = RiskySandBox_CameraControls.current_hovering_Tile;
            
            if(_Tile != null)
            {
                //toggle...
                if (this.tile_IDs.Contains(_Tile.ID))
                    this.tile_IDs.Remove(_Tile.ID);
                else
                    this.tile_IDs.Add(_Tile.ID);

            }

        }

        
        if (Input.GetKeyDown(KeyCode.V))
        {
            //add a point to the current border...
            if(current_LineRenderer == null)
                createNewBorder();
            RiskySandBox_LevelEditorHandle.createHandle(RiskySandBox_CameraControls.mouse_position,this.PRIVATE_border_width);
        } 
        

        if(Input.GetKeyDown(KeyCode.C))
        {
            current_LineRenderer = null;
            RiskySandBox_LevelEditorHandle.destroyAllHandles();
        }
        
    }

    void LevelEditorHandleEventReceiver_OnListUpdate_all_instances()
    {
        if (RiskySandBox_LevelEditor.is_enabled == false || this.show_level_editor_ui == false || current_LineRenderer == null)
            return;

        current_LineRenderer.positionCount = RiskySandBox_LevelEditorHandle.all_instances.Count;
        current_LineRenderer.SetPositions(RiskySandBox_LevelEditorHandle.all_instances.Select(x => x.transform.position).ToArray());
        
    }


    public void deleteBorderData()
    {
        foreach(LineRenderer _LineRenderer in this.border_LineRenderers)
        {
            Destroy(_LineRenderer);
        }

        border_LineRenderers.Clear();
    }

    public void createNewBorder()
    {
        LineRenderer _new_LineRenderer = new GameObject().AddComponent<LineRenderer>();
        _new_LineRenderer.transform.parent = this.transform;
        border_LineRenderers.Add(_new_LineRenderer);
        current_LineRenderer = _new_LineRenderer;
        current_LineRenderer.startWidth = this.PRIVATE_border_width;
        current_LineRenderer.endWidth = this.PRIVATE_border_width;
        RiskySandBox_LevelEditorHandle.destroyAllHandles();
        
    }



    public void createNewBorder(IEnumerable<Vector3> _points)
    {
        //create a new linerenderer...
        LineRenderer _new_LineRenderer = new GameObject().AddComponent<LineRenderer>();
        _new_LineRenderer.gameObject.transform.parent = this.transform;

        border_LineRenderers.Add(_new_LineRenderer);

        List<Vector3> _points_list = new List<Vector3>(_points);
        _new_LineRenderer.positionCount = _points_list.Count();

        for(int i = 0; i < _points_list.Count(); i += 1)
        {
            _new_LineRenderer.SetPosition(i, _points_list[i]);
        }

        _new_LineRenderer.startWidth = this.PRIVATE_border_width;
        _new_LineRenderer.endWidth = this.PRIVATE_border_width;
    }


    public void selfDestruct()
    {
        UnityEngine.Object.Destroy(gameObject);
    }



    public static void destroyAllBonuses()
    {
        foreach(RiskySandBox_Bonus _Bonus in new List<RiskySandBox_Bonus>(RiskySandBox_Bonus.all_instances))
        {
            UnityEngine.Object.Destroy(_Bonus.gameObject);
        }
    }


    public static RiskySandBox_Bonus createNewBonus()
    {
        if(PrototypingAssets.run_server_code.value == false)
        {
            GlobalFunctions.printError("only the server can create a bonus! (for now)",null);
            return null;
        }    

        GameObject _new_GameObject = Photon.Pun.PhotonNetwork.InstantiateRoomObject(RiskySandBox_Resources.Bonus_prefab.name, new Vector3(0, 0, 0), Quaternion.identity);

        //GameObject _new_GameObject = UnityEngine.Object.Instantiate(RiskySandBox_Resources.Bonus_prefab,RiskySandBox_MainGame.instance.bonus_parent_Transform);

        return _new_GameObject.GetComponent<RiskySandBox_Bonus>();


    }


    public static RiskySandBox_Bonus GET_RiskySandBox_Bonus(RiskySandBox_Tile _Tile)
    {
        if(_Tile == null)
        {
            //TODO - debug wtf?!?!?!?
            return null;
        }
        //TODO - what happens if multiple bonuses can have the samme tile????
        foreach(RiskySandBox_Bonus _Bonus in RiskySandBox_Bonus.all_instances)
        {
            if (_Bonus.tile_IDs.Contains(_Tile.ID) == true)
                return _Bonus;
        }
        return null;
    }

    public void updateVisuals()
    {
        if(RiskySandBox_LevelEditor.is_enabled == false)
        {

            //a list of all the "Teams" that control tiles within this bonus...
            List<RiskySandBox_Team> _Teams = new HashSet<RiskySandBox_Team>(this.tile_IDs.Select(x => RiskySandBox_Tile.GET_RiskySandBox_Tile(x)).Where(x => x != null).Select(x => x.my_Team)).ToList();




            ///TODO - what happens if a team doesnt have to control ALL tiles in order to get the bonus...
            //this is unlikely to be a feature but could be interesting???  
            if (_Teams.Count == 1 && _Teams[0] != null)
            {
                //update my borders to match....
                foreach(LineRenderer _LineRenderer in this.border_LineRenderers)
                {
                    _LineRenderer.material = _Teams[0].my_Material;
                }

            }
            

            return;
        }

        if (show_level_editor_ui == false)
        {
            foreach(LineRenderer _LineRenderer in this.border_LineRenderers)
            {
                _LineRenderer.material = null;
            }
            return;
        }
            

        foreach(RiskySandBox_Tile _Tile in RiskySandBox_Tile.all_instances)
        {
            _Tile.my_LevelEditor_Material = null;
        }

        foreach(int _id in this.tile_IDs) 
        {
            RiskySandBox_Tile _Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(_id);
            if (_Tile == null)
                continue;

            _Tile.my_LevelEditor_Material = PrototypingAssets_Materials.white;
            
        }

        foreach (LineRenderer _LineRenderer in this.border_LineRenderers)
        {
            _LineRenderer.material = PrototypingAssets_Materials.black;
        }
        


    }



    void EventReceiver_OnsaveMap(string _directory)
    {
        //save myself into the directory!
        int _bonus_id = RiskySandBox_Bonus.all_instances.IndexOf(this);

        string _file = System.IO.Path.Combine(_directory, string.Format("Bonus_{0}.txt", _bonus_id));

        System.IO.StreamWriter _writer = new System.IO.StreamWriter(_file);


        _writer.WriteLine("generation:" + this.generation.ToString());//save "generation"
        _writer.WriteLine("name:" + this.name);//save the name
        _writer.WriteLine("tiles:" + string.Join(",", this.tile_IDs));//save the list of tile ids...
        _writer.WriteLine(string.Format("my_Color:{0},{1},{2}", this.my_Color_r, this.my_Color_g, this.my_Color_b));//save the material for the bonus
        _writer.WriteLine(string.Format("ui_scale:{0},{1},{2}", this.ui_scale.x, this.ui_scale.y, this.ui_scale.z));
        _writer.WriteLine(string.Format("ui_position:{0},{1},{2}", this.ui_position.x, this.ui_position.y, this.ui_position.z));
        _writer.WriteLine("border_width:" + this.PRIVATE_border_width.value);

        string _borders_string = "";
        foreach (LineRenderer _LineRenderer in this.border_LineRenderers)
        {
            for (int p = 0; p < _LineRenderer.positionCount; p += 1)
            {
                Vector3 _v3 = _LineRenderer.GetPosition(p);
                _borders_string = _borders_string + string.Format("{0},{1},{2},", _v3.x, _v3.y, _v3.z);
            }
            _borders_string = _borders_string.TrimEnd(',');
            _borders_string = _borders_string + "|";
        }

        if (_borders_string.Count() > 0)
        {
            //remove the last '|'
            _borders_string = _borders_string.TrimEnd('|');
        }

        _writer.WriteLine("borders:" + _borders_string);

        _writer.Close();//save...
    }


    public static RiskySandBox_Bonus loadBonus(string[] _lines)
    {
        //open up the data file....

        RiskySandBox_Bonus _new_Bonus = createNewBonus();

        Dictionary<string, string> _data = new Dictionary<string, string>();

        foreach (string _line in _lines)
        {
            _data.Add(_line.Split(":")[0],_line.Split(":")[1]);
        }


        if(_data.ContainsKey("name") == true)
            _new_Bonus.name.value = _data["name"];

        if(_data.ContainsKey("generation") == true)
            _new_Bonus.generation.value = int.Parse(_data["generation"]);

        if(_data.ContainsKey("tiles") == true)
            _new_Bonus.tile_IDs.AddRange(_data["tiles"].Split(",").Select(x => int.Parse(x)).ToList());

        if(_data.ContainsKey("my_Color") == true)
        {
            _new_Bonus.my_Color_r.value = int.Parse(_data["my_Color"].Split(",")[0]);
            _new_Bonus.my_Color_g.value = int.Parse(_data["my_Color"].Split(",")[1]);
            _new_Bonus.my_Color_b.value = int.Parse(_data["my_Color"].Split(",")[2]);
        }

        if(_data.ContainsKey("ui_scale") == true)
            _new_Bonus.ui_scale.value = new Vector3(float.Parse(_data["ui_scale"].Split(",")[0]), float.Parse(_data["ui_scale"].Split(",")[1]), float.Parse(_data["ui_scale"].Split(",")[2]));

        if (_data.ContainsKey("ui_position") == true)
            _new_Bonus.ui_scale.value = new Vector3(float.Parse(_data["ui_position"].Split(",")[0]), float.Parse(_data["ui_position"].Split(",")[1]), float.Parse(_data["ui_position"].Split(",")[2]));


        if(_data.ContainsKey("borders") == true)
        {
            if (_data["borders"].Length > 0)
            {
                foreach (string _line in _data["borders"].Split("|"))
                {
                    
                    float[] _float_values = _line.Split(",").Select(x => float.Parse(x)).ToArray();
                    List<Vector3> _points = new List<Vector3>();

                    for (int i = 0; i < _float_values.Count() / 3; i += 1)
                    {
                        _points.Add(new Vector3(_float_values[i * 3], _float_values[i * 3 + 1], _float_values[i * 3 + 2]));
                    }
                    _new_Bonus.createNewBorder(_points);
                }
            }
        }

        if(_data.ContainsKey("border_width"))
            _new_Bonus.PRIVATE_border_width.value = float.Parse(_data["border_width"]);
        

        return _new_Bonus;
    }


}
