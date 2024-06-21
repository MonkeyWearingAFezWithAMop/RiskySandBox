using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_MainGame_CapitalsMode : MonoBehaviour
{

    public static RiskySandBox_MainGame_CapitalsMode instance;

    [SerializeField] bool debugging;

    [SerializeField] ObservableInt PRIVATE_next_Team_index;



    public static ObservableInt num_capitals_startGame { get { return instance.PRIVATE_num_capitals_startGame; } }
    public static ObservableInt capital_troop_generation { get { return instance.PRIVATE_capital_troop_generation; } }

    public static ObservableBool enable_capitals { get { return instance.PRIVATE_capitals_mode; } }
    public static ObservableFloat capital_conquest_percentage { get { return instance.PRIVATE_capital_conquest_percentage; } }

    public static ObservableBool setup_complete { get { return instance.PRIVATE_setup_complete; } }



    [SerializeField] ObservableBool PRIVATE_capitals_mode;
    [SerializeField] ObservableFloat PRIVATE_capital_conquest_percentage;
    [SerializeField] ObservableInt PRIVATE_capital_troop_generation;
    [SerializeField] ObservableInt PRIVATE_num_capitals_startGame;
    [SerializeField] ObservableBool PRIVATE_setup_complete;





    private void Awake()
    {
        instance = this;
        RiskySandBox_MainGame.OnplaceCapital += EventReceiver_OnplaceCapital;
    }

    void EventReceiver_OnplaceCapital(RiskySandBox_Tile _Tile)
    {
        if (PrototypingAssets.run_server_code.value == false || RiskySandBox_MainGame_CapitalsMode.setup_complete.value == true)
        {
            if (this.debugging)
                GlobalFunctions.print("PrototypingAssets.run_server_code.value == false OR this.setup_complete == true", this);
            return;
        }


            


        _Tile.my_Team.current_turn_state.value = RiskySandBox_Team.turn_state_waiting;


        List<RiskySandBox_Team> _next_Team_candidates = RiskySandBox_Team.all_instances.Where(x => x.required_capital_placements > 0).ToList();

        if(_next_Team_candidates.Count() == 0)
        {
            if (debugging)
                GlobalFunctions.print("a capital was just placed on " + _Tile+" the capitals setup is now complete... handing control back to RiskySandBox_MainGame", this);

            this.PRIVATE_setup_complete.value = true;
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
                if (this.debugging)
                    GlobalFunctions.print("placing " + RiskySandBox_Team.all_instances[PRIVATE_next_Team_index] + " into the placing capital state!",this);
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
            _Team.required_capital_placements.value += RiskySandBox_MainGame_CapitalsMode.num_capitals_startGame.value;
        }


        if(RiskySandBox_Team.all_instances.Count() > 0)
        {
            RiskySandBox_Team.all_instances[0].current_turn_state.value = RiskySandBox_Team.turn_state_placing_capital;
        }
            



    }
}
