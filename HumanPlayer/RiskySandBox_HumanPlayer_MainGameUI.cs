using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_HumanPlayer_MainGameUI : MonoBehaviour
{

    [SerializeField] bool debugging;

    [SerializeField] ObservableInt PRIVATE_team_info_shift;

    [SerializeField] RectTransform team_info_start;


    [SerializeField] ObservableBool PRIVATE_show_ui;
    



    public void disable()
    {
        this.PRIVATE_show_ui.value = false;
    }






    private void Awake()
    {
        this.PRIVATE_show_ui.OnUpdate += delegate { doThing(); };
        RiskySandBox_Team.all_instances.OnUpdate += EventReceiver_OnTeamListUpdate;
        RiskySandBox_Team.OnVariableUpdate_ID_STATIC += TeamEventReceiver_OnVariableUpdate_ID_STATIC;//whenever a Team's ID changes...

        
        PRIVATE_team_info_shift.OnUpdate += delegate { doThing(); };

        




    }

    void TeamEventReceiver_OnVariableUpdate_ID_STATIC(RiskySandBox_Team _Team)
    {
        doThing();
    }

    void EventReceiver_OnTeamListUpdate()
    {
        PRIVATE_team_info_shift.max_value = Math.Max(0, RiskySandBox_Team.all_instances.Count - 6);
        //the RiskySandBox_Team.all_instances list just changed... either a new team was added or a team was removed...
        //we need to update the position of that teams ui...
        doThing();
    }




    public void doThing()
    {
        
        if (team_info_start == null)
        {
            if (debugging)
                GlobalFunctions.print("team_info_start == null... returning", this);
            return;
        }
        foreach(RiskySandBox_Team _Team in RiskySandBox_Team.all_instances.OrderByDescending(x => x.ID.value).Reverse())
        {
            if( _Team == null || _Team.my_UI == null)
            {
                //TODO - print WTF?!?!?! - something is going wrong (or we have just quit?...)
            }
            int _shift = PRIVATE_team_info_shift - _Team.ID;
            Vector2 _final_position = team_info_start.anchoredPosition + new Vector2(0, (_shift) *  60);
            _Team.my_UI.GetComponent<RectTransform>().anchoredPosition = _final_position;

            if (this.debugging)
                GlobalFunctions.print("calling _Team.my_UI.SetActive(" + this.PRIVATE_show_ui.value + ")",this);
            


            _Team.my_UI.gameObject.SetActive(this.PRIVATE_show_ui.value && (_shift >= -5 && _shift <= 0));//disable if "off screen or we are trying to hide the ui...)


        }
    }




}
