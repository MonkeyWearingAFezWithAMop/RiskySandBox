using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_MainGame : MonoBehaviour
{
    public static RiskySandBox_MainGame instance;

    //multiplayer events...
    public static event Action<int,int> OnSET_num_troops_MultiplayerBridge;
    public static event Action<int,int> OnSET_my_Team_MultiplayerBridge;


    //
    public static event Action<RiskySandBox_Tile> OnSET_num_troops;
    public static event Action<RiskySandBox_Tile> OnSET_my_Team;


    [SerializeField] bool debugging;

    public ObservableString map_ID { get { return this.PRIVATE_map_ID; } }


    [SerializeField] ObservableString PRIVATE_map_ID;
    [SerializeField] ObservableBool PRIVATE_capitals_mode;


    public GameObject game_setup_UI { get { return PRIVATE_game_setup_UI; } }

    [SerializeField] GameObject PRIVATE_game_setup_UI;


    public Dictionary<int, List<int>> graph = new Dictionary<int, List<int>>();
    public List<Bonus> bonuses = new List<Bonus>();


    //if the map hasnt been loaded yet? - we need to buffer these values so that once the map has been loaded... the Tiles can be updated with their Teams and num troops...
    public Dictionary<int, int> num_troops_buffer = new Dictionary<int, int>();
    public Dictionary<int, int> team_buffer = new Dictionary<int, int>();


    public Transform tile_parent_Transform { get { return this.transform; } }

    


    List<RiskySandBox_Team> turn_order { get { return RiskySandBox_Team.all_instances.Where(x => x != null && x.defeated.value == false).OrderByDescending(x => x.ID.value).Reverse().ToList(); } }



    

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
            
            _Tile.previous_my_Team = _Tile.my_Team;
            _Tile.my_Team = RiskySandBox_Team.GET_RiskySandBox_Team(_Team_ID);

            RiskySandBox_MainGame.OnSET_my_Team?.Invoke(_Tile);

            return;
        }

        else
        {
            //we need to update the buffer so that once that map is loaded this can be pushed through...
        }


    }





    protected virtual void endGameCheck()
    { 
        int _n_undefeated_Teams = RiskySandBox_Team.undefeated_Teams.Count();

        if (this.debugging)
            GlobalFunctions.print("checking if we should end the game... _n_undefeated_Teams = "+_n_undefeated_Teams, this);
        //if there is only 1 team left?
        
        if(_n_undefeated_Teams <= 1)
        {
            endGame();
        }


       
    }


    private void Awake()
    {
        instance = this;
        RiskySandBox_Team.OnVariableUpdate_defeated_STATIC += delegate { endGameCheck(); };

        this.map_ID.OnUpdate += delegate { this.loadMap(this.map_ID); };
    }



 



    public virtual void startTurn(RiskySandBox_Team _Team)
    {
        _Team.current_turn_state.value = RiskySandBox_Team.turn_state_deploy;
        _Team.deployable_troops.value += calculateTroopGeneration(_Team);
    }

    public virtual void endTurn(RiskySandBox_Team _Team)
    {
        _Team.current_turn_state.value = RiskySandBox_Team.turn_state_waiting;
        _Team.deployable_troops.value = 0;
        
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
                //nope dont allow this...
                return;
            }
            _Team.current_turn_state.value = RiskySandBox_Team.turn_state_attack;
        }

        else if(_current_state == RiskySandBox_Team.turn_state_attack)//attack -> fortify...
        {
            _Team.current_turn_state.value = RiskySandBox_Team.turn_state_fortify;
        }

        else if(_current_state == RiskySandBox_Team.turn_state_capture)//capture -> attack
        {
            //not allowed...
        }

        else if(_current_state == RiskySandBox_Team.turn_state_fortify)//foritfy -> next team turn...
        {

            RiskySandBox_Team _next_Team = GET_nextTeam(_Team);

            endTurn(_Team);

            startTurn(_next_Team);
            
        }

    }


    [Serializable]
    public struct Bonus
    {
        public int generation;
        public List<int> tile_IDs;
    }




}



