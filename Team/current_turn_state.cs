using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_Team
{

    public ObservableString current_turn_state { get { return PRIVATE_current_turn_state; } }
    [SerializeField] private ObservableString PRIVATE_current_turn_state;



    /// <summary>
    /// invoked whenever a Team.current_turn_state.value changes...
    /// </summary>
    public static event Action<RiskySandBox_Team> OnVariableUpdate_current_turn_state_STATIC;



    public void EventReceiver_OnVariableUpdate_current_turn_state(ObservableString _current_turn_state)
    {
        OnVariableUpdate_current_turn_state_STATIC?.Invoke(this);
    }




}
