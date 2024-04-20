using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_GameSetupUI : MonoBehaviour
{

    [SerializeField] GameObject root_UI;

    public void enable()
    {
        root_UI.SetActive(true);
    }

    public void disable()
    {
        root_UI.SetActive(false);
    }


    private void Awake()
    {
        RiskySandBox_GameLobby.OnenterLobby += EventReceiver_OnenterLobby; 
    }

    private void Start()
    {
        disable();
    }


    void EventReceiver_OnenterLobby()
    {
        //ok! - lets disable the set up ui?

        this.disable();

    }


}
