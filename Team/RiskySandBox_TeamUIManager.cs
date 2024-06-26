using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_TeamUIManager : MonoBehaviour
{
    [SerializeField] bool debugging;
    [SerializeField] RiskySandBox_Team my_Team;

    [SerializeField] UnityEngine.UI.Text remaining_turn_length_Text;
    [SerializeField] UnityEngine.UI.Image turn_timer_Image;

    [SerializeField] UnityEngine.UI.Image UI_background_Image;


    [SerializeField] UnityEngine.UI.Text num_troops_Text;
    [SerializeField] UnityEngine.UI.Text num_Tiles_Text;
   



    public void EventReceiver_OnVariableUpdate_ID(ObservableInt _ID)
    {
        if (this.debugging)
            GlobalFunctions.print("my ID just changed... updating the ui element colors!", this, _ID);

        UI_background_Image.color = my_Team.my_Color;

        this.turn_timer_Image.color = my_Team.my_Color;

        num_troops_Text.color = my_Team.text_Color;
        num_Tiles_Text.color = my_Team.text_Color;
        remaining_turn_length_Text.color = my_Team.text_Color;


    }



    // Update is called once per frame
    void Update()
    {
        float _remaining_turn_length = my_Team.end_turn_time_stamp.value - RiskySandBox.instance.current_time.value;//TODO - replace with my_Team.remaining_turn_length - just duplicated code otherwise...

        if (this.remaining_turn_length_Text != null)
            this.remaining_turn_length_Text.text = "" + Mathf.CeilToInt(_remaining_turn_length);

        float _fill_amount = _remaining_turn_length / RiskySandBox_MainGame.instance.turn_length_seconds;

        if (_fill_amount < 0)
            _fill_amount = 0;

        if (_fill_amount > 1)
            _fill_amount = 1;

        turn_timer_Image.fillAmount = _fill_amount;
    }
}
