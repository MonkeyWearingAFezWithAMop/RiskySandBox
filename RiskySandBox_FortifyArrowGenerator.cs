using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_FortifyArrowGenerator : MonoBehaviour
{
    [SerializeField] bool debugging;


    private void Awake()
    {
        RiskySandBox_MainGame.Onfortify += EventReceiver_Onfortify;
    }

    private void OnDestroy()
    {
        RiskySandBox_MainGame.Onfortify -= EventReceiver_Onfortify;
    }



    void EventReceiver_Onfortify(RiskySandBox_MainGame.EventInfo_Onfortify _EventInfo)
    {
        if(this.debugging)
            GlobalFunctions.print("detected a fortify event!",this);

        //determine the path TODO - (include the path in the eventinfo????)
        List<RiskySandBox_Tile> _path = RiskySandBox_MainGame.findPath(_EventInfo.from, _EventInfo.to);

        //ok now go through the path...
        if(_path == null)
        {
            GlobalFunctions.printWarning("WARNING - why was _path null...", this);
            return;
        }

        for(int i = 0; i < _path.Count() - 1; i += 1)
        {
            RiskySandBox_FortifyArrow.createNew(_path[i], _path[i + 1]);
        }

    }


}
