using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Ball Animator for sprite</summary>
public class BallAnimator : MonoBehaviour
{
    /*
    x Idle
    x Walk
    x Run
    Inflate
    x Boost
    Hurt
    SpikeHurt
    Dead
    Victory

    float xmove
    bool boost
    trigger inflate
    int hurt
    bool death
    bool victory
    */

    #region Variables
    [Header("Components")]
    public Animator anim = null;
    public SpriteRenderer SR = null;
    public GameObject Sphere = null;

    [Header("Animation")]
    /// <summary>PositionTween</summary>
    //public PositionTween posTween;
    /// <summary>AlphaTween</summary>
    public AlphaTween alphaTween;

    /// <summary>Are we ready to update?</summary>
    private bool ready = false;
    #endregion

    #region Setup
    /// <summary>Animator param type</summary>
    public enum animParamType
    {
        APT_NONE,
        APT_FLOAT,
        APT_INT,
        APT_BOOL,
        APT_TRIGGER,
        MAX = APT_TRIGGER
    };
    /// <summary>Max type of animator params. Aligned with the animParamType enum</summary>
    public const byte max_animParamType = (byte)(animParamType.MAX) + 0x01;

    /// <summary>Animator param names</summary>
    public enum animParamName
    {
        NONE,
        Dir,
        Boost,
        Inflate,
        Inflating,
        Hit,
        Dead,
        DeadReset,
        Victory,
        VictoryReset,
        MAX = VictoryReset
    };
    /// <summary>Max amount of animator param names. Aligned with the animParamName enum</summary>
    public const byte max_ParamName = (byte)(animParamName.MAX) + 0x01;

    /// <summary>Struct holding pair of animParamName and animParamType</summary>
    public struct aParam
    {
        /// <summary>The Param name</summary>
        public animParamName name;
        /// <summary>The Param type// </summary>
        public animParamType type;

        /// <summary>aParam constructor</summary>
        /// <param name="Name">ParamName</param>
        /// <param name="Type">ParamType</param>
        public aParam(animParamName Name = animParamName.NONE, animParamType Type = animParamType.APT_NONE)
        {
            name = Name;
            type = Type;
        }

        /// <summary>Get the string name of the paramName</summary>
        /// <returns>String of enum</returns>
        public string GetName()
        {
            string result = name.ToString();
            return result;
        }
    }

    [NamedArrayAttribute(typeof(animParamName))]
    /// <summary>Array of valid parameters names/type pairs for standard animator. Aligned with the animParamName enum</summary>
    private aParam[] _params = new aParam[max_ParamName]
    {
        //Dummy entry
        new aParam(animParamName.NONE, animParamType.APT_NONE),
        
        //Normal entries
        new aParam(animParamName.Dir, animParamType.APT_FLOAT),
        new aParam(animParamName.Boost, animParamType.APT_BOOL),
        new aParam(animParamName.Inflate, animParamType.APT_TRIGGER),
        new aParam(animParamName.Inflating, animParamType.APT_BOOL),
        new aParam(animParamName.Hit, animParamType.APT_INT),
        new aParam(animParamName.Dead, animParamType.APT_BOOL),
        new aParam(animParamName.DeadReset, animParamType.APT_TRIGGER),
        new aParam(animParamName.Victory, animParamType.APT_BOOL),
        new aParam(animParamName.VictoryReset, animParamType.APT_TRIGGER),
    };
    #endregion

    #region StdUnityEvents
    public void Start()
    {
        ready = true;
    }

    public void Update()
    {
        if (ready)
        {
            bool good = (Sphere != null);
            if (good){this.gameObject.transform.position = Sphere.transform.position;}
        }
    }
    #endregion

    #region AnimEvents

    #region MovementFace
    /// <summary>Sets the idle/walking movement + facing direction</summary>
    /// <param name="vtr">Movement vector</param>
    public void SetMovement_Anim(Vector2 vtr)
    {
        bool flipX = (vtr.x > 0f);
        SetSpriteFlipX(flipX);
        float xaxis = Mathf.Abs(vtr.x);
        SetDir(xaxis);
    }


    /// <summary>AlphaTween fade in/out the animated sprite</summary>
    /// <param name="time">Playback Time/direction (signed)</param>
    public void FadeSprite(float time)
    {
        float s = Mathf.Sign(time);     //Get the sign of param
        float t = Mathf.Abs(time);      //Get absVal(time)
        bool fwd = (s >= 0f);           //If sign>=0f (0/+), then fwds; else backwarts
        alphaTween.playbackTime = t;    //Set the playbackTime

        //Play direction appropriately
        if (fwd) { alphaTween.PlayForward(); }
        else { alphaTween.PlayBackward(); }
    }

    public void SetDir(float dir)
    {
        const animParamName name = animParamName.Dir;
        SetFloat(name, dir);
    }

    public float GetDir()
    {
        const animParamName name = animParamName.Dir;
        float dir = GetFloat(name);
        return dir;
    }
    #endregion

    #region Params
    public void SetBoost(bool state)
    {
        const animParamName name = animParamName.Boost;
        SetBool(name, state);
    }

    public bool GetBoost()
    {
        const animParamName name = animParamName.Boost;
        bool state = GetBool(name);
        return state;
    }

    public void SetInflate()
    {
        const animParamName name = animParamName.Inflate;
        SetTrigger(name);
    }

    public bool GetInflate()
    {
        const animParamName name = animParamName.Inflate;
        bool state = GetBool(name);
        return state;
    }

    public void SetInflating(int s)
    {
        bool state = (s != 0);
        SetInflating(state);
    }

    public void SetInflating(bool state)
    {
        const animParamName name = animParamName.Inflating;
        SetBool(name, state);
    }

    public bool GetInflating()
    {
        const animParamName name = animParamName.Inflating;
        bool state = GetBool(name);
        return state;
    }

    public void SetHurt(int s)
    {
        const animParamName name = animParamName.Hit;
        SetInteger(name, s);
    }

    public int GetHurt()
    {
        const animParamName name = animParamName.Hit;
        int s = GetInteger(name);
        return s;
    }

    public void SetDead(bool state)
    {
        const animParamName name = animParamName.Dead;
        SetBool(name, state);
    }

    public bool GetDead()
    {
        const animParamName name = animParamName.Dead;
        bool state = GetBool(name);
        return state;
    }

    public void SetDeadReset()
    {
        const animParamName name = animParamName.DeadReset;
        SetTrigger(name);
    }

    public bool GetDeadReset()
    {
        const animParamName name = animParamName.DeadReset;
        bool state = GetBool(name);
        return state;
    }

    public void SetVictory(bool state)
    {
        const animParamName name = animParamName.Victory;
        SetBool(name, state);
    }

    public bool GetVictory()
    {
        const animParamName name = animParamName.Victory;
        bool state = GetBool(name);
        return state;
    }

    public void SetVictoryReset()
    {
        const animParamName name = animParamName.VictoryReset;
        SetTrigger(name);
    }

    public bool GetVictoryReset()
    {
        const animParamName name = animParamName.VictoryReset;
        bool state = GetBool(name);
        return state;
    }
    #endregion

    #region MotionParams
    public void SetSpriteFlipX(bool state)
    {
        SR.flipX = state;
    }

    /// <summary>Sets the current animator playback speed</summary>
    /// <param name="speed">AnimSpeed</param>
    public void SetAnimSpeed(float speed)
    {
        if (anim != null) { anim.speed = speed; }
    }

    /// <summary>Gets the current animator playback speed</summary>
    /// <returns>Speed</returns>
    public float GetAnimSpeed()
    {
        float result = 1f;
        if (anim != null) { result = anim.speed; }
        return result;
    }
    #endregion

    #endregion

    #region ParamHelperFuncs
    /// <summary>Sets a float param value</summary>
    /// <param name="param">Param</param>
    /// <param name="value">Value</param>
    public void SetFloat(animParamName param, float value)
    {
        aParam p = GetAParam(param);
        bool good = (p.type == animParamType.APT_FLOAT);
        if (good)
        {
            string name = p.GetName();
            if (anim != null) { anim.SetFloat(name, value); }
        }
    }

    /// <summary>Gets a float param value</summary>
    /// <param name="param">Param</param>
    /// <returns>Float</returns>
    public float GetFloat(animParamName param)
    {
        //Get the aParam; if its type is indeed a float, then get its name and then its float value
        float result = 0f;
        aParam p = GetAParam(param);
        bool good = (p.type == animParamType.APT_FLOAT);
        if (good)
        {
            string name = p.GetName();
            if (anim != null) { result = anim.GetFloat(name); }
        }
        return result;
    }

    /// <summary>Sets an integer param value</summary>
    /// <param name="param">Param</param>
    /// <param name="value">Value</param>
    public void SetInteger(animParamName param, int value)
    {
        //Get the aParam; if its type is indeed an int, then get its name and then set value
        aParam p = GetAParam(param);
        bool good = (p.type == animParamType.APT_INT);
        if (good)
        {
            string name = p.GetName();
            if (anim != null) { anim.SetInteger(name, value); }
        }
    }

    /// <summary>Gets an int param's value</summary>
    /// <param name="param">Param</param>
    /// <returns>Int</returns>
    public int GetInteger(animParamName param)
    {
        //Get the aParam; if its type is indeed an int, get param's name and then return its value
        int result = 0;
        aParam p = GetAParam(param);
        bool good = (p.type == animParamType.APT_INT);
        if (good)
        {
            string name = p.GetName();
            if (anim != null) { result = anim.GetInteger(name); }
        }
        return result;
    }

    /// <summary>Set a bool/trigger param's value</summary>
    /// <param name="param">Param</param>
    /// <param name="state">State</param>
    public void SetBool(animParamName param, bool state)
    {
        //Get the aParam; if its type is indeed an bool/trigger, set state
        aParam p = GetAParam(param);
        bool good = (p.type == animParamType.APT_BOOL) || (p.type == animParamType.APT_TRIGGER);
        if (good)
        {
            string name = p.GetName();
            if (anim != null) { anim.SetBool(name, state); }
        }
    }

    /// <summary>Get a bool/trigger param's state</summary>
    /// <param name="param">Param</param>
    /// <returns>State</returns>
    public bool GetBool(animParamName param)
    {
        //Get the aParam; if its type is indeed an bool/trigger, get state
        bool result = false;
        aParam p = GetAParam(param);
        bool good = ((p.type == animParamType.APT_BOOL) || (p.type == animParamType.APT_TRIGGER));
        if (good)
        {
            string name = p.GetName();
            if (anim != null) { result = anim.GetBool(name); }
        }
        return result;
    }

    /// <summary>Sets a trigger param</summary>
    /// <param name="param">Param</param>
    public void SetTrigger(animParamName param)
    {
        //Get the aParam; if its type is indeed a trigger, then set
        aParam p = GetAParam(param);
        bool good = (p.type == animParamType.APT_TRIGGER);
        if (good)
        {
            string name = p.GetName();
            if (anim != null) { anim.SetTrigger(name); }
        }
    }

    /// <summary>Resets a trigger param</summary>
    /// <param name="param">Param</param>
    public void ResetTrigger(animParamName param)
    {
        //Get the aParam; if its type is indeed a trigger, then reset
        aParam p = GetAParam(param);
        bool good = (p.type == animParamType.APT_TRIGGER);
        if (good)
        {
            string name = p.GetName();
            if (anim != null) { anim.ResetTrigger(name); }
        }
    }

    /// <summary>Given an animParamName, returns the matching aParam from _params array</summary>
    /// <param name="name">animParamName</param>
    /// <returns>AParam</returns>
    private aParam GetAParam(animParamName name)
    {
        byte index = (byte)(name);          //Convert name to index byte
        aParam result = _params[index];     //Fetch array element
        return result;                      //Return
    }
    #endregion

    #region HelperFuncs
    /// <summary>Gets the main Audio singleton</summary>
    /// <param name="audio">Audio singleton</param>
    /// <returns>Exists?</returns>
    private bool GetAudio_Singleton(ref Audio audio)
    {
        audio = Audio.instance;
        bool result = (audio != null);
        return result;
    }
    #endregion
}