using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleAir : MonoBehaviour
{
    [Header("Constants")]
    public float MaxAir = 100f;
    public float MinAir = 0f;
    public float StartingAir = 25f;
    public float pumpUpIncrement;    
    public float airLossPerSecond = 10f;
    public float airBoostLossPerSecond = 2.5f;
    public float hurtLossPerSecond = 25f;
    public float spikeLossPerSecond = 25f;

    [Header("Debug")]
    public float currentAir;

    //Components
    public Audio audio;
    public Audio boostClip;
    public BallController ballC;
    public BallPhysics physics;

    private bool boosting = false;    
    private bool ready = false;

    public void Start()
    {
        currentAir = StartingAir;
        ready = true;
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
    public bool PumpUp()
    {       
        float val = (currentAir + pumpUpIncrement);
        bool delta = false;
        currentAir = ChangeAir(val, ref delta);
        if (delta) { audio.sfx_play(Audio.SFX.SFX_PUMP); }
        return delta;
    }

    [Button]
    public void DoBlow()
    {
        float val = (currentAir - airBoostLossPerSecond/4f);
        bool delta = false;
        currentAir = ChangeAir(val,ref delta);
        if (delta){ToggleBoostSFX(true);}
    }

    public void DoBlow_Hurt(float hurt, int hurtType, Audio.SFX clip)
    {
        float val = (currentAir - hurt);
        bool delta = false;
        currentAir = ChangeAir(val, ref delta);
        if (delta){audio.sfx_play(clip);}

        bool good = (hurtType != 0);
        if (good)
        {
            ballC.animator.SetHurt(hurtType);
            Debug.Log("Hurtanim" + hurtType.ToString());
        }
    }

    public void StopBlow()
    {
        ToggleBoostSFX(false);
    }

    public void Deflate()
    {
        float def = (airLossPerSecond);
        def *= UnityEngine.Time.deltaTime;
        float val = (currentAir - def);
        bool delta = false;
        currentAir = ChangeAir(val, ref delta);
    }

    private float ChangeAir(float val, ref bool change)
    {
        bool dead = ((val < MinAir) || (val > MaxAir));
        if (dead){ballC.ToggleDeath(true);}

        float newVal = Mathf.Clamp(val, MinAir, MaxAir);
        change = (val != newVal);
        float size = (currentAir / MaxAir);
        physics.changeSize(size);
        return newVal;
    }

    private void ToggleBoostSFX(bool state)
    {
        boostClip.gameObject.SetActive(state);
        boosting = state;
        /*
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
        */
    }
}
