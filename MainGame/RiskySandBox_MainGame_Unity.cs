using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_MainGame : MonoBehaviour
{
    [SerializeField] bool debugging;

    [SerializeField] ObservableInt PRIVATE_num_capitals_startGame;
    [SerializeField] ObservableFloat PRIVATE_world_domination_percentage;
    [SerializeField] ObservableString PRIVATE_map_ID;
    [SerializeField] ObservableBool PRIVATE_capitals_mode;
    [SerializeField] ObservableBool PRIVATE_game_started;
    [SerializeField] ObservableFloat PRIVATE_capital_conquest_percentage;
    [SerializeField] ObservableInt PRIVATE_n_bots;
    [SerializeField] ObservableBool PRIVATE_assassin_mode;
    [SerializeField] GameObject PRIVATE_game_setup_UI;
    [SerializeField] ObservableInt PRIVATE_max_num_cards;
    [SerializeField] ObservableInt PRIVATE_capital_troop_generation;



    private void Awake()
    {
        instance = this;
        RiskySandBox_Team.OnVariableUpdate_defeated_STATIC += delegate { endGameCheck(); };
        RiskySandBox_Team.OnturnTimerReachedZero += TeamEventReceiver_OnturnTimerReachedZero;


    }


}
