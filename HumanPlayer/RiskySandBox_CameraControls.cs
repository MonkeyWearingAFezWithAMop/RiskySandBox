using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_CameraControls : MonoBehaviour
{

    public static Vector3 mouse_position { get { return instance.PRIVATE_mouse_position.value; } }

    public static RiskySandBox_CameraControls instance { get; private set; }

    [SerializeField] bool debugging;

    [SerializeField] GameObject root_UI;


    public Camera my_Camera { get { return PRIVATE_my_Camera; } }
    public ObservableFloat camera_fov { get { return PRIVATE_camera_fov; } }


    [SerializeField] Camera PRIVATE_my_Camera;

    [SerializeField] ObservableFloat PRIVATE_camera_movement_speed;
    [SerializeField] ObservableFloat PRIVATE_camera_zoom_speed;
    
    [SerializeField] ObservableFloat PRIVATE_camera_fov;
    [SerializeField] UnityEngine.UI.Text current_hovering_Tile_ID_Text;

    [SerializeField] Vector3 grid_size = new Vector3(1, 1, 1);

    [SerializeField] ObservableVector3 PRIVATE_camera_position;
    [SerializeField] ObservableVector3 PRIVATE_mouse_position;


    public static Vector3 current_hit_point
    {
        get
        {
            Ray _Ray = instance.PRIVATE_my_Camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(_Ray, out RaycastHit _hit, Mathf.Infinity))
                return _hit.point;
            return Vector3.positiveInfinity ;
        }
    }


    public static RiskySandBox_Tile current_hovering_Tile
    {
        get
        {
            Ray _Ray = instance.PRIVATE_my_Camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(_Ray, out RaycastHit _hit, Mathf.Infinity))
                return _hit.collider.GetComponent<RiskySandBox_Tile>();
            return null;
        }
    }


    private void Awake()
    {
        instance = this;
    }


    

    public void enable()
    {
        this.root_UI.SetActive(true);
    }

    public void disable()
    {
        this.root_UI.SetActive(false);
    }



    Vector3 RoundVector3To1DP(Vector3 vector)
    {
        return new Vector3(Mathf.Round(vector.x * 10f) / 10f, Mathf.Round(vector.y * 10f) / 10f, Mathf.Round(vector.z * 10f) / 10f);
    }

    private void Update()
    {

        Ray ray = instance.PRIVATE_my_Camera.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);

        if (plane.Raycast(ray, out float _distance))
        {
            PRIVATE_mouse_position.value = GlobalFunctions.snapToGrid(ray.GetPoint(_distance), instance.grid_size);
        }



        PRIVATE_my_Camera.fieldOfView = PRIVATE_camera_fov;

        Vector3 _camera_movement = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.W))
            _camera_movement += Vector3.forward;
        if (Input.GetKey(KeyCode.A))
            _camera_movement += Vector3.left;
        if (Input.GetKey(KeyCode.S))
            _camera_movement += Vector3.back;
        if (Input.GetKey(KeyCode.D))
            _camera_movement += Vector3.right;

        _camera_movement = _camera_movement * this.PRIVATE_camera_movement_speed;

        _camera_movement.y += 1f * PRIVATE_camera_zoom_speed * Input.GetAxis("Mouse ScrollWheel");

        if(Input.GetKey(KeyCode.UpArrow))
        {
            _camera_movement.y += 1f * this.PRIVATE_camera_zoom_speed;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            _camera_movement.y -= 1f * this.PRIVATE_camera_zoom_speed;
        }

        _camera_movement *= Time.deltaTime;

        this.PRIVATE_camera_position.value += _camera_movement;

        if (this.PRIVATE_camera_position.y <= 1)
            this.PRIVATE_camera_position.y = 1f;
        


        RiskySandBox_Tile _current_hovering_Tile = current_hovering_Tile;

        if (_current_hovering_Tile == null)
        {
            current_hovering_Tile_ID_Text.text = "Tile ID: null";
        }
        else
        {
            current_hovering_Tile_ID_Text.text = "Tile ID: " + _current_hovering_Tile.ID;
        }

    }

    public void SET_cameraPosition(Vector3 _position)
    {
        this.PRIVATE_camera_position.value = _position;
    }

    public Vector3 GET_cameraPosition()
    {
        return this.PRIVATE_camera_position.value;
    }



    public void loadSettings(string _camera_settings_path)
    {
        Dictionary<string, string> _camera_settings = new Dictionary<string, string>();

        foreach (string _line in System.IO.File.ReadAllLines(_camera_settings_path))
        {
            _camera_settings.Add(_line.Split(':')[0], _line.Split(':')[1]);
        }

        if (_camera_settings.ContainsKey("position") == true)
        {
            List<float> _float_values = _camera_settings["position"].Split(",").Select(x => float.Parse(x)).ToList();
            RiskySandBox_CameraControls.instance.SET_cameraPosition(new Vector3(_float_values[0],_float_values[1],_float_values[2]));
        }
        if (_camera_settings.ContainsKey("fov") == true)
        {
            RiskySandBox_CameraControls.instance.camera_fov.value = float.Parse(_camera_settings["fov"]);
        }

        if (_camera_settings.ContainsKey("camera_movement_speed_xz"))
        {
            RiskySandBox_CameraControls.instance.PRIVATE_camera_movement_speed.value = float.Parse(_camera_settings["camera_movement_speed_xz"]);
        }

        if (_camera_settings.ContainsKey("camera_movement_speed_y"))
        {
            RiskySandBox_CameraControls.instance.PRIVATE_camera_zoom_speed.value = float.Parse(_camera_settings["camera_movement_speed_y"]);
        }
    }

    public void saveSettings(string _directory)
    {
        System.IO.StreamWriter writer = new System.IO.StreamWriter(_directory);
        writer.WriteLine(string.Format("position:{0},{1},{2}",this.PRIVATE_camera_position.x,this.PRIVATE_camera_position.y,this.PRIVATE_camera_position.z));
        writer.WriteLine("fov:"+this.camera_fov.value);
        writer.WriteLine("camera_movement_speed_xz:" + PRIVATE_camera_movement_speed.value);
        writer.WriteLine("camera_movement_speed_y:" + PRIVATE_camera_zoom_speed.value);
        writer.Close();
    }

}
