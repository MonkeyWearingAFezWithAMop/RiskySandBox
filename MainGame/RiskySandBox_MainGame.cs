using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_MainGame
{

    


    public static RiskySandBox_MainGame instance;

    //multiplayer events...
    public static event Action<int,int> OnSET_num_troops_MultiplayerBridge;
    public static event Action<int,int> OnSET_my_Team_MultiplayerBridge;


    public static event Action<RiskySandBox_Tile> OnSET_num_troops;
    public static event Action<RiskySandBox_Tile> OnSET_my_Team;

    public static event Action OnclearMap;

    /// <summary>
    /// called whenever a Team "trades in" territory cards... (the team AND the number of troops they got
    /// </summary>
    public static event Action<RiskySandBox_Team,int> OnterritoryCardTrade;

    public ObservableList<RiskySandBox_Team> ai_Teams = new ObservableList<RiskySandBox_Team>();



    

    public ObservableFloat turn_length_seconds;
    public ObservableString map_ID { get { return this.PRIVATE_map_ID; } }
    public ObservableBool game_started { get { return PRIVATE_game_started; } }
    public ObservableInt n_bots { get { return this.PRIVATE_n_bots; } }
    public GameObject game_setup_UI { get { return PRIVATE_game_setup_UI; } }



    public ObservableInt progressive_trade_in_increment;

    //to begin with you would do something like 4,6,8,10,12,15 (then we would have 15 + increment, 15 + 2* increment and so on...) 
    public ObservableString progressive_trade_in_start_string;

    public ObservableInt progressive_trade_in_count;//each time a progressive trade in happens... we increment this by 1...

    public int calculateProgressiveTradeIn(int _n)
    {

        if(progressive_trade_in_start_string.value == "")
        {
            return progressive_trade_in_increment * (_n + 1);
        }


        //so first look at the string!
        List<int> _start_values = progressive_trade_in_start_string.value.Split(',').Select(x => int.Parse(x)).ToList();

        if(_start_values.Count == 0)
        {
            return progressive_trade_in_increment * (_n + 1);
        }

        if (_n >= _start_values.Count)
        {
            return _start_values[_start_values.Count - 1] + progressive_trade_in_increment * (_n - _start_values.Count());
        }

        return _start_values[_n];
        



    }



    public ObservableInt num_wildcards { get { return this.PRIVATE_num_wildcards; } }

    public ObservableInt max_num_cards { get { return this.PRIVATE_max_num_cards; } }

    

    public ObservableInt n_stable_portals { get { return this.PRIVATE_n_stable_portals;} }
    public ObservableInt n_unstable_portals { get { return this.PRIVATE_n_unstable_portals; } }
    public ObservableInt n_blizards { get { return this.PRIVATE_n_blizards; } }

    public ObservableBool display_bonuses { get { return this.PRIVATE_display_bonuses; } }


    public ObservableBool enable_territory_cards { get { return this.PRIVATE_enable_territory_cards; } }
    public ObservableString territory_card_mode { get { return this.PRIVATE_territory_card_mode; } }

    /// <summary>
    /// how much time does the team "get back" after capturing...
    /// </summary>
    public ObservableFloat capture_increment { get { return this.PRIVATE_capture_increment; } }//TODO - rename this to capture_turn_timer_increment????

    /// <summary>
    /// how many troops does each team start the game with...
    /// </summary>
    public ObservableInt num_troops_startGame { get { return this.PRIVATE_n_troops_startGame; } }



    //if the map hasnt been loaded yet? - we need to buffer these values so that once the map has been loaded... the Tiles can be updated with their Teams and num troops...
    public Dictionary<int, int> num_troops_buffer = new Dictionary<int, int>();
    public Dictionary<int, int> team_buffer = new Dictionary<int, int>();


    public Transform tile_parent_Transform { get { return this.transform; } }
    public Transform bonus_parent_Transform { get { return this.transform; } }
    
    public ObservableBool show_escape_menu { get { return this.PRIVATE_show_escape_menu; } }

    List<RiskySandBox_Team> turn_order { get { return RiskySandBox_Team.all_instances.Where(x => x != null && x.defeated.value == false).OrderByDescending(x => x.ID.value).Reverse().ToList(); } }



    public void clearMap()
    {
        OnclearMap?.Invoke();
        RiskySandBox_Tile.destroyAllTiles();
        RiskySandBox_Bonus.destroyAllBonuses();
    }

    

    public void SET_num_troops(int _Tile_ID,int _n_troops)
    {
        
        

        if(PrototypingAssets.run_server_code.value == true)//if we are the server...
        {
            //ok! - lets tell all the connected client(s) that this tile has a new number of troops!
            OnSET_num_troops_MultiplayerBridge?.Invoke(_Tile_ID, _n_troops);
        }

        RiskySandBox_Tile _Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(_Tile_ID);//get the TIle with that id....

        if (_Tile != null)
        {

            _Tile.num_troops.value = _n_troops;//ok great lets just set it and move on

            RiskySandBox_MainGame.OnSET_num_troops?.Invoke(_Tile);//let everyone know that we just changed the number of troops on a tile...

            return;
        }

        else
        {
            //ok that is weird??? but alright... theoretically the map maybe hasn't been loaded? - so lets buffer it and wait for the Map to get loaded////
            num_troops_buffer[_Tile_ID] = _n_troops;
        }

    }

    public void SET_my_Team(int _Tile_ID,int _Team_ID)
    {
        //get the TIle with that id....
        //if none?
        //ok... this is fine what we will do is add it to the buffer...
        RiskySandBox_Tile _Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(_Tile_ID);

        if (PrototypingAssets.run_server_code == true)
            OnSET_my_Team_MultiplayerBridge?.Invoke(_Tile_ID, _Team_ID);//tell all connected clients that a Tile has a new Team...

        if (_Tile != null)
        {

            _Tile.my_Team_ID.value = _Team_ID;

            RiskySandBox_MainGame.OnSET_my_Team?.Invoke(_Tile);

            return;
        }

        else
            {
            //we need to update the buffer so that once that map is loaded this can be pushed through...
        }


    }










    void TeamEventReceiver_OnturnTimerReachedZero(RiskySandBox_Team _Team)
    {
        if (PrototypingAssets.run_server_code == false)
            return;
        //TODO - we must do some checks...
        //make sure all the troops have been deployed...
        //if there are still some we must deploy them automatically?
        
        this.endTurn(_Team, "turn timer ran out");//end current teams turn...
        //TODO - end of game check...
        RiskySandBox_Team _next_Team = GET_nextTeam(_Team);
        if (_next_Team != null)
            this.startTurn(_next_Team);
        
    }




    protected virtual void endTurn(RiskySandBox_Team _Team,string _debug_reason)
    {
        if (this.debugging)
            GlobalFunctions.print("ending the teams turn..." + _debug_reason, _Team,_Team,_debug_reason);

        

        //if territory cards are enabled? AND the team has caputred a Tile...
        if(RiskySandBox_MainGame.instance.enable_territory_cards.value == true && _Team.has_captured_Tile == true)
        {
            //give them a card! (if any)
            List<int> _availible_cards = RiskySandBox_Tile.all_instances.Select(x => x.ID.value).ToList();


            int _n_wilds = RiskySandBox_MainGame.instance.num_wildcards;

            foreach(RiskySandBox_Team _Team1 in RiskySandBox_Team.all_instances)
            {
                if(_Team == null)
                {
                    //TODO - WTF?!?!?!? - really this isnt my problem so maybe lets just not worry?
                    continue;
                }

                _n_wilds -= _Team1.territory_card_IDs.Where(x => x == RiskySandBox_TerritoryCard.wildcard_ID).Count();

                foreach(int _ID in _Team1.territory_card_IDs)
                {
                    //remove from availbile cards
                    if (_ID == RiskySandBox_TerritoryCard.wildcard_ID)
                        continue;
                    _availible_cards.Remove(_ID);
                }
            }


            if (_n_wilds > 0 || _availible_cards.Count() > 0)
            {
                int _random_card = RiskySandBox_TerritoryCard.drawRandomCard(_availible_cards, _n_wilds);

                _Team.addTerritoryCard(_random_card);
            }

        }


        _Team.has_captured_Tile.value = false;
        _Team.current_turn_state.value = RiskySandBox_Team.turn_state_waiting;
        _Team.deployable_troops.value = 0;
        _Team.is_my_turn.value = false;
        
    }

    public RiskySandBox_Team GET_nextTeam(RiskySandBox_Team _current_Team)
    {

        List<RiskySandBox_Team> _turn_order = new List<RiskySandBox_Team>(turn_order);
        if (_current_Team == null)
            return _turn_order[0];


        int _current_index = _turn_order.IndexOf(_current_Team);

        _current_index += 1;

        if (_current_index >= _turn_order.Count)
            _current_index = 0;

        return _turn_order[_current_index];


    }



    /// <summary>
    /// put the team into the next state...
    /// </summary>
    public void goToNextTurnState(RiskySandBox_Team _Team)
    {
        //waiting -> deploy -> attack -> fortify -> waiting -> deploy -> attack -> fortify -> waiting (and so on...)

        string _current_state = _Team.current_turn_state.value;

        

        if(_current_state == RiskySandBox_Team.turn_state_deploy)// deploy -> attack...
        {

            if(_Team.deployable_troops.value > 0)
            {
                if (this.debugging)
                    GlobalFunctions.print("staying in the deploy state (the Team still had deployable troops...",this);
                //nope dont allow this...
                return;
            }
            if (this.debugging)
                GlobalFunctions.print("putting the team into the attack state",this);

            _Team.current_turn_state.value = RiskySandBox_Team.turn_state_attack;
        }

        else if(_current_state == RiskySandBox_Team.turn_state_attack)//attack -> fortify...
        {
            if (this.debugging)
                GlobalFunctions.print("putting the team into the fortify state", this);
            _Team.current_turn_state.value = RiskySandBox_Team.turn_state_fortify;
        }

        else if(_current_state == RiskySandBox_Team.turn_state_capture)//capture -> attack
        {
            //not allowed...
        }

        else if(_current_state == RiskySandBox_Team.turn_state_fortify)//foritfy -> next team turn...
        {
            if (this.debugging)
                GlobalFunctions.print("ending the teams turn and starting the next teams turn", this);

            RiskySandBox_Team _next_Team = GET_nextTeam(_Team);

            endTurn(_Team,"fortify -> next team turn");

            startTurn(_next_Team);
            
        }

    }


    public bool isValidTrade(List<int> _card_IDs)
    {
        if (RiskySandBox_MainGame.instance.enable_territory_cards == false)
            return false;

        string _card_mode = RiskySandBox_MainGame.instance.territory_card_mode;
        int _n_wilds = _card_IDs.Where(x => x == RiskySandBox_TerritoryCard.wildcard_ID).Count();

        if (_card_mode == RiskySandBox_TerritoryCard.fixed_mode || _card_mode == RiskySandBox_TerritoryCard.progressive_mode)
        {
            //make sure there are 3 cards!
            if (_card_IDs.Count != 3)
                return false;

            HashSet<int> _card_types = new HashSet<int>(_card_IDs.Where(x => x != RiskySandBox_TerritoryCard.wildcard_ID).Select(x => x % 3));

            if(_n_wilds == 0)
            {
                if (this.debugging)
                    GlobalFunctions.print("n wilds == 0 _card_tyoes.Count() == " + _card_types.Count(),this);
                //all are the same type... or they are all different types...
                return _card_types.Count() == 1 || _card_types.Count() == 3;
            }
            if(_n_wilds == 1)
            {
                if (this.debugging)
                    GlobalFunctions.print("n wilds == 1 _card_tyoes.Count() == " + _card_types.Count(), this);
                //if there is only 1 type? or there are 2 types?
                return _card_types.Count() == 1 || _card_types.Count() == 2;
            }
            if(_n_wilds == 2)
            {
                if (this.debugging)
                    GlobalFunctions.print("n wilds == 2 _card_tyoes.Count() == " + _card_types.Count(), this);
                return _card_types.Count() == 1;
            }

            if(_n_wilds == 3)
            {
                if (this.debugging)
                    GlobalFunctions.print("n wilds == 3", this);
                return true;
            }

            GlobalFunctions.printError("WTF?!?!??!?!",this);
            return false;





        }
        //presumably you (the person reading this) is trying to implement your own "territory card" algoriythm please read the below instructions...
        //TODO - put in some detailed instructions here to help people do this...
        //https://github.com/MonkeyWearingAFezWithAMop

        GlobalFunctions.printError("UNIMPLEMENTED!!!!",this);
        return false;
    }

    public void TRY_tradeInCards(RiskySandBox_Team _Team, IEnumerable<int> _card_IDs)
    {
        //make sure the team has cards with these ids...
        if (RiskySandBox_MainGame.instance.enable_territory_cards == false)
        {
            if (this.debugging)
                GlobalFunctions.print("RiskySandBox_MainGame.instance.enable_territory_cards == false",this);
            return;
        }

        //TODO - does the team have to be in the "deploy" state? or the force trade in state?
        //the other alternative is the deploy state || they have 5? cards...

        


        List<int> _card_IDs_List = new List<int>(_card_IDs);


        if(_card_IDs_List.Count() <= 0)
        {
            //no...
            if (this.debugging)
                GlobalFunctions.print("_card_IDs.Count() <= 0?!?!?!",this);
            return;
        }


        foreach(int _ID in _card_IDs_List)
        {
            //if the team doesnt have the required ids...
            int _count = _card_IDs_List.Where(x => x == _ID).ToList().Count();

            int _Team_Count = _Team.territory_card_IDs.Where(x => x == _ID).ToList().Count();

            if (_count > _Team_Count)
            {
                if(debugging)
                    GlobalFunctions.print("the team doesnt have enough of the cards with the id = " + _ID, this);
                return;
            }

        }

        int _bonus_troops = 0;

        if(RiskySandBox_MainGame.instance.territory_card_mode == RiskySandBox_TerritoryCard.progressive_mode)
        {
            _bonus_troops = RiskySandBox_MainGame.instance.calculateProgressiveTradeIn(RiskySandBox_MainGame.instance.progressive_trade_in_count);

            

            RiskySandBox_MainGame.instance.progressive_trade_in_count.value += 1;

        }

        if(RiskySandBox_MainGame.instance.territory_card_mode == RiskySandBox_TerritoryCard.fixed_mode)
        {
            int _troops = 0;

            _Team.deployable_troops.value += _troops;
        }

        foreach(int _card_id in _card_IDs_List)
        {
            //remove this card from the team...
            _Team.removeTerritoryCard(_card_id);
        }

        //give the team the deployable troops!
        _Team.deployable_troops.value += _bonus_troops;

        RiskySandBox_MainGame.OnterritoryCardTrade?.Invoke(_Team, _bonus_troops);//tell everyone a trade in just happened...


        //if they are in the force trade in state????
        if (_Team.current_turn_state == RiskySandBox_Team.turn_state_force_trade_in)
        {
            if(_Team.num_cards < RiskySandBox_MainGame.instance.max_num_cards)
            {
                //TODO - nope! - we want to instead say something like _Team.TRY_enterDeployState() - this is because of capitals (or other turn states) may need to be dealt with before entering the deploy state...
                //say for example a player started the game with 5 cards and needed to place a capital! - they would first need to place the capital... play the 5 cards then go into deploy state!
                //obviously this isnt how the standard rules work... but this game is a sandbox mode where many unusual things can happen!
                _Team.current_turn_state.value = RiskySandBox_Team.turn_state_deploy;
            }
        }

    }




}



