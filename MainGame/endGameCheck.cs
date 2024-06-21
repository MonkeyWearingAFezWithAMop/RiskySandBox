using System.Collections;using System.Collections.Generic;using System.Linq;using System;


public partial class RiskySandBox_MainGame
{


    protected virtual void endGameCheck()
    {
        int _n_undefeated_Teams = RiskySandBox_Team.undefeated_Teams.Count();

        if (this.debugging)
            GlobalFunctions.print("checking if we should end the game... _n_undefeated_Teams = " + _n_undefeated_Teams, this);
        //if there is only 1 team left?


        HashSet<RiskySandBox_Team> _winners = new HashSet<RiskySandBox_Team>(); //hashset so dont worry about duplication (because of trggering multiple win conditions)

        if (_n_undefeated_Teams == 1)//if there is only 1 team left?
        {
            _winners.Add(RiskySandBox_Team.undefeated_Teams[0]);
        }

        foreach (RiskySandBox_Team _Team in RiskySandBox_Team.undefeated_Teams)
        {
            //great lets check if they have the world domination %...
            int _n_Tiles_Team = _Team.n_Tiles;
            int _n_Tiles_total = RiskySandBox_Tile.all_instances.Count;

            float _percentage = 100 * (_n_Tiles_Team / _n_Tiles_total);

            if (_percentage >= this.PRIVATE_world_domination_percentage)
            {
                //ok this team is a winner!
                _winners.Add(_Team);
            }

            else
            {
                //if they have one because they have captured all the 'capitals'
                int _n_capitals_Team = _Team.n_capitals;
                int _n_capitals_total = RiskySandBox_Capital.all_instances.Count;

                if (_n_capitals_total > 0)
                {
                    float _capitals_percentage = _n_capitals_Team / _n_capitals_total;
                    if (_capitals_percentage >= RiskySandBox_MainGame_CapitalsMode.capital_conquest_percentage.value)//if they have captured enough capitals in order to win...
                    {
                        _winners.Add(_Team);//add them to the winners 
                    }
                }

                //TODO - we need a way for other users to create endGameConditions if they implement their own game mode(s) func? or perhaps we expose endGame as a public function that other people can call with their code?
                //they will have to pass in a list of winners (and everything that is needed for the endGame scene...)

            }
        }

        if (_winners.Count > 0)
        {
            //end the game...
            endGame();//TODO - pass in (or "save") the winners so that the "endgame scene" can show everyone who won!
        }




    }


}