using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_Team
{

    /// <summary>
    /// called whenever a Team.deployable_troops.value changes....
    /// </summary>
    public static event Action<RiskySandBox_Team> OnVariableUpdate_deployable_troops_STATIC;



    public ObservableInt deployable_troops { get { return PRIVATE_deployable_troops; } }
    [SerializeField] private ObservableInt PRIVATE_deployable_troops;



    public void OnVariableUpdate_deployable_troops(ObservableInt _deployable_troops)
    {
        //tell everyone that this changed...
        OnVariableUpdate_deployable_troops_STATIC?.Invoke(this);
    }


}
