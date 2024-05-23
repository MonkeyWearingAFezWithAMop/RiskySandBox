using System.Collections;using System.Collections.Generic;using System.Linq;using System;


public partial class RiskySandBox_AttackSimulations
{
    public static string capitals_mode_string { get { return "capital"; } }
    public static string zombies_mode_string { get { return "apocalypse"; } }


    



    public static void doBattle(int _n_attackers,int _n_defenders,string _simulation_mode, out int _remaining_attackers,out int _remaining_defenders)
    {
        _remaining_attackers = _n_attackers;
        _remaining_defenders = _n_defenders;


        if (_simulation_mode == capitals_mode_string)
        {
            //basically the exact same thing but importantly if _remaining_defenders > 2 they get a bonus dice???? - is this a "free" dice roll

        }

        while(_remaining_attackers > 0 && _remaining_defenders > 0)
        {
            //roll 3 dice for the attacker....
            int _n_attacker_rolls = Math.Min(_remaining_attackers, 3);
            int _n_defender_rolls = Math.Min(_remaining_defenders, 2);

            List<int> _attacker_dice_rolls = GlobalFunctions.randomInts(_n_attacker_rolls, 1, 6).OrderByDescending(x => x).ToList();
            List<int> _defender_dice_rolls = GlobalFunctions.randomInts(_n_defender_rolls, 1, 6).OrderByDescending(x => x).ToList();


            //ok great now its a sort of penalty shoot out..

            int _max_i = Math.Min(_n_attacker_rolls, _n_defender_rolls);

            for(int i = 0; i < _max_i; i += 1)
            {
                if (_attacker_dice_rolls[i] > _defender_dice_rolls[i])
                    _remaining_defenders -= 1;
                else
                    _remaining_attackers -= 1;
            }


        }

    }




    public static float calculate(int attNumInput, int defNumInput, string modeType, bool _balanced_blitz)//TODO - how to implement other types of dice?
    {
        float _probability = getProbability(attNumInput - 1, defNumInput, modeType);

        if (_balanced_blitz)//if using the "balanced blitz mode"
            _probability = (MathF.Pow(_probability, 1.3f) / (MathF.Pow(_probability, 1.3f) + MathF.Pow((1f - _probability), 1.3f)) - 0.1f) / 0.8f;//magic forumula?

        if (_probability > 1)
            return 1f;

        if (_probability < 0)
            return 0;

        return _probability;
    }

    static float getProbability(int attNum, int defNum, string modeType)
    {
        int arrayLength = attNum + 2;
        int arrayWidth = defNum + 1;

        float[,] array = new float[arrayLength, arrayWidth];
        //for (int i = 0; i < arrayLength; ++i)
        //{
        //    array[i] = new List<float>(); ;
        //}

        // normal odds
        float a1v1 = 15f / 36;
        float d1v1 = 21f / 36;
        float a1v2 = 55f / 216;
        float d1v2 = 161f / 216;
        float a2v1 = 125f / 216;
        float d2v1 = 91f / 216;
        float a3v1 = 855f / 1296;
        float d3v1 = 441f / 1296;
        float a2v2 = 295f / 1296;
        float d2v2 = 581f / 1296;
        float ad2v2 = 420f / 1296;
        float a3v2 = 2890f / 7776;
        float d3v2 = 2275f / 7776;
        float ad3v2 = 2611f / 7776;

        // apocalypse mode odds
        if (modeType == zombies_mode_string)
        {
            a1v1 = 21f / 36;
            d1v1 = 15f / 36;
            a1v2 = 91f / 216;
            d1v2 = 125f / 216;
            a2v1 = 161f / 216;
            d2v1 = 55f / 216;
            a3v1 = 119f / 144;
            d3v1 = 25f / 144;
            a2v2 = 581f / 1296;
            d2v2 = 295f / 1296;
            ad2v2 = 420f / 1296;
            a3v2 = 4816f / 7776;
            d3v2 = 979f / 7776;
            ad3v2 = 1981f / 7776;
        }

        // capital mode odds
        float a1v3 = 25f / 144;
        float d1v3 = 119f / 144;
        float a2v3 = 979f / 7776;
        float ad2v3 = 1981f / 7776;
        float d2v3 = 4816f / 7776;
        float a3v3 = 6420f / 46656;
        float a2d3v3 = 10017f / 46656;
        float d2a3v3 = 12348f / 46656;
        float d3v3 = 17871f / 46656;

        // 0 defenders
        for (int a = 0; a < arrayLength; ++a)
        {
            array[a, 0] = 1;
        }

        // 0 attackers
        for (int d = 0; d < arrayWidth; ++d)
        {
            array[0, d] = 0;
        }

        // 1 attacker 1 defender
        array[1, 1] = a1v1;

        // 2 attackers 1 defender
        array[2, 1] = 1 - d2v1 * d1v1;

        // 3+ attackers 1 defender
        for (int a = 3; a < arrayLength; ++a)
        {
            array[a, 1] = a3v1 + d3v1 * array[a - 1, 1];
        }

        // 1 attacker 2+ defenders
        for (int d = 2; d < arrayWidth; ++d)
        {
            array[1, d] = a1v2 * array[1, d - 1];
        }

        // 2 attackers 2+ defenders
        for (int d = 2; d < arrayWidth; ++d)
        {
            array[2, d] = a2v2 * array[2, d - 2] + ad2v2 * array[1, d - 1];
        }

        // 3+ attackers 2+ defenders
        for (int a = 3; a < arrayLength; ++a)
        {
            for (int d = 2; d < arrayWidth; ++d)
            {
                array[a, d] = a3v2 * array[a, d - 2] + ad3v2 * array[a - 1, d - 1] + d3v2 * array[a - 2, d];
            }
        }

        if (modeType == capitals_mode_string)
        {
            // 1 attacker 3+ defenders
            for (int d = 3; d < arrayWidth; ++d)
            {
                array[1, d] = a1v3 * array[1, d - 1];
            }

            // 2 attackers 3+ defenders
            for (int d = 3; d < arrayWidth; ++d)
            {
                array[2, d] = a2v3 * array[2, d - 2] + ad2v3 * array[1, d - 1];
            }

            // 3+ attackers 3+ defenders
            for (int a = 3; a < arrayLength; ++a)
            {
                for (int d = 3; d < arrayWidth; ++d)
                {
                    array[a, d] = a3v3 * array[a, d - 3] + a2d3v3 * array[a - 1, d - 2] + d2a3v3 * array[a - 2, d - 1] + d3v3 * array[a - 3, d];
                }
            }
        }

        return array[attNum, defNum];
    }
}
