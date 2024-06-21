using System.Collections.Generic;
using UnityEngine;

public class ColorTransition : MonoBehaviour
{

    public Color colorA = Color.red;
    public Color colorB = Color.blue;



    [SerializeField] ObservableFloat my_ObservableFloat;

    [SerializeField] List<Material> my_Materials = new List<Material>();
    [SerializeField] List<MeshRenderer> my_MeshRenderers = new List<MeshRenderer>();



    public Color current_Color;



    private void Awake()
    {
        my_ObservableFloat.OnUpdate += EventReceiver_OnUpdate;

    }

    private void OnDestroy()
    {
        if(my_ObservableFloat != null)
            my_ObservableFloat.OnUpdate -= EventReceiver_OnUpdate;

    }


    void EventReceiver_OnUpdate(ObservableFloat _my_ObservableFloat)
    {
        updateColor();
    }


    void updateColor()
    {
        current_Color = Color.Lerp(colorA, colorB, my_ObservableFloat.ilerp_value);
        foreach(Material _Material in this.my_Materials)
        {
            _Material.color = current_Color;
        }

        foreach(MeshRenderer _MeshRenderer in this.my_MeshRenderers)
        {
            _MeshRenderer.material.color = current_Color;
        }

    }


}
