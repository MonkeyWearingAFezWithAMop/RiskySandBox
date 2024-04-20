using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_Capital : MonoBehaviour
{

    public static List<RiskySandBox_Capital> all_instances = new List<RiskySandBox_Capital>();

    //a capital seems to be a special object that "defends" better i dont quite understand how this works...


    [SerializeField] bool debugging;

    private void OnEnable()
    {
        all_instances.Add(this);
    }

    private void OnDisable()
    {
        all_instances.Remove(this);
    }

}
