using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox : MonoBehaviour
{
    public static RiskySandBox instance { get; private set; }
    [SerializeField] bool debugging;







    private void Awake()
    {
        instance = this;
    }



}
