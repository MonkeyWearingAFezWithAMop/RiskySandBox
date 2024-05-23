using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_MainGame
{

    public static event Action<int, bool> OnSET_has_capital_MultiplayerBridge;

    public static event Action<RiskySandBox_Tile> OnplaceCapital;

    public void SET_has_capital(RiskySandBox_Tile _Tile,bool _value)
    {
        if (PrototypingAssets.run_server_code.value == true)
            OnSET_has_capital_MultiplayerBridge?.Invoke(_Tile.ID, _value);

        _Tile.has_capital.value = _value;
    }


    public void TRY_placeCapital(RiskySandBox_Tile _Tile)
    {
        //create a capital on that tile...


        if(_Tile.has_capital == true)
        {
            if (this.debugging)
                GlobalFunctions.print("_Tile.has_capital == true... denying request...", this);
            return;
        }

        if(_Tile.my_Team.required_capital_placements <= 0)
        {
            if (this.debugging)
                GlobalFunctions.print("_Tile.my_Team.required_capital_placements <= 0 - denying request!", this, _Tile);
            return;
        }
        _Tile.my_Team.required_capital_placements.value -= 1;
        

        RiskySandBox_MainGame.instance.SET_has_capital(_Tile,true);

        

        int _new_num_troops = _Tile.num_troops.value + RiskySandBox_MainGame.instance.capital_troop_generation;

        RiskySandBox_MainGame.instance.SET_num_troops(_Tile.ID, _new_num_troops);

        RiskySandBox_MainGame.OnplaceCapital?.Invoke(_Tile);





    }




}
