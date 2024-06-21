using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_FortifyArrow : MonoBehaviour
{
    /// <summary>
    /// the time (in seconds) the attack arrows exist for (before self destructing)
    /// </summary>
    public static readonly float arrow_lifetime = 1/2f;

    [SerializeField] bool debugging;

    RiskySandBox_Tile start
    {
        get { return PRIVATE_start; }
        set
        {
            this.PRIVATE_start = value;
            this.transform.position = value.transform.position + value.UI_position;
        }
    }

    [SerializeField] RiskySandBox_Tile PRIVATE_start;

    RiskySandBox_Tile end
    {
        get { return this.PRIVATE_end; }
        set
        {
            this.PRIVATE_end = value;
            
            Vector3 _look_at = value.transform.position + value.UI_position;
            _look_at.y = this.transform.position.y;

            this.transform.LookAt(_look_at);


            this.movement_speed = ((this.start.transform.position + this.start.UI_position) - (this.end.transform.position + this.end.UI_position)).magnitude / arrow_lifetime;
            this.transform.localScale = new Vector3(value.UI_scale_factor, value.UI_scale_factor, value.UI_scale_factor);

            UnityEngine.Object.Destroy(this.gameObject, arrow_lifetime);
        }
    }
    [SerializeField] RiskySandBox_Tile PRIVATE_end;

    float movement_speed = 0f;



    private void Update()
    {
        transform.position += transform.forward * this.movement_speed * Time.deltaTime ;

        
    }


    public static RiskySandBox_FortifyArrow createNew(RiskySandBox_Tile _from, RiskySandBox_Tile _to)
    {
        //instantiate the prefab...
        RiskySandBox_FortifyArrow _new_arrow = UnityEngine.Object.Instantiate(RiskySandBox_Resources.fortify_arrow_prefab).GetComponent<RiskySandBox_FortifyArrow>();

        _new_arrow.start = _from;
        _new_arrow.end = _to;




        return _new_arrow;


    }



}
