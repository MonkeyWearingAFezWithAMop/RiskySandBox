using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_HumanPlayer_PlaceCapitalState : MonoBehaviour
{

    [SerializeField] RiskySandBox_HumanPlayer my_HumanPlayer;


    private void Awake()
    {
        RiskySandBox_HumanPlayer.OnleftClick += EventReceiver_OnLeftClick;
    }

    private void OnDestroy()
    {
        RiskySandBox_HumanPlayer.OnleftClick -= EventReceiver_OnLeftClick;
    }

    void EventReceiver_OnLeftClick(RiskySandBox_HumanPlayer _HumanPlayer, string _current_turn_state)
    {
        if (_HumanPlayer != this.my_HumanPlayer)
            return;

        if (_current_turn_state != RiskySandBox_Team.turn_state_placing_capital)
            return;

        RiskySandBox_Tile _current_Tile = RiskySandBox_CameraControls.current_hovering_Tile;

        if (_current_Tile == null)
            return;

        my_HumanPlayer.TRY_placeCapital(_current_Tile);

    }




}
