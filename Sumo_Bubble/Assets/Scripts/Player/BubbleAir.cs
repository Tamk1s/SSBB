using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleAir : MonoBehaviour
{
    #region Setup
    public enum hurtType
    {
        HT_NONE,
        HT_BALL,
        HT_OBSTACLE,
        MAX = HT_OBSTACLE
    };
    public const byte MAX_HURTTYPE = (byte)(hurtType.MAX) + 0x01;
    #endregion

    #region Vars
    #region Constants
    [Header("Constants")]
    public float MaxAir = 100f;
    public float MinAir = 0f;
    public float StartingAir = 25f;
    public float pumpUpIncrement;    
    public float airLossPerSecond = 10f;
    public float airBoostLossPerSecond = 2.5f;
    public float hurtLossPerSecond = 25f;
    public float spikeLossPerSecond = 25f;
    #endregion

    #region Debug
    [Header("Debug")]
    public float currentAir;
    #endregion region

    #region Components
    //Components
    [Header("Components")]
    public Audio audio;
    public Audio boostClip;
    public BallController ballC;
    public BallPhysics physics;
    #endregion

    #region States
    private bool boosting = false;    
    private bool ready = false;
    #endregion
    #endregion

    #region StdUnityEvents
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
        else
        {
            ToggleBoostSFX(false);
        }
    }

    public void ToggleReady(bool state)
    {
        ready = state;
    }
    #endregion

    #region AirFuncs
    [Button]
    public bool PumpUp()
    {       
        float val = (currentAir + pumpUpIncrement);
        bool delta = false;
        currentAir = ChangeAir(val, ref delta);
        //if (delta) {
        audio.sfx_play(Audio.SFX.SFX_PUMP);
        return delta;
    }

    [Button]
    public void DoBlow()
    {
        float val = (currentAir - airBoostLossPerSecond/4f);
        bool delta = false;
        currentAir = ChangeAir(val,ref delta);
        //if (delta){
            ToggleBoostSFX(true);
        //}
    }

    public void DoBlow_Hurt(float hurt, hurtType hType, Audio.SFX clip)
    {
        bool good = (hType != hurtType.HT_NONE);
        if (good) { DoHurt(hType); }

        float val = (currentAir - hurt);
        bool delta = false;
        currentAir = ChangeAir(val, ref delta);
        //if (delta){
        audio.sfx_play(clip);
        //}
    }

    public void DoHurt(hurtType hType)
    {
        ballC.animator.SetHit(hType);
        ballC.animator.SetHurt();
        Debug.Log("Hurtanim" + hType.ToString());
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
        bool dead = (((val < MinAir) || (val > MaxAir)) && (!ballC.isDead));
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
    #endregion
}
