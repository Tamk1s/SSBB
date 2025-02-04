using UnityEngine;
using ByteSheep.Events;

/// <summary>CanvasGroup group with Alpha component tweening (transparency)</summary>
public class CanvasGroupAlpha : TweenBase
{
    /// <summary>Use unscaledTime instead of scaled?</summary>
    public bool unscaledTime = false;
    /// <summary>From alpha value</summary>
    public float from;
    /// <summary>To alpha value</summary>
    public float to;
    /// <summary>Event to play post-backward</summary>
    public AdvancedEvent backwardEvent;
    /// <summary>Event to play post-forward</summary>
    public AdvancedEvent forwardEvent;
    /// <summary>Canvas group component. Must be attached to gameobject on this script!</summary>
    private CanvasGroup canvasGroup;                

    private void Start()
    {
        //Get CG component from this gameobject
        canvasGroup = GetComponent<CanvasGroup>();
    }

    /// <summary>Handles updating canvas group alphaTweening</summary>
    private void Update()
    {
        //Only update thread if playing
        if (!isPlaying){return;}

        //Handle playback direction
        switch (playbackDirection)
        {
            //Fwds dir
            case PlaybackDirection.FORWARD:
                //Handle value update apprporiate. If unscaled time, use unscaledDT; else DT
                if (unscaledTime == false)
                {value += (Time.deltaTime / playbackTime);}
                else{value += (Time.unscaledDeltaTime / playbackTime);}

                if (value < 1f)
                {
                    //If limit not met, then lerp canvasGroup alpha between from/to by value;
                    canvasGroup.alpha = Mathf.Lerp(from, to, curve.Evaluate(value));
                }
                else
                {
                    //If limit met, reset playing flag. Then lerp canvasGroup alpha between from/to by value;
                    isPlaying = false;
                    canvasGroup.alpha = Mathf.Lerp(from, to, curve.Evaluate(1f));
                    forwardEvent.Invoke();
                }
                break;

            //Bck dir
            case PlaybackDirection.BACKWARD:
                //Handle value update apprporiate. If unscaled time, use unscaledDT; else DT
                if (unscaledTime == false)
                {value += (Time.deltaTime / playbackTime);}
                else{value += (Time.unscaledDeltaTime / playbackTime);}

                if (value < 1f)
                {
                    //If limit not met, then lerp canvasGroup alpha between to/from by value;
                    canvasGroup.alpha = Mathf.Lerp(to, from, curve.Evaluate(value));
                }
                else
                {
                    //If limit met, reset playing flag. Then lerp canvasGroup alpha between to/from by value;
                    isPlaying = false;
                    canvasGroup.alpha = Mathf.Lerp(to, from, curve.Evaluate(1f));
                    backwardEvent.Invoke();
                }
                break;
        }
    }

    /// <summary>Plays tween fwd</summary>
    public override void PlayForward()
    {
        base.PlayForward();
    }

    /// <summary>Plays tween bck</summary>
    public override void PlayBackward()
    {
        base.PlayBackward();
    }

    /// <summary>Stops tween</summary>
    /// <param name="reset">Reset timer?</param>
    public override void StopTween(bool reset)
    {
        if (isPlaying)
        {
            //If was playing, determine direction and then fire appropriate events
            if (playbackDirection == PlaybackDirection.BACKWARD)
            { backwardEvent.Invoke(); }
            else { forwardEvent.Invoke(); }
        }
        base.StopTween(reset);
    }

    /// <summary>Callback onReset of tween</summary>
    public override void OnReset()
    {
        //If canvasGropu found, then set alpha to 1f
        if (canvasGroup){canvasGroup.alpha = 1f;}
    }
}