using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Photon.Pun;


public partial class RiskySandBox_JoinRoomUI : MonoBehaviourPunCallbacks
{
    [SerializeField] bool debugging;

    [SerializeField] GameObject root_ui;
    [SerializeField] UnityEngine.UI.Button join_Button;
    [SerializeField] UnityEngine.UI.InputField room_name_InputField;
	

    // Start is called before the first frame update
    void Start()
    {
        this.disable();
    }


    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        this.disable();
    }

    public void enable()
    {
        //enable the root canvas...
        root_ui.SetActive(true);

    }


    private void Update()
    {
        join_Button.interactable = RiskySandBox.instance.connect_room_name.value != "";
    }

    public void disable()
    {
        root_ui.SetActive(false);
    }



}
