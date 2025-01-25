using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleAir : MonoBehaviour
{
    [Header("Constants")]
    public float MaxAir = 100;
    public float MinAir = 0f;
    public float StartingAir = 25;
    public float pumpUpIncrement;
    public float airLossPerSecond = 10;

    [Header("Debug")]
    public float currentAir;

    //Components
    public Audio audio;
    public Audio boostClip;
    public BallPhysics physics;

    private bool boosting = false;    
    private bool ready = false;

    public void Start()
    {
        currentAir = StartingAir;
    }

    public void Update()
    {
        if (ready)
        {
            Deflate();
        }
    }

    public void ToggleReady(bool state)
    {
        ready = state;
    }

    [Button]
    public void PumpUp()
    {
        float val = (currentAir + pumpUpIncrement);
        bool delta = false;
        currentAir = ChangeAir(val, ref delta);
        if (delta) { audio.sfx_play(Audio.SFX.SFX_PUMP); }
    }

    [Button]
    public void DoBlow()
    {
        float val = (currentAir - airLossPerSecond);
        bool delta = false;
        currentAir = ChangeAir(val,ref delta);
        if (delta)
        {
            ToggleBoostSFX(true);
        }
    }

    public void StopBlow()
    {
        ToggleBoostSFX(false);
    }

    public void Deflate()
    {
        float def = (airLossPerSecond / 2f);        
        float val = (currentAir - (UnityEngine.Time.deltaTime * def));
        bool delta = false;
        currentAir = ChangeAir(val, ref delta);        
    }

    private float ChangeAir(float val, ref bool change)
    {
        float newVal = Mathf.Clamp(val, MinAir, MaxAir);
        change = (val != newVal);
        if (change)
        {
            float size = (currentAir / MaxAir);
            physics.changeSize(size);
        }
        return newVal;
    }

    private void ToggleBoostSFX(bool state)
    {
        boostClip.gameObject.SetActive(state);
        if (state)
        {
            if (!boosting)
            {
                boosting = true;
                //boostClip.sfx_play(Audio.SFX.SFX_BOOST);
            }
        }
        else
        {            
            //boostClip.sfx_stop();
            boosting = false;
        }
    }
}
