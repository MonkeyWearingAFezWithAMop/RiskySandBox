using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using System.IO;

public partial class RiskySandBox_MainGame
{






    public void loadMap(string _map_ID)
    {
        
        this.clearMap();//essentailly just reset the map (kill all tiles, kill all bonuses) TODO - kill all temporary gameobjects (e.g. the capital or the team?)



        bool _found_map = false;

        // Loop through each directory
        foreach (string directory in Directory.GetDirectories(RiskySandBox.maps_folder_path))//go through all folders in the /Maps folder...
        {
            if (this.debugging)
                GlobalFunctions.print("", this, _map_ID);
            string _MapInfo_path = Path.Combine(directory, "MapInfo.txt");

            if (File.Exists(_MapInfo_path) == false)
                continue;

            string[] _lines = File.ReadAllLines(_MapInfo_path);
            Dictionary<string, string> _map_info = new Dictionary<string, string>();

            // Iterate through each string in the list
            foreach (string keyValueString in _lines)
            {
                // Split the string by ":" to get the key and value
                string[] keyValue = keyValueString.Split(':');
                _map_info.Add(keyValue[0], keyValue[1]);
            }


            if (_map_info["ID"] != _map_ID)
                continue;

            //Fantastic we have found the correct directory...
            _found_map = true;

            this.loadMapFromDirectory(directory);//lets load out all the content....
        }







        foreach (KeyValuePair<int, int> _KVP in num_troops_buffer)
        {
            //get the tile
            RiskySandBox_Tile _Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(_KVP.Key);
            _Tile.num_troops.value = _KVP.Value;
        }
        num_troops_buffer.Clear();

        foreach(KeyValuePair<int,int> _KVP in team_buffer)
        {
            RiskySandBox_Tile _Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(_KVP.Key);
            _Tile.my_Team_ID.value = _KVP.Value;
        }
        team_buffer.Clear();


        
    }



    void loadMapFromDirectory(string _directory)
    {
        if (Directory.Exists(_directory + "/Tiles") == true)
        {
            foreach (string _Tile_folder in Directory.GetDirectories(_directory + "/Tiles"))
            {
                RiskySandBox_Tile.loadTile(_Tile_folder);
            }
        }

        string _camera_settings_path = Path.Combine(_directory, "Camera_Settings.txt");

        if (File.Exists(_camera_settings_path) == true)
        {
            RiskySandBox_CameraControls.instance.loadSettings(_camera_settings_path);
        }

        string _graph_path = Path.Combine(_directory, "Graph.txt");

        if (File.Exists(_graph_path) == true)
        {
            //load graph...

            foreach (string _line in File.ReadAllLines(_graph_path))
            {
                try
                {
                    List<int> _data = _line.Split(",").Select(x => int.Parse(x)).ToList();
                    int _key = _data[0];
                    List<int> _connections = _data.Skip(1).ToList();

                    RiskySandBox_Tile _Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(_key);
                    if (_Tile != null)
                    {
                        _Tile.graph_connections.AddRange(_connections);

                    }

                    else
                    {
                        //TODO - print WTF?!?!?!?
                    }
                }
                catch
                {
                    //something went wrong... probably an issue with graph line? e.g. something like 1,    or 1,  2 , 3        (something that int.parse was unable to deal with)
                    GlobalFunctions.printError("error while loading the graph... _line = '" + _line + "'   _directory = "+_directory, this);
                }
            }   
        }

        if (Directory.Exists(_directory + "/Bonuses") == true)
        {

            foreach(string _bonus_folder in Directory.GetDirectories(_directory+ "/Bonuses"))
            {
                RiskySandBox_Bonus.loadBonus(_bonus_folder);
            }


            
        }
    }



}
