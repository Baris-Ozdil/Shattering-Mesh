using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranparencyChange : MonoBehaviour
{
    private Color color;
    private float time;
    private float newAlpha;
    public void setData(float _time,float _newAlpha)
    {
        color = gameObject.GetComponent<MeshRenderer>().material.color;
        time = _time;
        newAlpha = _newAlpha;

    }
    // Update is called once per frame
    void Update()
    {
        //Here we cnhage the alpho of material and we make it more transparent
        if(color.a > newAlpha)
        {
            if(color.a - (1 - newAlpha) * Time.deltaTime/time <= 0)
            {
                color = new Color(color.r, color.g, color.b, newAlpha);
                gameObject.GetComponent<MeshRenderer>().material.color = color ;
            }
            else if(color.a != newAlpha)
            {
                color = new Color(color.r, color.g, color.b, color.a - (1- newAlpha) * Time.deltaTime / time);
                gameObject.GetComponent<MeshRenderer>().material.color = color;
            }
            
        }

        
    }
}
