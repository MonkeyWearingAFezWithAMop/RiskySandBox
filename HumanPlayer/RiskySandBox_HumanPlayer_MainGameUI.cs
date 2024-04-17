using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_HumanPlayer_MainGameUI : MonoBehaviour
{

    [SerializeField] int team_info_shift = 0;

    [SerializeField] RectTransform team_info_start;

    [SerializeField] GameObject root_ui_GameObject;
    

    public void shiftTeamInfoDown()
    {
        team_info_shift += 1;
        doThing();
    }

    public void shiftTeamInfoUp()
    {
        if (team_info_shift == 0)
            return;
        team_info_shift -= 1;
        doThing();
    }



    private void Awake()
    {
        RiskySandBox_MainGame.OnstartGame += EventReceiver_OnstartGame;
        RiskySandBox_MainGame.OnendGame += EventReceiver_OnendGame;
        RiskySandBox_Team.all_instances.OnUpdate += EventReceiver_OnTeamListUpdate;
    }


    void EventReceiver_OnTeamListUpdate()
    {
        //the RiskySandBox_Team.all_instances list just changed... either a new team was added or a team was removed...
        //we need to update the position of that teams ui...
        doThing();
    }

    void EventReceiver_OnstartGame(RiskySandBox_MainGame.EventInfo_OnstartGame _EventInfo)
    {
        //enable the ui...
        root_ui_GameObject.SetActive(true);
        doThing();
    }

    void EventReceiver_OnendGame()
    {
        root_ui_GameObject.SetActive(false);
    }


    void doThing()
    {
        foreach(RiskySandBox_Team _Team in RiskySandBox_Team.all_instances.OrderByDescending(x => x.ID.value).Reverse())
        {  
            int _shift = team_info_shift - _Team.ID;
            Vector2 _final_position = team_info_start.anchoredPosition + new Vector2(0, (_shift) *  60);
            _Team.my_UI.GetComponent<RectTransform>().anchoredPosition = _final_position;


            _Team.my_UI.gameObject.SetActive(_shift >= -5 && _shift <= 0);//disable if "off screen"
        }
    }




}
