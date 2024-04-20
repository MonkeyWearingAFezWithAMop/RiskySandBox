using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using System.IO;

public partial class RiskySandBox_MainGame
{



    public static string map_path { get { return Application.streamingAssetsPath + "/Maps"; } }



    static Dictionary<string, string> extractData(string[] lines)
    {
        // Create a dictionary to store key-value pairs
        Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

        // Iterate over each line
        foreach (string line in lines)
        {
            // Split the line at the colon
            string[] parts = line.Split(':');

            // Ensure that the line contains both key and value
            if (parts.Length == 2)
            {
                // Trim whitespace from key and value
                string key = parts[0].Trim();
                string value = parts[1].Trim();

                // Add key-value pair to the dictionary
                keyValuePairs[key] = value;
            }
        }



        return keyValuePairs;
    }




    void loadGraph(string[] _lines)
    {


        this.graph.Clear();//wipe all data...

        foreach(string _line in _lines)
        {
            List<int> _data = _line.Split(",").Select(x => int.Parse(x)).ToList();


            int _key = _data[0];

            List<int> _connections = _data.Skip(1).ToList();


            this.graph[_key] = _connections;

        }
    }

    void loadBonuses(string[] _lines)
    {
        //create a new bonus...
        foreach(string _line in _lines)
        {
            List<int> _data = _line.Split(",").Select(x => int.Parse(x)).ToList();


            int _generation = _data[0];

            List<int> _tile_IDs = _data.Skip(1).ToList();


            RiskySandBox_MainGame.Bonus _new_Bonus = new RiskySandBox_MainGame.Bonus();
            _new_Bonus.generation = _generation;
            _new_Bonus.tile_IDs = _tile_IDs;


            this.bonuses.Add(_new_Bonus);

        }
    }





    public void loadTiles(string _path)
    {
        if(this.debugging == true)
            print("loading the tiles... in folder: '" + _path + "'");

        foreach (string _tile_file in Directory.GetFiles(_path))
        {
            if (Path.GetFileName(_tile_file).EndsWith(".txt") == false)//we dont care about file types that arn't .txt
                continue;
            if(this.debugging == true)
                print("loading the tile at: " + _tile_file);

            string[] _lines = File.ReadAllLines(_tile_file);

            Dictionary<string, string> _tile_data = extractData(_lines);


            // pull out the required infomation about the tile... constuct a mesh for this tile from the vertices and the triangles...
            //create a tile class for this tile and assign the mesh to it...

            int _tile_ID = int.Parse(_tile_data["ID"]);

            List<Vector3> _mesh_verts = new List<Vector3>();

            // Replace all occurrences of "[" and "]"
            while (_tile_data["mesh_verts"].Contains("[") || _tile_data["mesh_verts"].Contains("]"))
            {
                _tile_data["mesh_verts"] = _tile_data["mesh_verts"].Replace("[", "").Replace("]", "");
            }

            // Split the input string by ","
            string[] vectorStrings = _tile_data["mesh_verts"].Split(',');

            // Iterate through each string in the array
            for (int i = 0; i < vectorStrings.Length; i += 3)
            {
                // Parse each component of the Vector3
                float x = float.Parse(vectorStrings[i]);
                float y = float.Parse(vectorStrings[i + 1]);
                float z = float.Parse(vectorStrings[i + 2]);

                // Create a new Vector3 and add it to the list
                _mesh_verts.Add(new Vector3(x, y, z));
            }


            List<int> _mesh_tris = _tile_data["mesh_tris"].Replace("[", "").Replace("]", "").Split(",").Select(x => int.Parse(x)).ToList();

            _tile_data["position"] = _tile_data["position"].Replace("[", "").Replace("]", "");
            Vector3 _position = new Vector3(float.Parse(_tile_data["position"].Split(",")[0]), float.Parse(_tile_data["position"].Split(",")[1]), float.Parse(_tile_data["position"].Split(",")[2]));

            _tile_data["scale"] = _tile_data["scale"].Replace("[", "").Replace("]", "");
            Vector3 _scale = new Vector3(float.Parse(_tile_data["scale"].Split(",")[0]), float.Parse(_tile_data["scale"].Split(",")[1]), float.Parse(_tile_data["scale"].Split(",")[2]));


            RiskySandBox_Tile.createTile(_tile_ID, _position, Quaternion.identity, _scale, _mesh_verts, _mesh_tris);






        }
    }





    public void loadMap(string _Map_ID)
    {
        //kill all tiles...
        RiskySandBox_Tile.destroyAllTiles();


        // Get a list of all folders (directories) in the specified folder path
        string[] directories = Directory.GetDirectories(map_path);
        bool _found_map = false;

        // Loop through each directory
        foreach (string directory in directories)
        {
            //print("seaching through " + directory);
            // Get all files within the current directory
            string[] files = Directory.GetFiles(directory);

            // Loop through each file in the current directory
            foreach (string file in files)
            {
                //print("checking file: " + file);
                // Check if the file is named "MapInfo"
                if (Path.GetFileName(file) == "MapInfo.txt")
                {

                    string[] lines = File.ReadAllLines(file);

                    Dictionary<string, string> _map_info = extractData(lines);

                    

                    if (_map_info["ID"] == _Map_ID)
                    {
                        //fantastic! - we found the correct map - its time to load the tiles...
                        loadTiles(directory + "/Tiles");//path to the tiles...
                        _found_map = true;
                    }
                }

                if(Path.GetFileName(file) == "Graph.txt")//awesome the graph can be loaded using this file!
                {
                    string[] _lines = File.ReadAllLines(file);
                    loadGraph(_lines);
                }

                if(Path.GetFileName(file) == "Bonuses.txt")
                {
                    string[] _lines = File.ReadAllLines(file);
                    loadBonuses(_lines);
                }


            }
        }

        

        if (_found_map == false)//TODO - how do we handles this? - do we error out? or ask the server? to give us a copy of this map?
        {
            GlobalFunctions.print("unable to find the Map for the Map ID = '" + _Map_ID+"'", this);
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
            _Tile.my_Team = RiskySandBox_Team.GET_RiskySandBox_Team(_KVP.Value);
        }


        
    }




    public static List<string> GET_mapIDs()
    {
        List<string> _IDS = new List<string>();


        // Get a list of all folders (directories) in the specified folder path
        string[] directories = Directory.GetDirectories(map_path);

        
        // Loop through each directory
        foreach (string directory in directories)//read through the map directories
        {
            string[] files = Directory.GetFiles(directory);//grab all the files

            
            foreach (string file in files)// Loop through each file in the current directory
            {
                if (Path.GetFileName(file) != "MapInfo.txt")//awesome! lets get the id...
                    continue;
                string[] lines = File.ReadAllLines(file);

                Dictionary<string, string> _map_info = extractData(lines);


                string _map_ID = _map_info["ID"];

                _IDS.Add(_map_ID);
            }
        }

        return _IDS;
    }



}
