using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_LevelEditor_LineMode : MonoBehaviour
{
    public static RiskySandBox_LevelEditor_LineMode instance;


    [SerializeField] bool debugging;

    public ObservableBool enable_behaviour{get{return this.PRIVATE_enable_behaviour;}}
    [SerializeField] ObservableBool PRIVATE_enable_behaviour;


    bool just_enabled_behaviour;
    LineRenderer current_LineRenderer;

    public ObservableFloat line_width { get { return PRIVATE_line_width; } }
    [SerializeField] ObservableFloat PRIVATE_line_width;


    public ObservableInt line_Color_r { get { return PRIVATE_line_Color_r; } }
    public ObservableInt line_Color_g { get { return PRIVATE_line_Color_g; } }
    public ObservableInt line_Color_b { get { return PRIVATE_line_Color_b; } }
    public Color line_Color { get { return new Color(line_Color_r / 255f, line_Color_g / 255f, line_Color_b / 255f); } }

    [SerializeField] ObservableInt PRIVATE_line_Color_r;
    [SerializeField] ObservableInt PRIVATE_line_Color_g;
    [SerializeField] ObservableInt PRIVATE_line_Color_b;


    [SerializeField] List<LineRenderer> all_lines = new List<LineRenderer>();



    private void Awake()
    {
        instance = this;

        RiskySandBox_LevelEditor.Ondisable += EventReceiver_Ondisable;
        RiskySandBox_LevelEditor.OnrequestCloseOtherBehaviours += EventReceiver_OnrequestCloseOtherBehaviours;
        RiskySandBox_LevelEditorHandle.all_instances.OnUpdate += EventReceiver_OneditorHandlesUpdate;

        RiskySandBox_MainGame.OnsaveMap += EventReceiver_OnsaveMap;
        

        this.enable_behaviour.OnUpdate_true += delegate
        {
            this.just_enabled_behaviour = true;
            RiskySandBox_LevelEditor.instance.requestCloseOtherBehaviours();
        };

        this.line_width.OnUpdate += delegate { updateLineRenderer(); };

        this.line_Color_r.OnUpdate += delegate { updateLineRenderer(); };
        this.line_Color_g.OnUpdate += delegate { updateLineRenderer(); };
        this.line_Color_b.OnUpdate += delegate { updateLineRenderer(); };

        this.line_width.OnUpdate += delegate
        {
            foreach(RiskySandBox_LevelEditorHandle _handle in RiskySandBox_LevelEditorHandle.all_instances)
            {
                _handle.transform.localScale = new Vector3(this.line_width, this.line_width, this.line_width);
            }
        };
    }

    private void OnDestroy()
    {
        RiskySandBox_LevelEditor.Ondisable -= EventReceiver_Ondisable;
        RiskySandBox_LevelEditor.OnrequestCloseOtherBehaviours -= EventReceiver_OnrequestCloseOtherBehaviours;
        RiskySandBox_LevelEditorHandle.all_instances.OnUpdate -= EventReceiver_OneditorHandlesUpdate;
    }

    void EventReceiver_Ondisable()
    {
        this.enable_behaviour.value = false;

    }

    void EventReceiver_OneditorHandlesUpdate()
    {
        if (this.enable_behaviour == false)
            return;
        updateLineRenderer();
    }

    void EventReceiver_OnrequestCloseOtherBehaviours()
    {
        if(this.just_enabled_behaviour == true)
        {
            this.just_enabled_behaviour = false;
            return;
        }
        this.enable_behaviour.value = false;
    }

    void updateLineRenderer()
    {
        if (current_LineRenderer == null)
            //TODO - WTF?!?!?!
            return;

        current_LineRenderer.startWidth = this.PRIVATE_line_width;
        current_LineRenderer.endWidth = this.PRIVATE_line_width;

        //update positions..
        current_LineRenderer.positionCount = RiskySandBox_LevelEditorHandle.all_instances.Count;
        for (int _i = 0; _i < RiskySandBox_LevelEditorHandle.all_instances.Count; _i += 1)
        {
            current_LineRenderer.SetPosition(_i, RiskySandBox_LevelEditorHandle.all_instances[_i].transform.position);
        }

        if(current_LineRenderer.material == null)
        {
            current_LineRenderer.material = new Material(Shader.Find("Standard"));
        }

        current_LineRenderer.material.color = this.line_Color;

        
    }

    void EventReceiver_OnsaveMap(string _directory)
    {
        string _file_path = System.IO.Path.Combine(_directory, "Lines.txt");

        System.IO.StreamWriter _writer = new System.IO.StreamWriter(_file_path);

        foreach (LineRenderer _LineRenderer in this.all_lines)
        {
            //save the linerenderer points AND color...
            string _line = string.Format("{0},{1},{2},{3}",_LineRenderer.material.color.r,_LineRenderer.material.color.g,_LineRenderer.material.color.b,_LineRenderer.startWidth);

            for(int _i = 0; _i < _LineRenderer.positionCount; _i += 1)
            {
                _line = _line + string.Format(",{0},{1},{2}", _LineRenderer.GetPosition(_i).x, _LineRenderer.GetPosition(_i).y, _LineRenderer.GetPosition(_i).z);
            }

            _writer.WriteLine(_line);

        }
        _writer.Close();

    }

    private void Update()
    {
        if (this.enable_behaviour == false)
            return;

        if(Input.GetKeyDown(KeyCode.Space))
        {
            //if current line is null?
            if(current_LineRenderer == null)
            {
                //create a new linerenderer...
                current_LineRenderer = new GameObject().AddComponent<LineRenderer>();
                
            }
            //create a point at this point..
            RiskySandBox_LevelEditorHandle.createHandle(RiskySandBox_CameraControls.mouse_position,this.line_width.value);
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            if (current_LineRenderer != null)
            {
                all_lines.Add(current_LineRenderer);
                current_LineRenderer = null;
            }
            RiskySandBox_LevelEditorHandle.destroyAllHandles();
        }

    }


    public static void loadLines(string[] _lines)
    {
        foreach(string _line in _lines)
        {
            float[] _float_values = _line.Split(",").Select(x => float.Parse(x)).ToArray();

            LineRenderer _new_LineRenderer = new GameObject("new LineRenderer").AddComponent<LineRenderer>();

            int _n_positions = (_float_values.Count() - 4) / 3;

            _new_LineRenderer.positionCount = _n_positions;

            for(int i = 0; i < _n_positions; i += 1)
            {
                float _x = _float_values[i * 3 + 0 + 4];
                float _y = _float_values[i * 3 + 1 + 4];
                float _z = _float_values[i * 3 + 2 + 4];

                Vector3 _point = new Vector3(_x, _y, _z);
                _new_LineRenderer.SetPosition(i, _point);
            }

            //create new material...
            Material _new_Material = new Material(Shader.Find("Standard"));
            _new_Material.color = new Color(_float_values[0], _float_values[1], _float_values[2]);
            _new_LineRenderer.material = _new_Material;
            _new_LineRenderer.startWidth = _float_values[3];
            _new_LineRenderer.endWidth = _float_values[3];

            instance.all_lines.Add(_new_LineRenderer);
        }
    }

}
