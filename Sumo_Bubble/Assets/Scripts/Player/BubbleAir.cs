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
    public Audio moveClip;
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
        ToggleMoveSFX(ready);
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
        float val = currentAir - (airBoostLossPerSecond * UnityEngine.Time.deltaTime);
        bool delta = false;
        currentAir = ChangeAir(val, ref delta);
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
        if (dead) { ballC.ToggleDeath(true,true); }

        float newVal = Mathf.Clamp(val, MinAir, MaxAir);
        change = (val != newVal);
        float size = (currentAir / MaxAir);
        physics.changeSize(size);
        return newVal;
    }

    public Color32 ColorLerp(float pcent)
    {
        const byte hexMax = 0xFF;
        const byte hexMid = 0x7F;
        const byte hexLow = 0x00;
        const byte maxPts = 0x05;
        float[] pts = new float[maxPts]
        {
            0f,
            25f,
            50f,
            75f,
            100f,
        };

        Color32 clrRedMax = new Color32(hexMax, hexLow, hexLow, hexMax);
        Color32 clrRedMid = new Color32(hexMid, hexLow, hexLow, hexMax);
        Color clrGreen = new Color32(hexLow, hexMax, hexLow, hexMax);

        float t = 0f;
        float d = 0f;
        Color result = Color.white;

        if (pcent < pts[0x01])
        {
            pcent = Mathf.Clamp(pcent, 0f, pts[0x01]);
            t = pcent / pts[0x01];
            result = Color32.Lerp(clrRedMax, clrRedMid, t);
        }
        else if ((pcent >= pts[0x01]) && (pcent <= pts[0x02]))
        {
            t = (pcent - pts[0x01]);
            d = (pts[0x02] - pts[0x01]);
            t = (t / d);
            result = Color32.Lerp(clrRedMid, clrGreen, t);
        }
        else if ((pcent >= pts[0x02]) && (pcent <= pts[0x03]))
        {
            t = (pcent - pts[0x02]);
            d = (pts[0x03] - pts[0x02]);
            t = (t / d);
            result = Color32.Lerp(clrGreen, clrRedMid, t);
        }
        else if (pcent > pts[0x03])
        {
            pcent = Mathf.Clamp(pcent, pts[0x03], pts[0x04]);

            t = (pcent - pts[0x03]);
            d = (pts[0x04] - pts[0x03]);
            t = (t / d);
            result = Color32.Lerp(clrRedMid, clrRedMax, t);
        }
        return result;
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

    private void ToggleMoveSFX(bool state)
    {
        moveClip.gameObject.SetActive(state);
    }
    #endregion
}