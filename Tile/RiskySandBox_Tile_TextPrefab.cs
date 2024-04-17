using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_Tile_TextPrefab : MonoBehaviour
{
    [SerializeField] float duration = 1f;
    [SerializeField] Vector3 movement_direction = new Vector3(0, 1, 1);
    [SerializeField] float movement_speed = 1f;




    private void Start()
    {
        UnityEngine.Object.Destroy(this.gameObject, duration);
    }


    private void Update()
    {
        this.transform.position += movement_direction * this.movement_speed * Time.deltaTime;
    }
}
