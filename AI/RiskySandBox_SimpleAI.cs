using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_SimpleAI : MonoBehaviour
{
    [SerializeField] bool debugging;




    private void Awake()
    {
        RiskySandBox_Team.OnVariableUpdate_current_turn_state_STATIC += EventReceiver_OnVariableUpdate_current_turn_state_STATIC;    
    }


    //NOTE!!!! - DO NOT USE ANY WHILE LOOPS - why? - because we don't want the game to crash because of ai...
    //remember! - the game is going to be very open to modding and people may include additional states...
    void EventReceiver_OnVariableUpdate_current_turn_state_STATIC(RiskySandBox_Team _Team)
    {

        if(_Team.defeated == true)
        {
            //DEBUG wtf?!?!?!
            return;
        }

        if (RiskySandBox_MainGame.instance.ai_Teams.Contains(_Team) == false)
            return;

        

        List<RiskySandBox_Tile> _Team_Tiles = RiskySandBox_Tile.all_instances.Where(x => x.my_Team == _Team).ToList();


        if(_Team.current_turn_state == RiskySandBox_Team.turn_state_placing_capital)
        {
            //place a capital on a random _Team_Tile...
            int _random_Tile_index = GlobalFunctions.randomInt(0, _Team_Tiles.Count - 1);

            RiskySandBox_Tile _random_Tile = _Team_Tiles[_random_Tile_index];

            RiskySandBox_MainGame.instance.TRY_placeCapital(_random_Tile);
        }


        //TODO - trade in 5? terrotory cards! (or if we enter the force trade in state...)
        // if progressive...    trade in "correctly" - if 3 soldiers, 1 cavalry 1 cannon you "should" trade in the 3 soldiers
        //also try not to trade in a "joker"

        if (_Team.current_turn_state == RiskySandBox_Team.turn_state_deploy)
        {

            if (this.debugging)
                GlobalFunctions.print("doing deploy code!",this);

            int _n_deploys = _Team.deployable_troops;
            for (int i = 0; i < _n_deploys; i += 1)
            {
                int _random_Tile_index = GlobalFunctions.randomInt(0, _Team_Tiles.Count - 1);

                RiskySandBox_Tile _random_Tile = _Team_Tiles[_random_Tile_index];

                RiskySandBox_MainGame.instance.deploy(_Team, _random_Tile, 1);//deploy 1 troop to the random tile...
            }

            RiskySandBox_MainGame.instance.goToNextTurnState(_Team);//enter the attack state?
        }
            

        if(_Team.current_turn_state == RiskySandBox_Team.turn_state_attack)
        {
            if (this.debugging)
                GlobalFunctions.print("doing attack code!", this);

            //TODO - do a simple attack?
            RiskySandBox_MainGame.instance.goToNextTurnState(_Team);//enter the "fortify state"
        }

        if(_Team.current_turn_state == RiskySandBox_Team.turn_state_fortify)
        {
            if (this.debugging)
                GlobalFunctions.print("doing fortify code!", this);

            //TODO - foritfy troops out of the "corners"
            RiskySandBox_MainGame.instance.goToNextTurnState(_Team);//end my turn!
        }






    }
}
