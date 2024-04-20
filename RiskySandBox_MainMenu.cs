using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_MainMenu : MonoBehaviour
{
    [SerializeField] bool debugging;

    [SerializeField] GameObject root_ui;
    [SerializeField] int level_editor_scene_id;

    [SerializeField] UnityEngine.UI.Button play_Button;

    [SerializeField] UnityEngine.UI.InputField nickname_InputField;



    public void enable()
    {
        //enable the root ui!
        root_ui.SetActive(true);

    }

    public void disable()
    {
        //disable the root ui!
        root_ui.SetActive(false);
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




  


    public void levelEditor()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(level_editor_scene_id);
    }


    public void quitGame()
    {
        Application.Quit();
    }




}
