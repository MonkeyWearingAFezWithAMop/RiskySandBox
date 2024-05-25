using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_Team : MonoBehaviour
{

    [SerializeField] bool debugging;

    [SerializeField] private ObservableInt PRIVATE_capture_start_ID;
    [SerializeField] private ObservableInt PRIVATE_capture_end_ID;
    [SerializeField] private ObservableInt PRIVATE_n_capitals;
    [SerializeField] private ObservableInt PRIVATE_ID;
    [SerializeField] private ObservableBool PRIVATE_defeated;
    [SerializeField] private ObservableInt PRIVATE_required_capital_placements;
    [SerializeField] private ObservableInt PRIVATE_n_Tiles;

    [SerializeField] private ObservableInt PRIVATE_assassin_target_ID;
    [SerializeField] private ObservableInt PRIVATE_killer_ID;
    [SerializeField] private ObservableBool PRIVATE_show_assassin_target_indicator;


    [SerializeField] ObservableFloat PRIVATE_end_turn_time_stamp;
    [SerializeField] ObservableBool PRIVATE_is_my_turn;
    [SerializeField] GameObject PRIVATE_my_UI;
}
