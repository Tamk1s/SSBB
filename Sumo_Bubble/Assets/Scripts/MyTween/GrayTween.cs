
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>Tweens _2dxFX_GrayScale components</summary>
public class GrayTween : TweenBase
{
    public float from;                  //From grey value
    public float to;                    //To grey value
    public _2dxFX_GrayScale[] gray;     //Grey components
    private float timeValue=0f;         //Grey amount value to apply

    /// <summary>Handles updating grey component values for tween</summary>
    private void Update()
    {
        //Only update thread if playing
        if (!isPlaying){return;}

        //Handle playback direction
        switch (playbackDirection)
        {
            //Fwds dir
            case PlaybackDirection.FORWARD:
                //Increment by unscaledDT
                //!@ Update script to handle un/scaled DT
                value += (Time.unscaledDeltaTime / playbackTime);
                
                if (value < 1f)
                {
                    //If limit not met, then lerp sizeDelta between from/to by value;
                    timeValue = Mathf.Lerp(from, to, curve.Evaluate(value));
                }
                else
                {
                    //If limit met, reset playing flag; then lerp sizeDelta between from/to by value
                    isPlaying = false;
                    timeValue = Mathf.Lerp(from, to, curve.Evaluate(1f));
                }
                break;

            //Bck dir
            case PlaybackDirection.BACKWARD:
                //!@ Update script to handle un/scaled DT
                value += (Time.unscaledDeltaTime / playbackTime);

                if (value < 1f)
                {
                    //If limit not met, then lerp timeValue between to/from by value;
                    timeValue = Mathf.Lerp(to, from, curve.Evaluate(value));
                }
                else
                {
                    //If limit met, reset playing flag; then lerp timeValue between to/from by value
                    isPlaying = false;
                    timeValue = Mathf.Lerp(to, from, curve.Evaluate(1f));
                }
                break;
        }

        //Apply the new timeValue
        Apply(timeValue);
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
        base.StopTween(reset);
    }

    /// <summary>Callback onReset of tween</summary>
    public override void OnReset()
    {
        //Apply 0f to timeValue
        timeValue = Mathf.Lerp(from, to, curve.Evaluate(0f));
        Apply(timeValue);
    }

    /// <summary>Applies greyValue to all materials</summary>
    /// <param name="value">greyValue</param>
    private void Apply(float value)
    {
        //Iterate through all grey components
        foreach (_2dxFX_GrayScale g in gray)
        {
            //If exists, then apply grey amounts
            if (g != null){g._EffectAmount = value;}
        }
    }
}