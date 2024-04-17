using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_Tile_TextEmitter : MonoBehaviour
{

    [SerializeField] GameObject emitText_prefab;


    private void OnEnable()
    {
        RiskySandBox_MainGame.OnSET_num_troops += EventReceiver_OnSET_num_troops;
    }

    private void OnDisable()
    {
        RiskySandBox_MainGame.OnSET_num_troops -= EventReceiver_OnSET_num_troops;
    }

    void emitText(RiskySandBox_Tile _Tile, string _text)
    {
        UnityEngine.UI.Text _new_Text = UnityEngine.Object.Instantiate(emitText_prefab).transform.GetChild(0).GetComponent<UnityEngine.UI.Text>();
        _new_Text.gameObject.transform.position = _Tile.transform.position;
        _new_Text.text = _text;
    }

    void EventReceiver_OnSET_num_troops(RiskySandBox_Tile _Tile)
    {
        emitText(_Tile, "" + _Tile.num_troops.delta_value);
    }
      






}
