using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_MainGame
{
    public static event Action OnstartGame_MultiplayerBridge;



    void createTeams()
    {
        for (int i = 0; i < RiskySandBox_HumanPlayer.all_instances.Count; i += 1)//foreach human player...
        {
            GameObject _new_Team_GO = Photon.Pun.PhotonNetwork.InstantiateRoomObject(RiskySandBox_Resources.Team_prefab.name, Vector3.zero, Quaternion.identity);//create a new team...
            RiskySandBox_Team _new_Team = _new_Team_GO.GetComponent<RiskySandBox_Team>();
            _new_Team.ID.value = i;//assign a unique id for the Team...




            RiskySandBox_HumanPlayer _HumanPlayer = RiskySandBox_HumanPlayer.all_instances[i];//assign the humanplayer team id to be this value aswell...
            _HumanPlayer.my_Team_ID.value = i;

        }

        for (int i = 0; i < RiskySandBox_MainGame.instance.n_bots; i += 1)
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


        this.loadMap(this.map_ID.value);//load the current map...

        if (PrototypingAssets.run_server_code.value == false)
        {
            if (this.debugging)
                GlobalFunctions.print("not the server - returning", this);
            return;
        }

        OnstartGame_MultiplayerBridge?.Invoke();



        //TODO -throw up a temp screen that says 10... 9.... 8.... 7... 6... 5... 4... 3... 2... 1... go!
        //this also gives time for the map to get loaded/synced to clients?



        createTeams();



        //so now this depends... if we are in capitals mode... we want to place the capitals



        foreach (RiskySandBox_Team _Team in RiskySandBox_Team.all_instances)
        {
            _Team.current_turn_state.value = RiskySandBox_Team.turn_state_waiting;
        }



        RiskySandBox_UnorganisedFunctions.startGameDistributeTroops_simple();





        if(this.PRIVATE_assassin_mode.value == true)
        {
            //TODO - assignAssassinTargets()

            List<int> _unassigned_targets = RiskySandBox_Team.all_instances.Select(x => x.ID.value).ToList();



            foreach (RiskySandBox_Team _Team in RiskySandBox_Team.all_instances)
            {

                int _Team_target = -1;

                List<int> _candidates = _unassigned_targets.Where(x => x != _Team.ID).ToList();

                if(_candidates.Count > 0)
                {
                    int _random_index = GlobalFunctions.randomInt(0, _candidates.Count - 1);

                    _Team_target = _candidates[_random_index];
                    _unassigned_targets.Remove(_Team_target);
                }

                _Team.assassin_target_ID.value = _Team_target;
            }


        }

        //TODO - this.createInitialBlizards() or createBlizards(n)... so that way as time goes on we can "shrink" the map with more and more blizards to force the endgame...
        for(int _b = 0; _b < PRIVATE_n_blizards; _b += 1)
        {
            Debug.LogWarning("WARNING - blizards are unimplemented...");
        }

        //TODO - this.createInitialStablePortals()
        for (int _sp = 0; _sp < PRIVATE_n_stable_portals; _sp += 1)
        {
            Debug.LogWarning("WARNING - stable portals are unimplemented...");
        }

        //TODO - this.createInitialUnStablePortals()
        for (int _up = 0; _up < PRIVATE_n_unstable_portals; _up += 1)
        {
            Debug.LogWarning("WARNING - unstable portals are unimplemented...");
        }






        if (this.PRIVATE_capitals_mode.value == true)//if we are using capitals mode...
        {
            RiskySandBox_MainGame_CapitalsMode.instance.startGame();
            //great! - we make the CapitalsMode do its start of game logic then once it is done it should call EventReceiver_OncapitalsModeSetupComplete (this script will then start the main game)
        }

        else
        {
            this.game_started.value = true;

            //well surely we know this is just the Team with the ID = 0 (or whatever the first id is...)
            RiskySandBox_Team _next_Team = GET_nextTeam(null);
            startTurn(_next_Team);
        }











    }

    public void EventReceiver_OncapitalsModeSetupComplete()
    {
        this.game_started.value = true;

        //well surely we know this is just the Team with the ID = 0 (or whatever the first id is...)
        RiskySandBox_Team _next_Team = GET_nextTeam(null);
        startTurn(_next_Team);
    }






}
