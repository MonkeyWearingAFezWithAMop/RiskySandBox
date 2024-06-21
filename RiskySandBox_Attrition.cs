using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_Attrition : MonoBehaviour
{
    [SerializeField] bool debugging;

    [SerializeField] ObservableBool PRIVATE_enable_attrition;
    [SerializeField] ObservableInt PRIVATE_fixed_attrition;
    [SerializeField] ObservableInt PRIVATE_fixed_attrition_threshold;

    [SerializeField] ObservableString PRIVATE_attrition_trigger_condition;



    private void Awake()
    {
        RiskySandBox_Team.OnVariableUpdate_is_my_turn_STATIC += EventReceiver_OnVariableUpdate_is_my_turn_STATIC;
    }

    private void OnDestroy()
    {
        RiskySandBox_Team.OnVariableUpdate_is_my_turn_STATIC -= EventReceiver_OnVariableUpdate_is_my_turn_STATIC;
    }


    void EventReceiver_OnVariableUpdate_is_my_turn_STATIC(RiskySandBox_Team _Team)
    {
        //if the value is now false???
        if(_Team.is_my_turn.value == false && _Team.is_my_turn.previous_value == true)
        {
            //if the trigger condition is "is_my_turn == true"
            if(PRIVATE_attrition_trigger_condition == "is_my_turn == false")
            {
                //apply attrition to the team!
                applyFixedAttrition(_Team);
            }
        }

        if (_Team.is_my_turn.value == true && _Team.is_my_turn.previous_value == false)
        {
            if(PRIVATE_attrition_trigger_condition == "is_my_turn == true")
            {
                applyFixedAttrition(_Team);
            }
        }

    }




    void applyFixedAttrition(RiskySandBox_Team _Team)
    {
        if (PRIVATE_enable_attrition == false)
            return;
        //go through all the teams tiles...
        foreach(RiskySandBox_Tile _Tile in _Team.my_Tiles)
        {
            if (_Tile == null || _Tile.immune_to_attrition)
                continue;

            if(_Tile.num_troops > PRIVATE_fixed_attrition_threshold)
            {
                int _new_value = Math.Max(_Tile.num_troops - PRIVATE_fixed_attrition, PRIVATE_fixed_attrition_threshold);
                _Tile.num_troops.value = _new_value;
            }
        }
    }

    
}
