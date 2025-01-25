using UnityEngine;
using UnityEngine.UI;

/// <summary>Fill tween amount, for sliced filled images (e.g. circular/horiz/vert health bars etc)</summary>
public class fillTween : TweenBase
{
    public Image img;               //Image component to modify
    public Vector3 from;            //From fill amount. !@ Currently just handles x-component lerping
    public Vector3 to;              //To fill amount. !@ Currently just handles x-component lerping

    void Update()
    {
        Vector3 v = Vector3.zero;   //V3 register

        //Only update thread if playing
        if (!isPlaying){return;}

        //Handle playback direction
        switch (playbackDirection)
        {
            //Fwds dir
            case PlaybackDirection.FORWARD:                
                //!@ Update script to allow un/scaledDT
                //Increment value by DT
                value += (Time.deltaTime / playbackTime);                
                if (value < 1f)
                {
                    //If limit not met, then lerp fill amount between from/to values; apply fillAmount
                    v = Vector3.Lerp(from, to, curve.Evaluate(value));
                    img.fillAmount = v.x;
                }
                else
                {
                    //If limit met, then reset playing flag. Lerp fill amount between from/to values; apply fillAmount
                    isPlaying = false;
                    v = Vector3.Lerp(from, to, curve.Evaluate(1f));
                    img.fillAmount = v.x;
                }
                break;

            //Back dir
            case PlaybackDirection.BACKWARD:
                //!@ Update script to allow un/scaledDT
                //Increment value by DT
                value += Time.deltaTime / playbackTime;
                if (value < 1f)
                {
                    //If limit not met, then lerp fill amount between to/from values; apply fillAmount
                    v = Vector3.Lerp(to, from, curve.Evaluate(value));
                    img.fillAmount = v.x;
                }
                else
                {
                    //If limit met, then reset playing flag. Lerp fill amount between to/from values; apply fillAmount
                    isPlaying = false;
                    v = Vector3.Lerp(to, from, curve.Evaluate(1f));
                    img.fillAmount = v.x;
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
        base.StopTween(reset);
    }

    /// <summary>Callback onReset of tween</summary>
    public override void OnReset()
    {
        //Reset transform localscale to one
        //!@ This is incorrect function meat? Fixup
        //transform.localScale = Vector3.one;
        img.fillAmount = 0f;
    }
}