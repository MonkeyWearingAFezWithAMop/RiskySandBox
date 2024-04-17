using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_MainGame
{
    public static event Action<EventInfo_OnstartGame> OnstartGame_MultiplayerBridge;
    public static event Action<EventInfo_OnstartGame> OnstartGame;


    public void startGame()
    {

        //load the Map! - NOTE - this will kill the existing tiles? - is this truely what we want to do?
        this.loadMap(this.map_ID.value);
        
        
        if (PrototypingAssets.run_server_code.value == false)
        {
            if (this.debugging)
                GlobalFunctions.print("not the server - returning", this);
            return;
        }

        foreach (RiskySandBox_Team _Team in RiskySandBox_Team.all_instances)
        {
            _Team.current_turn_state.value = RiskySandBox_Team.turn_state_waiting;
        }




        //ok lets go through all the tiles...
        int _current_Team_index = 0;
        foreach (RiskySandBox_Tile _Tile in RiskySandBox_Tile.all_instances)//make sure every single tile has some troops on it...
        {
            RiskySandBox_MainGame.instance.SET_my_Team(_Tile.ID, RiskySandBox_Team.all_instances[_current_Team_index].ID);
            RiskySandBox_MainGame.instance.SET_num_troops(_Tile.ID, 1);
            _current_Team_index += 1;
            if (_current_Team_index >= RiskySandBox_Team.all_instances.Count)
                _current_Team_index = 0;
        }

        EventInfo_OnstartGame _EventInfo = new EventInfo_OnstartGame();


        OnstartGame_MultiplayerBridge?.Invoke(_EventInfo);

        OnstartGame?.Invoke(_EventInfo);

        RiskySandBox_Team _next_Team = GET_nextTeam(null);
        startTurn(_next_Team);

    }




    public struct EventInfo_OnstartGame
    {
        //TODO - add in any useful infomation...
    }



}
