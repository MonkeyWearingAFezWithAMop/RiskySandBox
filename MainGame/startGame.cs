using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_MainGame
{
    public static event Action OnstartGame_MultiplayerBridge;



    void createTeams(int _n_Tiles)
    {
        if (debugging)
            GlobalFunctions.print("",this,_n_Tiles);

        int _n_human_Teams = RiskySandBox_HumanPlayer.all_instances.Count();
        if(RiskySandBox_HumanPlayer.all_instances.Count() > _n_Tiles)
        {
            _n_human_Teams = _n_Tiles;
        }

        for (int i = 0; i < _n_human_Teams; i += 1)//foreach human player...
        {
            GameObject _new_Team_GO = Photon.Pun.PhotonNetwork.InstantiateRoomObject(RiskySandBox_Resources.Team_prefab.name, Vector3.zero, Quaternion.identity);//create a new team...
            RiskySandBox_Team _new_Team = _new_Team_GO.GetComponent<RiskySandBox_Team>();
            _new_Team.ID.value = i;//assign a unique id for the Team...




            RiskySandBox_HumanPlayer _HumanPlayer = RiskySandBox_HumanPlayer.all_instances[i];//assign the humanplayer team id to be this value aswell...
            _HumanPlayer.my_Team_ID.value = i;

        }


        int _n_bots = RiskySandBox_MainGame.instance.n_bots;
        if(_n_human_Teams + _n_bots > _n_Tiles)
        {
            _n_bots = Math.Max(0, _n_Tiles - _n_human_Teams);
        }

        for (int i = 0; i < _n_bots; i += 1)
        {
            //create a new team...
            GameObject _new_Team_GO = Photon.Pun.PhotonNetwork.InstantiateRoomObject(RiskySandBox_Resources.Team_prefab.name, Vector3.zero, Quaternion.identity);
            RiskySandBox_Team _new_Team = _new_Team_GO.GetComponent<RiskySandBox_Team>();
            _new_Team.ID.value = i + RiskySandBox_HumanPlayer.all_instances.Count;

            RiskySandBox_MainGame.instance.ai_Teams.Add(_new_Team);


        }
    }

    public void startGame()
    {

        if (PrototypingAssets.run_server_code.value == false)
        {
            if (this.debugging)
                GlobalFunctions.print("not the server - returning", this);
            return;
        }

        this.loadMap(this.map_ID.value);//load the current map...

        


//        RiskySandBox_ServerToClientMessages.instance.syncMap();



    }


    void EventReceiver_OnloadMapCompleted()
    {
        //so actually the level editor probably loaded a map... so the main game should NOT DO ANYTHING
        if (RiskySandBox_LevelEditor.is_enabled == true)
            return;

        OnstartGame_MultiplayerBridge?.Invoke();

        createTeams(RiskySandBox_Tile.all_instances.Count());

        //integrate the other alliance settings

        if (RiskySandBox_AllianceSettings.enable_alliances)
        {
            //depending on the alliance settings...
            //lets create the initial alliances...

            try
            {
                if (RiskySandBox_AllianceSettings.alliance_string != "")
                {
                    //parse it...
                    string[] _teams = RiskySandBox_AllianceSettings.alliance_string.value.Split(",");

                    foreach (string _Team_string in _teams)
                    {
                        int[] _ids = _Team_string.Split(".").Select(x => int.Parse(x)).ToArray();
                        //foreach id...
                        //get the team...
                        //if the team isnt null...
                        //add all the other teams...
                        foreach (int _ID in _ids)
                        {
                            //get the team...
                            RiskySandBox_Team _Team = RiskySandBox_Team.GET_RiskySandBox_Team(_ID);
                            if (_Team == null)//weird...
                            {
                                //TODO - perhaps this actually is an error? and we should error out 
                                continue;
                            }

                            foreach (int _other_ID in _ids.Where(x => x != _ID))
                            {
                                //get the team!
                                RiskySandBox_Team _other_Team = RiskySandBox_Team.GET_RiskySandBox_Team(_other_ID);
                                if (_other_Team == null)
                                {
                                    //TODO - decide if this is actually an error...
                                    continue;
                                }
                                _Team.createAlliance(_other_Team);
                            }

                        }

                    }

                }
            }
            catch
            {
                //TODO
                //tell everyone something has gone wrong so they can:
                //  fix it for themselves using the ingame ally?
                //  quit/return to lobby?
                //allow the host of the game to edit the alliances string
                //then give all connected players a chance to "allow" this string to change?
                GlobalFunctions.printError("unable to parse the alliance string... '" + RiskySandBox_AllianceSettings.alliance_string + "'", this);

            }
        }



        //so now this depends... if we are in capitals mode... we want to place the capitals



        foreach (RiskySandBox_Team _Team in RiskySandBox_Team.all_instances)
        {
            _Team.current_turn_state.value = RiskySandBox_Team.turn_state_waiting;
        }



        RiskySandBox_UnorganisedFunctions.startGameDistributeTroops_simple();





        if (RiskySandBox_MainGame_AssassinMode.enable_assassin_mode.value == true)
            RiskySandBox_UnorganisedFunctions.assignAssassinTargets();


        //TODO - this.createInitialBlizards() or createBlizards(n)... so that way as time goes on we can "shrink" the map with more and more blizards to force the endgame...
        for (int _b = 0; _b < PRIVATE_n_blizards; _b += 1)
        {
            List<RiskySandBox_Tile> _blizard_options = RiskySandBox_Tile.all_instances.Where(x => x != null && x.has_blizard.value == false).ToList();


            RiskySandBox_UnorganisedFunctions.createBlizards(PRIVATE_n_blizards);

            //we must be careful here...
            //if we place a blizard...
            //we must ENSURE that all tiles are still technically accessible...
            Debug.LogWarning("WARNING - blizards are unimplemented...");
        }

        //TODO - this.createInitialStablePortals()
        for (int _sp = 0; _sp < PRIVATE_n_stable_portals; _sp += 1)
        {
            List<RiskySandBox_Tile> _portal_options = RiskySandBox_Tile.all_instances.Where(x => x != null && x.has_stable_portal.value == false).ToList();
            if (_portal_options.Count() > 0)
            {
                //select a random tile...
                int _random_index = GlobalFunctions.randomInt(0, _portal_options.Count() - 1);
                RiskySandBox_Tile _Tile = _portal_options[_random_index];
                _Tile.has_stable_portal.value = true;

            }
        }

        //TODO - this.createInitialUnStablePortals()
        for (int _up = 0; _up < PRIVATE_n_unstable_portals; _up += 1)
        {
            Debug.LogWarning("WARNING - unstable portals are unimplemented...");
        }



        ///the game has now officially begun!
        this.game_started.value = true;



        if (RiskySandBox_MainGame_CapitalsMode.enable_capitals.value == true)//if we are using capitals mode...
        {
            RiskySandBox_MainGame_CapitalsMode.instance.startGame();
            //great! - we make the CapitalsMode do its start of game logic then once it is done it should call EventReceiver_OncapitalsModeSetupComplete (this script will then start the main game)
        }

        else
        {
            //well surely we know this is just the Team with the ID = 0 (or whatever the first id is...)
            RiskySandBox_Team _next_Team = GET_nextTeam(null);
            startTurn(_next_Team);
        }
    }

    public void EventReceiver_OncapitalsModeSetupComplete()
    {
        //well surely we know this is just the Team with the ID = 0 (or whatever the first id is...)
        RiskySandBox_Team _next_Team = GET_nextTeam(null);
        startTurn(_next_Team);
    }






}
