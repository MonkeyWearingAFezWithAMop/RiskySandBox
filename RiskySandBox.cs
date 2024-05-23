using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox : MonoBehaviour
{

    public static string maps_folder_path { get { return Application.streamingAssetsPath + "/RiskySandBox/Maps"; } }


    public static RiskySandBox instance { get; private set; }
    [SerializeField] bool debugging;


    public ObservableString connect_room_name { get { return PRIVATE_connect_room_name; } }
    [SerializeField] ObservableString PRIVATE_connect_room_name;


    public ObservableFloat current_time { get { return PRIVATE_current_time; } }
    [SerializeField] ObservableFloat PRIVATE_current_time;


    [SerializeField] ObservableBool enable_multiplayer;



    void Awake()
    {
        instance = this;
        //Debug.LogError("hello pun console");
    }


    public void createMultiplayerRoomFromUI()
    {
        createRoom(connect_room_name.value);
    }

    public void createSinglePlayerRoomFromUI()
    {
        Photon.Pun.PhotonNetwork.OfflineMode = true;
        Photon.Pun.PhotonNetwork.CreateRoom("single player room!");
    }

    public void joinRoomFromUI()
    {
        Photon.Pun.PhotonNetwork.JoinRoom(connect_room_name.value);
    }

    void createRoom(string _room_name)
    {
        Photon.Pun.PhotonNetwork.CreateRoom(_room_name);
    }

    void joinRoom(string _room_name)
    {
        Photon.Pun.PhotonNetwork.JoinRoom(_room_name);
    }

    public void exitCurrentRoom()
    {
        if (Photon.Pun.PhotonNetwork.InRoom == false)
            return;
        Photon.Pun.PhotonNetwork.LeaveRoom();
    }




    private void Start()
    {
        connectToPhoton();
    }


    private void Update()
    {
        this.current_time.value += Time.deltaTime;
    }



    void connectToPhoton()
    {
        if (this.enable_multiplayer.value == false)
            return;

        if (Photon.Pun.PhotonNetwork.IsConnected == false)
            Photon.Pun.PhotonNetwork.ConnectUsingSettings();
        
        Invoke("connectToPhoton", 2f);//make sure to always try and connect... this is useful if for some reason connection to photon drops for some reason...
    }

}
