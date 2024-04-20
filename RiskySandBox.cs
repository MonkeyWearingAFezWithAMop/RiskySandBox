using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox : MonoBehaviour
{
    public static RiskySandBox instance { get; private set; }
    [SerializeField] bool debugging;


    public ObservableString connect_room_name { get { return PRIVATE_connect_room_name; } }
    [SerializeField] ObservableString PRIVATE_connect_room_name;


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



    private void Awake()
    {
        instance = this;
    }


    private void Start()
    {
        Photon.Pun.PhotonNetwork.ConnectUsingSettings();
    }



}
