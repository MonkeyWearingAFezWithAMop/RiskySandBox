using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_ProgressiveCardsMenu : MonoBehaviour
{
    [SerializeField] bool debugging;


    [SerializeField] List<UnityEngine.UI.Text> tradein_Texts = new List<UnityEngine.UI.Text>();


    private void Awake()
    {
        RiskySandBox_MainGame.instance.progressive_trade_in_count.OnUpdate += EventReceiver_OnVariableUpdate_progressive_trade_in_count;
    }

    private void OnDestroy()
    {
        
    }

    private void Start()
    {
        updateUI();
    }


    void EventReceiver_OnVariableUpdate_progressive_trade_in_count(ObservableInt _progressive_trade_in_count)
    {
        updateUI();
    }


    void updateUI()
    {
        //update the texts
        for (int i = 0; i < tradein_Texts.Count; i += 1)
        {
            UnityEngine.UI.Text _Text = tradein_Texts[i];
            if (_Text == null)
                continue;

            _Text.text = "" + RiskySandBox_MainGame.instance.calculateProgressiveTradeIn(RiskySandBox_MainGame.instance.progressive_trade_in_count + i);
        }
    }


}
