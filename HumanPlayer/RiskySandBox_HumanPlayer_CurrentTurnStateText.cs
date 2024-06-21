using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_HumanPlayer_CurrentTurnStateText : MonoBehaviour
{
    [SerializeField] bool debugging;

    [SerializeField] List<UnityEngine.UI.Text> UI_Texts = new List<UnityEngine.UI.Text>();


    [SerializeField] RiskySandBox_HumanPlayer my_HumanPlayer;
    RiskySandBox_Team my_Team { get { return this.my_HumanPlayer.my_Team; } }


    private void Awake()
    {
        RiskySandBox_Team.OnVariableUpdate_current_turn_state_STATIC += EventReceiver_OnVariableUpdate_current_turn_state_STATIC;
    }

    private void OnDestroy()
    {
        RiskySandBox_Team.OnVariableUpdate_current_turn_state_STATIC -= EventReceiver_OnVariableUpdate_current_turn_state_STATIC;
    }

    private void Start()
    {
        if (my_Team == null)
            updateText("team == null");
        else
            updateText(my_Team.current_turn_state.value);
    }

    void updateText(string _value)
    {
        foreach(UnityEngine.UI.Text _Text in this.UI_Texts)
        {
            if (_Text == null)
                continue;
            _Text.text = _value;
        }
    }

    void EventReceiver_OnVariableUpdate_current_turn_state_STATIC(RiskySandBox_Team _Team)
    {
        if (_Team != this.my_Team)
            return;

        updateText(_Team.current_turn_state.value);
    }
}
