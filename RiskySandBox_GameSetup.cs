using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Photon.Pun;


//this scene is here for the player to decide what type of game they want to play and the general settings of the game...
//the player will then hit the play button(s) to get loaded into the RiskySandBox_GameLobby scene where other players can join and Team related settings can be applied?




public partial class RiskySandBox_GameSetup : MonoBehaviourPunCallbacks
{
    [SerializeField] bool debugging;
    [SerializeField] int lobby_scene_ID;


    [SerializeField] UnityEngine.UI.InputField room_name_InputField;
    [SerializeField] UnityEngine.UI.Button create_multiplayer_Button;



    public void playSP()
    {
        Photon.Pun.PhotonNetwork.OfflineMode = true;
        Photon.Pun.PhotonNetwork.CreateRoom("SinglePlayer");
    }


    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Photon.Pun.PhotonNetwork.LoadLevel(lobby_scene_ID);
    }


    private void Start()
    {
        create_multiplayer_Button.onClick.AddListener(playMP);
        PhotonNetwork.ConnectUsingSettings();
        //
        RiskySandBox_MainGame.instance.game_setup_UI.SetActive(true);
    }

    private void Update()
    {
        create_multiplayer_Button.interactable = PhotonNetwork.IsConnected && !string.IsNullOrEmpty(room_name_InputField.text);
    }


    private void OnDestroy()
    {
        RiskySandBox_MainGame.instance.game_setup_UI.SetActive(false);
    }


    public void playMP()
    {
        Photon.Pun.PhotonNetwork.CreateRoom(room_name_InputField.text);
    }


    public void returnToMainMenu()
    {
        PhotonNetwork.Disconnect();
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    
}
