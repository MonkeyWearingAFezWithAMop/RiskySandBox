using System.Collections;using System.Collections.Generic;using System.Linq;using System;using System.IO;
using UnityEngine;

public partial class RiskySandBox_MainGame
{
    public static event Action<string> OnsaveMap;


    public void saveMap(string _map_ID)
    {

        if (debugging)
            GlobalFunctions.print("saving the map!",this,_map_ID);


        string _new_map_folder_dir = Path.Combine(RiskySandBox.maps_folder_path, _map_ID);

        if (Directory.Exists(_new_map_folder_dir) == false)
        {
            Directory.CreateDirectory(_new_map_folder_dir);
        }

        OnsaveMap?.Invoke(_new_map_folder_dir);



        StreamWriter writer = new StreamWriter(_new_map_folder_dir + "/MapInfo.txt");
        writer.WriteLine("ID:" + _map_ID);
        writer.Close();

        try//we DO NOT WANT to fail saving the map just because the graph fails...
        {


            writer = new StreamWriter(_new_map_folder_dir + "/Graph.txt");
            foreach (RiskySandBox_Tile _Tile in RiskySandBox_Tile.all_instances)
            {
                if (_Tile.graph_connections.Count > 0)//if the tile doesnt have connnections???? ok... this is weird but its probably the player using the LevelEditor...
                    writer.WriteLine(_Tile.ID + "," + string.Join(",", _Tile.graph_connections.Select(x => x.ID)));
            }
            writer.Close();
        }
        catch (Exception ex)
        {
            
            Debug.LogError("An error occurred while trying to save the graph... " + ex.Message);
        }


        try
        {
            string _camera_settings_path = Path.Combine(_new_map_folder_dir, "Camera_Settings.txt");
            RiskySandBox_CameraControls.instance.saveSettings(_camera_settings_path);
        }
        catch (Exception ex)
        {
            Debug.LogError("An Error occured while trying to save the camera settings..." + ex.Message);
        }

    }
}
