using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_TileColorPulse : MonoBehaviour
{
    [SerializeField] bool debugging;

    public List<Color> pulse_Colors = new List<Color>();

    [SerializeField] ObservableBool enable_behaviour;

    [SerializeField] RiskySandBox_Tile my_Tile;

    [SerializeField] ColorTransition my_ColorTransition;




    private void Awake()
    {
                                                                        

        this.enable_behaviour.OnUpdate += delegate { my_Tile.updateVisuals(); };

        my_Tile.my_Team_ID.OnUpdate += delegate
        {
            my_ColorTransition.colorA = my_Tile.my_Team.my_Color;
            my_ColorTransition.colorB = pulse_Colors[my_Tile.my_Team.ID.value];
        };

        RiskySandBox_HumanPlayer.OnVariableUpdate_deploy_target_STATIC += EventReceiver_OnVariableUpdate_deploy_target;
        RiskySandBox_HumanPlayer.OnVariableUpdate_attack_start_STATIC += EventReceiver_OnVariableUpdate_attack_start;
        RiskySandBox_HumanPlayer.OnVariableUpdate_attack_target_STATIC += EventReceiver_OnVariableUpdate_attack_target;
        RiskySandBox_HumanPlayer.OnVariableUpdate_fortify_start_STATIC += EventReceiver_OnVariableUpdate_fortify_start;
        RiskySandBox_HumanPlayer.OnVariableUpdate_fortify_target_STATIC += EventReceiver_OnVariableUpdate_fortify_target;


    }

    private void OnDestroy()
    {
        RiskySandBox_HumanPlayer.OnVariableUpdate_deploy_target_STATIC -= EventReceiver_OnVariableUpdate_deploy_target;
        RiskySandBox_HumanPlayer.OnVariableUpdate_attack_start_STATIC -= EventReceiver_OnVariableUpdate_attack_start;
        RiskySandBox_HumanPlayer.OnVariableUpdate_attack_target_STATIC -= EventReceiver_OnVariableUpdate_attack_target;
        RiskySandBox_HumanPlayer.OnVariableUpdate_fortify_start_STATIC -= EventReceiver_OnVariableUpdate_fortify_start;
        RiskySandBox_HumanPlayer.OnVariableUpdate_fortify_target_STATIC -= EventReceiver_OnVariableUpdate_fortify_target;
    }

    void EventReceiver_OnVariableUpdate_deploy_target(RiskySandBox_HumanPlayer _HumanPlayer) { recalculate(); }
    void EventReceiver_OnVariableUpdate_attack_start(RiskySandBox_HumanPlayer _HumanPlayer) { recalculate(); }
    void EventReceiver_OnVariableUpdate_attack_target(RiskySandBox_HumanPlayer _HumanPlayer) { recalculate(); }
    void EventReceiver_OnVariableUpdate_fortify_start(RiskySandBox_HumanPlayer _HumanPlayer) { recalculate(); }
    void EventReceiver_OnVariableUpdate_fortify_target(RiskySandBox_HumanPlayer _HumanPlayer) { recalculate(); }


    void recalculate()
    {
        //ok so this tile should pulse 1)
        //if the local human player is deploying (and can deploy to this tile)

        //if the human player is attacking (and has a selected tile?) and can attack this tile?

        //if the human player is fortifying... and has a selected tile and can fortify to this tile...

        bool _should_pulse = false;

        RiskySandBox_HumanPlayer _LocalPlayer = RiskySandBox_HumanPlayer.local_player;

        if(_LocalPlayer == null)
        {
            this.enable_behaviour.value = false;
            return;
        }

        //if we can deploy to this tile???
        _should_pulse = RiskySandBox_MainGame.instance.canDeploy(_LocalPlayer.my_Team, this.my_Tile, 1);
        _should_pulse |= RiskySandBox_MainGame.instance.canAttack(_LocalPlayer.attack_start, this.my_Tile);
        





        this.enable_behaviour.value = _should_pulse;


    }




}
