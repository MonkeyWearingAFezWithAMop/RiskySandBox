using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_Resources : MonoBehaviour
{
    public static RiskySandBox_Resources instance;


    public static GameObject human_player_prefab { get { return instance.PRIVATE_human_player_prefab; } }

    public static GameObject Team_prefab { get { return instance.PRIVATE_Team_prefab; } }

    public static GameObject tile_prefab { get { return instance.PRIVATE_tile_prefab; } }

    public static GameObject Bonus_prefab { get { return instance.PRIVATE_bonus_prefab; } }

    public static GameObject fortify_arrow_prefab { get { return instance.PRIVATE_fortify_arrow_prefab; } }

    public static GameObject territory_card { get { return instance.PRIVATE_territory_card; } }



    [SerializeField] bool debugging;
    [SerializeField] GameObject PRIVATE_tile_prefab;
    [SerializeField] private GameObject PRIVATE_human_player_prefab;
    [SerializeField] private GameObject PRIVATE_Team_prefab;
    [SerializeField] private GameObject PRIVATE_bonus_prefab;
    [SerializeField] private GameObject PRIVATE_fortify_arrow_prefab;
    [SerializeField] private GameObject PRIVATE_territory_card;



    private void Awake()
    {
        instance = this;
    }
}
