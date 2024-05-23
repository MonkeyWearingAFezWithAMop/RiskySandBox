using System.Collections;using System.Collections.Generic;using System.Linq;using System;using System.IO;
using UnityEngine;

public partial class RiskySandBox_MainGame
{
    public void saveMap(string _map_ID)
    {

        if (debugging)
            GlobalFunctions.print("saving the map!",this,_map_ID);


        string _new_map_folder_dir = Path.Combine(RiskySandBox.maps_folder_path, _map_ID);

        if (Directory.Exists(_new_map_folder_dir) == false)
        {
            Directory.CreateDirectory(_new_map_folder_dir);
        }

        StreamWriter writer = new StreamWriter(_new_map_folder_dir + "/MapInfo.txt");
        writer.WriteLine("ID:" + _map_ID);
        writer.Close();


        writer = new StreamWriter(_new_map_folder_dir + "/Graph.txt");
        foreach(RiskySandBox_Tile _Tile in RiskySandBox_Tile.all_instances)
        {
            if(_Tile.graph_connections.Count > 0)//if the tile doesnt have connnections???? ok... this is weird but its probably the player using the LevelEditor...
                writer.WriteLine(_Tile.ID + "," + string.Join(",", _Tile.graph_connections));
        }
        writer.Close();


        if(Directory.Exists(_new_map_folder_dir+"/"+"Bonuses") == false)
        {
            Directory.CreateDirectory(_new_map_folder_dir+ "/Bonuses");
        }

        for (int i = 0; i < RiskySandBox_Bonus.all_instances.Count; i += 1)
        {
            RiskySandBox_Bonus _Bonus = RiskySandBox_Bonus.all_instances[i];
            string _bonus_path = _new_map_folder_dir + "/Bonuses/"+i;
            RiskySandBox_Bonus.saveBonus(_Bonus, _bonus_path);
        }

        string _camera_settings_path = Path.Combine(_new_map_folder_dir, "Camera_Settings.txt");

        RiskySandBox_CameraControls.instance.saveSettings(_camera_settings_path);

        
        
        

        string tiles_path = Path.Combine(_new_map_folder_dir, "Tiles");

        if (!Directory.Exists(tiles_path))
        {
            Directory.CreateDirectory(tiles_path);
        }


        foreach (RiskySandBox_Tile _Tile in RiskySandBox_Tile.all_instances)
        {
            
            string _tile_path = Path.Combine(tiles_path, _Tile.ID.ToString());

            if (!Directory.Exists(_tile_path))
            {
                Directory.CreateDirectory(_tile_path);
            }

            RiskySandBox_Tile.saveTile(_Tile, _tile_path);
            
        }
    }
}
