using UnityEngine;

/// <summary>Base class for MyTween</summary>
public class TweenBase : MonoBehaviour
{
    public AnimationCurve curve;                                    //Anim curve
    public float playbackTime;                                      //Playback time
    public float multFact=1f;                                       //!@ Multiplier factor

    [HideInInspector]public float value;                            //Current timer value
    [HideInInspector]public PlaybackDirection playbackDirection;    //Playback dir
    [HideInInspector]public bool isPlaying = false;                 //Is tween playing?
    
    /// <summary>Plays tween fwd</summary>
    public virtual void PlayForward()
    {
        //Reset value, begin playing, set direction to fwd
        value = 0f;                                                 
        isPlaying = true;                                           
        playbackDirection = PlaybackDirection.FORWARD;
    }

    /// <summary>Plays tween bck</summary>
    public virtual void PlayBackward()
    {
        //Reset value, begin playing, set direction to bck
        value = 0f;
        isPlaying = true;
        playbackDirection = PlaybackDirection.BACKWARD;
    }

    /// <summary>Stops tween</summary>
    /// <param name="reset">Reset timer?</param>
    public virtual void StopTween(bool reset)
    {
        //Reset flag, and value if reset flag set
        isPlaying = false;
        if (reset){OnReset();}
    }

    /// <summary>Callback onReset of tween</summary>
    public virtual void OnReset()
    {

    }
}
