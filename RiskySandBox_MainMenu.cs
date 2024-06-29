using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_MainMenu : MonoBehaviour
{
    public static RiskySandBox_MainMenu instance;

    [SerializeField] bool debugging;

    public static ObservableBool is_enabled { get { return instance.PRIVATE_is_enabled; } }

    [SerializeField] ObservableBool PRIVATE_is_enabled;

    [SerializeField] GameObject root_ui;

    [SerializeField] UnityEngine.UI.Button play_Button;

    [SerializeField] UnityEngine.UI.InputField nickname_InputField;
    [SerializeField] UnityEngine.UI.Button join_room_Button;



    public UnityEngine.Events.UnityEvent Onenable_Inspector;


    public ObservableBool enable_full_screen;


    private void Awake()
    {
        instance = this;
    }


    public void enable()
    {
        //enable the root ui!
        root_ui.SetActive(true);

        Onenable_Inspector.Invoke();

    }

    public void disable()
    {
        //disable the root ui!
        root_ui.SetActive(false);
    }



    public void EventReceiver_OnVariableUpdate_enable_full_screen(ObservableBool _enable_full_screen)
    {
        Screen.fullScreen = _enable_full_screen;
    }


    public void returnToMainMenu()
    {
        this.enable();
    }
    



    private void Start()
    {

        enable();

        if(Photon.Pun.PhotonNetwork.NickName == "")
        {
            //thats ok.. we shall temporarily make it a username that doesnt matter for now...
            Photon.Pun.PhotonNetwork.NickName = "Player: " + GlobalFunctions.randomInt(0, 1000000);

        }
        
        nickname_InputField.text = Photon.Pun.PhotonNetwork.NickName;
        nickname_InputField.interactable = false;//this has not been implemented yet...
        nickname_InputField.onValueChanged.AddListener(setNickNameFromInputField);
        
    }





    void setNickNameFromInputField(string _incoming_value)
    {
        Photon.Pun.PhotonNetwork.NickName = _incoming_value;
    }


    private void Update()
    {
        this.join_room_Button.interactable = Photon.Pun.PhotonNetwork.IsConnected;
    }



    public void quitGame()
    {
        Application.Quit();
    }




}
