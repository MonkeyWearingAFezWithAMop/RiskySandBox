using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_MainGame_CapitalsMode : MonoBehaviour
{

    public static RiskySandBox_MainGame_CapitalsMode instance;

    [SerializeField] bool debugging;

    [SerializeField] ObservableInt PRIVATE_next_Team_index;


    private void Awake()
    {
        instance = this;
        RiskySandBox_MainGame.OnplaceCapital += EventReceiver_OnplaceCapital;
    }

    void EventReceiver_OnplaceCapital(RiskySandBox_Tile _Tile)
    {
        if (PrototypingAssets.run_server_code.value == false || RiskySandBox_MainGame.instance.game_started.value == true)
            return;


        _Tile.my_Team.current_turn_state.value = RiskySandBox_Team.turn_state_waiting;


        List<RiskySandBox_Team> _next_Team_candidates = RiskySandBox_Team.all_instances.Where(x => x.required_capital_placements > 0).ToList();

        if(_next_Team_candidates.Count() == 0)
        {
            RiskySandBox_MainGame.instance.EventReceiver_OncapitalsModeSetupComplete();//tell the main game the capitals mode has been complete!
            return;
        }



        while(true)
        {
            PRIVATE_next_Team_index.value += 1;

            if (PRIVATE_next_Team_index >= RiskySandBox_Team.all_instances.Count())
            {
                PRIVATE_next_Team_index.value = 0;
            }

            if(RiskySandBox_Team.all_instances[PRIVATE_next_Team_index].required_capital_placements > 0)
            {
                //
                RiskySandBox_Team.all_instances[PRIVATE_next_Team_index].current_turn_state.value = RiskySandBox_Team.turn_state_placing_capital;
                
                break;
            }

        }

    }

    public void startGame()
    {
        //great! - lets put everyone into the placing capitals mode state....
        foreach(RiskySandBox_Team _Team in RiskySandBox_Team.all_instances)
        {
            _Team.required_capital_placements.value += RiskySandBox_MainGame.instance.num_capitals_startGame;
        }


        if(RiskySandBox_Team.all_instances.Count() > 0)
        {
            RiskySandBox_Team.all_instances[0].current_turn_state.value = RiskySandBox_Team.turn_state_placing_capital;
        }
            



    }
}
