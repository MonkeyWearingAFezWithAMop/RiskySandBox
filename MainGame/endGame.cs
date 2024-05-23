using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_MainGame
{

    public static event Action OnendGame_MultiplayerBridge;
    public static event Action OnendGame;
    public UnityEngine.Events.UnityEvent OnendGame_Inspector;



    public void endGame()
    {

        if (this.debugging)
            GlobalFunctions.print("ending the game!", this);


        if (PrototypingAssets.run_server_code.value == true)
        {
            //end the game!

            OnendGame_MultiplayerBridge?.Invoke();
        }


        this.game_setup_UI.SetActive(false);
        RiskySandBox_Tile.destroyAllTiles();

        OnendGame_Inspector.Invoke();
        OnendGame?.Invoke();

        

    }
}
