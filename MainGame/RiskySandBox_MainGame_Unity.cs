using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_MainGame : MonoBehaviour
{
    [SerializeField] bool debugging;

    
    [SerializeField] ObservableFloat PRIVATE_world_domination_percentage;
    [SerializeField] ObservableString PRIVATE_map_ID;
    
    [SerializeField] ObservableBool PRIVATE_game_started;




    [SerializeField] ObservableInt PRIVATE_n_troops_startGame;


    [SerializeField] ObservableInt PRIVATE_n_bots;
    
    [SerializeField] GameObject PRIVATE_game_setup_UI;


    [SerializeField] ObservableInt PRIVATE_max_num_cards;
    [SerializeField] ObservableInt PRIVATE_num_wildcards;


    [SerializeField] ObservableInt PRIVATE_n_stable_portals;
    [SerializeField] ObservableInt PRIVATE_n_unstable_portals;
    [SerializeField] ObservableInt PRIVATE_n_blizards;
    [SerializeField] ObservableFloat PRIVATE_capture_increment;
    [SerializeField] ObservableBool PRIVATE_display_bonuses;
    [SerializeField] ObservableBool PRIVATE_show_escape_menu;


    [SerializeField] ObservableBool PRIVATE_enable_territory_cards;
    [SerializeField] ObservableString PRIVATE_territory_card_mode;



    private void Awake()
    {
        instance = this;
        RiskySandBox_Team.OnVariableUpdate_defeated_STATIC += delegate { endGameCheck(); };
        RiskySandBox_Team.OnturnTimerReachedZero += TeamEventReceiver_OnturnTimerReachedZero;


    }

    public void EventReceiver_OnenterMainMenu()
    {
        this.PRIVATE_show_escape_menu.value = false;
    }


}
