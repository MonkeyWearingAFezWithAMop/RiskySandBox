using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_EndGameScene : MonoBehaviour
{

    public static int scene_ID = 6;

    [SerializeField] bool debugging;


    
    private void Start()
    {
        //fantastic! - lets disconnect from photon...
        Photon.Pun.PhotonNetwork.Disconnect();





    }

    public void exitToMainMenu()
    {
        //go to the main menu...
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
}
