using UnityEngine;
using ByteSheep.Events;

/// <summary>
/// 2D SizeTween for UI Images
/// </summary>
public class SizeTween : TweenBase
{
    /// <summary>Use deltaTime or unscaledDeltaTime?</summary>
    public bool unscaledTime = false;
    /// <summary>Start size (width/height)</summary>
    public Vector2 from;
    /// <summary>End size (width/height)</summary>
    public Vector2 to;
    /// <summary>Event to play post-backward</summary>
    public AdvancedEvent backwardEvent;
    /// <summary>Event to play post-forward</summary>
    public AdvancedEvent forwardEvent;

    /// <summary>UI RectTransform</summary>
    private RectTransform rectTransform;        

    private void Start()
    {
        //Get RT onStart
        rectTransform = GetComponent<RectTransform>();        
    }

    private void Update()
    {
        //Only update thread if playing
        if (!isPlaying){return;}

        //Handle playback direction
        switch (playbackDirection)
        {
            //Fwds dir
            case PlaybackDirection.FORWARD:
                //Handle value update apprporiate. If unscaled time, used unscaledDT; else DT
                if (unscaledTime){value += (Time.unscaledDeltaTime / playbackTime);}
                else{value += (Time.deltaTime / playbackTime);}
                
                if (value < 1f)
                {
                    //If limit not met, then lerp sizeDelta between from/to by value;
                    rectTransform.sizeDelta = Vector2.Lerp(from, to, curve.Evaluate(value));
                }
                else
                {
                    //If limit met, reset playing flag; then lerp sizeDelta between from/to by value, then run fowardsEvent
                    isPlaying = false;
                    rectTransform.sizeDelta = Vector2.Lerp(from, to, curve.Evaluate(1f));
                    forwardEvent.Invoke();
                }
                break;

            //Backwards dir
            case PlaybackDirection.BACKWARD:
                if (unscaledTime){value += (Time.unscaledDeltaTime / playbackTime);}
                else{value += (Time.deltaTime / playbackTime);}

                if (value < 1f)
                {
                    //If limit not met, then lerp sizeDelta between to/from by value;
                    rectTransform.sizeDelta = Vector2.Lerp(to, from, curve.Evaluate(value));
                }
                else
                {
                    //If limit met, reset playing flag; then lerp sizeDelta between to/from by value, then run backwardEvent
                    isPlaying = false;
                    rectTransform.sizeDelta = Vector2.Lerp(to, from, curve.Evaluate(1f));
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
            {backwardEvent.Invoke();}
            else{forwardEvent.Invoke();}
        }
        base.StopTween(reset);
    }

    /// <summary>Callback onReset of tween</summary>
    public override void OnReset()
    {        
        //Go back to full size
        rectTransform.sizeDelta = Vector2.one * 100f;
    }
}