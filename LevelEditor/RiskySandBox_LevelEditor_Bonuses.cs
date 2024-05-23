using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_LevelEditor_Bonuses : MonoBehaviour
{
    [SerializeField] bool debugging;


    [SerializeField] ObservableBool enable_behaviour;


    [SerializeField] ObservableInt PRIVATE_current_bonus_index;


    [SerializeField] ObservableBool PRIVATE_enable_create_bonus_button;


    // Start is called before the first frame update
    void Start()
    {
        this.PRIVATE_current_bonus_index.min_value = 0;
        this.PRIVATE_current_bonus_index.max_value = Math.Max(0, RiskySandBox_Bonus.all_instances.Count - 1);
        updateLevelEditorUIs();
        this.enable_behaviour.OnUpdate += OnVariableUpdate_enable_behaviour;
        this.PRIVATE_current_bonus_index.OnUpdate += OnVariableUpdate_current_bonus_index;
        RiskySandBox_Bonus.all_instances.OnUpdate += EventReceiver_OnBonusListUpdate;

    }

    // Update is called once per frame
    void Update()
    {
        if (this.enable_behaviour == false)
            return;





    }

    public void createNewBonus()
    {
        RiskySandBox_Bonus.createNewBonus();
        this.PRIVATE_current_bonus_index.max_value += 1;
        this.PRIVATE_current_bonus_index.value = RiskySandBox_Bonus.all_instances.Count;
    }


   

    public void updateLevelEditorUIs()
    {
        //we want to enable the ui at the current bonus index and disable all the other uis...
        for (int i = 0; i < RiskySandBox_Bonus.all_instances.Count; i += 1)
        {
            RiskySandBox_Bonus.all_instances[i].show_level_editor_ui.value = false;
        }

        if (RiskySandBox_Bonus.all_instances.Count > 0)
        {
            try
            { 
                RiskySandBox_Bonus.all_instances[this.PRIVATE_current_bonus_index].show_level_editor_ui.value = this.enable_behaviour;
            }
            catch { }
        }

        this.PRIVATE_enable_create_bonus_button.value = this.enable_behaviour && (this.PRIVATE_current_bonus_index == this.PRIVATE_current_bonus_index.max_value);
    }

    void EventReceiver_OnBonusListUpdate()
    {
        this.PRIVATE_current_bonus_index.max_value = RiskySandBox_Bonus.all_instances.Count - 1;
        updateLevelEditorUIs();
    }

    void OnVariableUpdate_enable_behaviour(ObservableBool _enable_behaviour)
    {
        updateLevelEditorUIs();
    }

    void OnVariableUpdate_current_bonus_index(ObservableInt _current_bonus_index)
    {
        updateLevelEditorUIs();
    }



    
}
