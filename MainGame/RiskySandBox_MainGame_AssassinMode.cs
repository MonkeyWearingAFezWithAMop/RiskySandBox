using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_MainGame_AssassinMode : MonoBehaviour
{
    public static RiskySandBox_MainGame_AssassinMode instance;
    [SerializeField] bool debugging;


    public static ObservableBool enable_assassin_mode { get { return instance.PRIVATE_assassin_mode; } }
    [SerializeField] ObservableBool PRIVATE_assassin_mode;


    private void Awake()
    {
        instance = this;

        RiskySandBox_Team.OnVariableUpdate_killer_ID_STATIC += RiskySandBox_TeamOnVariableUpdate_killer_ID_STATIC;
        RiskySandBox_Team.OnVariableUpdate_assassin_target_ID_STATIC += EventReceiver_OnVariableUpdate_assassin_target_ID_STATIC;
        RiskySandBox_MainGame.Onattack += EventReceiver_Onattack;


    }

    private void OnDestroy()
    {
        RiskySandBox_Team.OnVariableUpdate_killer_ID_STATIC -= RiskySandBox_TeamOnVariableUpdate_killer_ID_STATIC;
        RiskySandBox_MainGame.Onattack -= EventReceiver_Onattack;
    }

    void EventReceiver_Onattack(RiskySandBox_MainGame.EventInfo_Onattack _EventInfo)
    {
        if (PrototypingAssets.run_server_code.value == false)
        {
            if (this.debugging)
                GlobalFunctions.print("run_server_code.value == false... returning",this);
            return;
        }

        if (RiskySandBox_MainGame_AssassinMode.enable_assassin_mode == false)
        {
            if (this.debugging)
                GlobalFunctions.print("assassin mode is not enabled... returning", this);
            return;
        }
        if (_EventInfo.capture_flag == false)
        {
            if (this.debugging)
                GlobalFunctions.print("wasnt a capture event... returning", this);
            return;
        }
            


        RiskySandBox_Team _attacked_Team = _EventInfo.defending_Team;

        List<RiskySandBox_Tile> _attacked_Teams_Tiles = RiskySandBox_Tile.all_instances.Where(t => t.my_Team_ID.value == _attacked_Team.ID.value).ToList();
        int _killer_ID = _EventInfo.start_Tile.my_Team.ID;

        if (_attacked_Teams_Tiles.Count > 0)

        {
            if (this.debugging)
                GlobalFunctions.print(_attacked_Team.ID.value + " still has tiles (so hasnt been killed...) returning", this);
            return;
        }



        if (this.debugging)
            GlobalFunctions.print("setting the team with id = " + _attacked_Team.ID + " killer_ID.value to " + _killer_ID,this);
        _attacked_Team.killer_ID.value = _killer_ID;
    }

    void EventReceiver_OnVariableUpdate_assassin_target_ID_STATIC(RiskySandBox_Team _Team)
    {
        foreach(RiskySandBox_Team _T in RiskySandBox_Team.all_instances)
        {
            _T.show_assassin_target_indicator.value = false;
        }

        //get hte local human player...
        RiskySandBox_HumanPlayer _HumanPlayer = RiskySandBox_HumanPlayer.local_player;
        if(_HumanPlayer == null || _HumanPlayer.my_Team == null)
        {
            //ok...
            return;
        }
        //get the assassin target...
        if(_HumanPlayer.my_Team.assassin_target != null)
        {
            _HumanPlayer.my_Team.assassin_target.show_assassin_target_indicator.value = true;
        }




    }

    void RiskySandBox_TeamOnVariableUpdate_killer_ID_STATIC(RiskySandBox_Team _Team)
    {
        if (RiskySandBox_MainGame_AssassinMode.enable_assassin_mode.value == false)
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
