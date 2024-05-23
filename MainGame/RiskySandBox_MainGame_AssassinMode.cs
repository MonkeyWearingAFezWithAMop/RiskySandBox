using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_MainGame_AssassinMode : MonoBehaviour
{
    [SerializeField] bool debugging;


    private void Awake()
    {

        //ok so when a team kills another team...

        RiskySandBox_Team.OnVariableUpdate_killer_ID_STATIC += RiskySandBox_TeamOnVariableUpdate_killer_ID_STATIC;


    }

    private void OnDestroy()
    {
        RiskySandBox_Team.OnVariableUpdate_killer_ID_STATIC -= RiskySandBox_TeamOnVariableUpdate_killer_ID_STATIC;
    }


    void RiskySandBox_TeamOnVariableUpdate_killer_ID_STATIC(RiskySandBox_Team _Team)
    {
        if (RiskySandBox_MainGame.instance.assassin_mode.value == false)
            return;

        RiskySandBox_Team _killer = RiskySandBox_Team.GET_RiskySandBox_Team(_Team.killer_ID.value);

        if (_killer.assassin_target_ID.value != _Team.ID)
            return;


        GlobalFunctions.print("calling RiskySandBox_MainGame.instance.endGame - assassin mode condition met!",this);
        //TODO - update RiskySandBox_MainGame.winners list...
        RiskySandBox_MainGame.instance.endGame();
        return;
        


    }




}
