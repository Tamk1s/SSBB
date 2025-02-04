using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Simple script which copies the color from the Sprite Renderer's material in use, and applies it to itself</summary>
public class SpriteRendColorCopy : MonoBehaviour
{
    private SpriteRenderer SR;          //SR attached to this gameObject
    public string _Color = "_Color";    //String name of color param
    public bool autoStart = true;       //AutoStart?
    private bool ready = false;         //Are we ready?

    void Start()
    {
        //Fetch the SR, set ready state to autoStart
        SR = this.gameObject.GetComponent<SpriteRenderer>();
        if (SR){ready = autoStart;}
    }

    void Update()
    {
        //Update the color if ready
        if (ready)
        {
            Color c = SR.sharedMaterial.GetColor(_Color);   //Get color from the sharedMaterial
            SR.color = c;                                   //Apply that color to the SpriteRenderer
        }
    }

    /// <summary>Sets the color name and ready state</summary>
    /// <param name="c"></param>
    public void SetColorName(string c)
    {
        _Color = c;
        ready = true;
    }
}
