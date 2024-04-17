using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_MainMenu : MonoBehaviour
{
    [SerializeField] bool debugging;
    [SerializeField] int play_menu_scene_id;
    [SerializeField] int level_editor_scene_id;

    [SerializeField] UnityEngine.UI.Button play_Button;

    [SerializeField] UnityEngine.UI.InputField nickname_InputField;



    private void Start()
    {
        Photon.Pun.PhotonNetwork.NickName = "Player: " + GlobalFunctions.randomInt(0, 1000000);
        nickname_InputField.text = Photon.Pun.PhotonNetwork.NickName;
        nickname_InputField.interactable = false;
    }

    private void Update()
    {
        play_Button.interactable = Photon.Pun.PhotonNetwork.NickName != "";
    }

    public void play()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(play_menu_scene_id);
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
