using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using System.IO;

#if UNITY_WEBGL
using UnityEngine.Networking;
#endif

public partial class RiskySandBox_MainGame
{

    //unfortunatly... once again webgl must load the map using a coroutine...
    //this means we must let everyone know once the map has been loaded (so that other things can happen e.g. populating the tile data and so on... starting to regret webgl build now... :D
    public static event Action OnloadMapCompleted;




    public void loadMap(string _map_ID)
    {
        
        this.clearMap();//essentailly just reset the map (kill all tiles, kill all bonuses) TODO - kill all temporary gameobjects (e.g. the capital or the team?)

        //webgl builds require slightly different file management...
        //it is really f***** annoying... but so be it its just about worth doing to make it easier for people to play...

#if UNITY_WEBGL && !UNITY_EDITOR
        StartCoroutine("loadMap_WEBGL",_map_ID);
            return;
#endif


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

#if UNITY_WEBGL
    IEnumerator loadMap_WEBGL(string _map_ID)
    {
        //TODO - really we want to ask the the MapInfo file for the number of tiles that exist?
        
        
        int i = 1;//TODO - magic number!
        while(true)
        {
            string _tile_url = Application.streamingAssetsPath + string.Format("/RiskySandBox/Maps/{0}/Tile_{1}.txt",_map_ID,i);

            UnityWebRequest _www = UnityWebRequest.Get(_tile_url);
            yield return _www.SendWebRequest();

            if (_www.result != UnityWebRequest.Result.Success)
                break;
            else
            {
                RiskySandBox_Tile.loadTile(_www.downloadHandler.text.Split('\n').ToArray());
            }

            i += 1;
        }

        //TODO - really we want to ask the MapInfo file for the number of bonuses...
        //load the bonuses...
        i = 0;//TODO - magic number!
        while(true)
        {
            string _bonus_url = Application.streamingAssetsPath + string.Format("/RiskySandBox/Maps/{0}/Bonus_{1}.txt", _map_ID, i);

            UnityWebRequest _www = UnityWebRequest.Get(_bonus_url);
            yield return _www.SendWebRequest();

            if (_www.result != UnityWebRequest.Result.Success)
                break;
            else
            {
                RiskySandBox_Bonus.loadBonus(_www.downloadHandler.text.Split('\n').ToArray());
            }

            i += 1;
        }


        //now load the graph...
        string _graph_url = Application.streamingAssetsPath + string.Format("/RiskySandBox/Maps/{0}/Graph.txt", _map_ID);

        UnityWebRequest www = UnityWebRequest.Get(_graph_url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            foreach (string _line in www.downloadHandler.text.Split('\n'))
            {
                try
                {
                    List<int> _data = _line.Split(",").Select(x => int.Parse(x)).ToList();
                    int _key = _data[0];
                    List<int> _connections = _data.Skip(1).ToList();

                    RiskySandBox_Tile _Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(_key);
                    if (_Tile != null)
                    {
                        _Tile.graph_connections_IDs.AddRange(_connections);

                    }

                    else
                    {
                        //TODO - print WTF?!?!?!?
                    }
                }
                catch
                {
                    //something went wrong... probably an issue with graph line? e.g. something like 1,    or 1,  2 , 3        (something that int.parse was unable to deal with)
                    GlobalFunctions.printError("error while loading the graph... _line = '" + _line, this);
                }
            }
        }


        //now load the camera_settings...




        OnloadMapCompleted?.Invoke();




    }
#endif


    void loadMapFromDirectory(string _directory)
    {

        foreach (string _file_path in Directory.GetFiles(_directory,"*.txt"))
        {
            //TODO - hmm actually what happens if a player calls one of the maps something that contains Tile_  we want a more robust method?
            if(_file_path.Contains("Tile_") == true)//if it is a tile file????
            {
                //load the tile...
                RiskySandBox_Tile.loadTile(System.IO.File.ReadAllLines(_file_path));
                continue;
            }

            if(_file_path == "Camera_Settings.txt")
            {
                RiskySandBox_CameraControls.instance.loadSettings(_file_path);
                continue;
            }

            //TODO - hmm actually what happens if a player calls one of the maps something that contains Bonus_  we want a more robust method?
            if (_file_path.Contains("Bonus_") == true)
            {
                RiskySandBox_Bonus.loadBonus(System.IO.File.ReadAllLines(_file_path));
            }
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
                        _Tile.graph_connections_IDs.AddRange(_connections);

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

        //lines... these are kinda fixed lines on the map e.g. to show the "sea" connections... e.g. brazil -> north africa doesnt touch
        string _lines_file = Path.Combine(_directory, "Lines.txt");
        if(File.Exists(_lines_file) == true)
        {
            RiskySandBox_LevelEditor_LineMode.loadLines(File.ReadAllLines(_lines_file));
        }

        OnloadMapCompleted?.Invoke();


        
    }



}
