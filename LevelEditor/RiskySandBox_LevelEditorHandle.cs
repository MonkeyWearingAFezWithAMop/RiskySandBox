using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_LevelEditorHandle : MonoBehaviour
{
    public static RiskySandBox_LevelEditorHandle selected_handle;

    public static ObservableList<RiskySandBox_LevelEditorHandle> all_instances = new ObservableList<RiskySandBox_LevelEditorHandle>();

    public static event Action OnUpdate_position_STATIC;


    [SerializeField] bool debugging;

    public static List<GameObject> hovering_handles = new List<GameObject>();

    public Material hovering_Material { get { return PrototypingAssets_Materials.red; } }
    public Material default_Material { get { return PrototypingAssets_Materials.white; } }




    

    // Start is called before the first frame update
    void Start()
    {
        all_instances.Add(this);
    }
    private void OnDestroy()
    {
        all_instances.Remove(this);
    }

    private void OnDisable()
    {
        if (hovering_handles.Contains(gameObject))
            hovering_handles.Remove(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (selected_handle == this)
        {
            transform.position = RiskySandBox_CameraControls.mouse_position;
            if(Input.GetMouseButtonUp(0))
            {
                OnUpdate_position_STATIC?.Invoke();
                selected_handle = null;
            }
        }
            
        else
        {
            if(Input.GetMouseButtonDown(0) && hovering_handles.Contains(this.gameObject))
            {
                //select myself?
                RiskySandBox_LevelEditorHandle.selected_handle = this;
            }
        }


        if (Input.GetMouseButtonDown(1))
        {
            if (RiskySandBox_LevelEditorHandle.hovering_handles.Contains(this.gameObject) == true)
            {
                destroyHandle(this);
            }
        }




    }

    private void OnMouseEnter()
    {
        if (hovering_handles.Contains(gameObject) == false)
            hovering_handles.Add(gameObject);

        this.GetComponent<MeshRenderer>().material = hovering_Material;
    }

    private void OnMouseExit()
    {
        if(hovering_handles.Contains(gameObject))
            hovering_handles.Remove(gameObject);
        this.GetComponent<MeshRenderer>().material = default_Material;
    }




    public static void createHandle(Vector3 _point,float _radius)
    {
        GameObject _new = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        _new.transform.position = _point;
        _new.transform.localScale = new Vector3(_radius, _radius, _radius);
        
        RiskySandBox_LevelEditorHandle _handle_script = _new.AddComponent<RiskySandBox_LevelEditorHandle>();
    }


    public static void destroyHandle(RiskySandBox_LevelEditorHandle _Handle)
    {
        //TODO - make sure it is a handle
        //TODO - make sure it is in the handles list
        //TODO - make sure it isnt null....

        UnityEngine.Object.Destroy(_Handle.gameObject);
    }

    public static void destroyAllHandles()
    {
        foreach (RiskySandBox_LevelEditorHandle _Handle in new List<RiskySandBox_LevelEditorHandle>(RiskySandBox_LevelEditorHandle.all_instances))
        {
            destroyHandle(_Handle);
        }
        RiskySandBox_LevelEditorHandle.all_instances.Clear();
    }




}
