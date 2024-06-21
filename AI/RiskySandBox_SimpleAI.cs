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

        if(_Team_Tiles.Count() <= 0)
        {
            //TODO - WTF why is this happening...
            //TODO - this is probably? an error where the game has been setup however the number of tiles on the map is less than the number of teams currently playing...
            //TODO - add a debug statement here...
            return;
        }


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
        //TODO - trade in such a way as to maximize the number of troops we get?
        //TODO - wait untill we are at 5? cards or instantly trade in always...
        //TODO - if we are in fixed cards mode? - ALWAYS trade in if you have 1 soldier, 1 cavalry, 1 cannon - otherwise we may "die" before we get a chance to trade in?


        //TODO - if we are allowed to "deploy" to allies? and we have an ally and we have no reason to deploy (cant attack any tiles?) we may aswell give the troops to our human? ally...
        //TODO priorise deploying to human allies...

        if (_Team.current_turn_state == RiskySandBox_Team.turn_state_deploy)
        {

            if (this.debugging)
                GlobalFunctions.print("doing deploy code!",this);

            Dictionary<RiskySandBox_Tile, int> _planned_deploys = new Dictionary<RiskySandBox_Tile, int>();

            int _n_deploys = _Team.deployable_troops;
            for (int i = 0; i < _n_deploys; i += 1)
            {
                int _random_Tile_index = GlobalFunctions.randomInt(0, _Team_Tiles.Count - 1);

                RiskySandBox_Tile _random_Tile = _Team_Tiles[_random_Tile_index];

                if(_planned_deploys.TryAdd(_random_Tile,1) == false)
                {
                    _planned_deploys[_random_Tile] += 1;
                }
            }

            foreach(KeyValuePair<RiskySandBox_Tile,int> _KVP in _planned_deploys)
            {
                RiskySandBox_MainGame.instance.deploy(_Team, _KVP.Key, _KVP.Value);
            }

            RiskySandBox_MainGame.instance.goToNextTurnState(_Team);//enter the attack state?
        }
            

        if(_Team.current_turn_state == RiskySandBox_Team.turn_state_attack)
        {
            if (this.debugging)
                GlobalFunctions.print("doing attack code!", this);

            //TODO - do a simple attack?
            //TODO - NOTE if we do a attack and we capture a tile we must "capture" the tile 
            RiskySandBox_MainGame.instance.goToNextTurnState(_Team);//enter the "fortify state"
        }

        if(_Team.current_turn_state == RiskySandBox_Team.turn_state_fortify)
        {
            if (this.debugging)
                GlobalFunctions.print("doing fortify code!", this);

            //TODO - foritfy troops out of the "corners"
            //TODO - fortify in such a way as to protect captured bonuses
            RiskySandBox_MainGame.instance.goToNextTurnState(_Team);//end my turn!
        }






    }
}
