using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;using UnityEngine.UI;
using Photon.Pun;

public partial class RiskySandBox_PlayMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] bool debugging;

    [SerializeField] int game_setup_scene_id;


    [SerializeField] UnityEngine.UI.InputField join_room_InputField;
    public Button joinButton;



    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;


        // Add listener to the button's onClick event
        joinButton.onClick.AddListener(TRY_joinGame);

        PhotonNetwork.ConnectUsingSettings();
    }


    void Update()
    {
        // Enable the button if connected to PhotonNetwork and if input field text isn't null
        joinButton.interactable = PhotonNetwork.IsConnected && !string.IsNullOrEmpty(join_room_InputField.text);
    }


    public void TRY_createGame()
    {
        PhotonNetwork.Disconnect();
        UnityEngine.SceneManagement.SceneManager.LoadScene(game_setup_scene_id);
    }



    /// <summary>
    /// public so we can just use a unityengine button...
    /// </summary>
    public void TRY_joinGame()
    {
        
        string roomName = join_room_InputField.text;

        if (!string.IsNullOrEmpty(roomName))
        {
            Photon.Pun.PhotonNetwork.JoinRoom(roomName);
        }
        else
        {
            Debug.LogError("Room name cannot be empty!");
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Failed to join room: " + message);
    }
}
