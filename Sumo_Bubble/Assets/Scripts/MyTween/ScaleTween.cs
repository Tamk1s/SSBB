using UnityEngine;

/// <summary>3D ScaleTween for 3D objs</summary>
public class ScaleTween : TweenBase
{
    public Vector3 from;    //From scale size
    public Vector3 to;      //To scale size

    void Update()
    {
        //Only update thread if playing
        if (!isPlaying) { return; }

        //Handle playback direction
        switch (playbackDirection)
        {
            //Fwds dir
            case PlaybackDirection.FORWARD:
                //Update value by DT
                //!@ This script needs updated by unscaled DT optional flag
                value += (Time.deltaTime / playbackTime);
                if (value < 1f)
                {
                    //If limit not met, then lerp localScale between from/to by value;
                    transform.localScale = Vector3.Lerp(from, to, curve.Evaluate(value));
                }
                else
                {
                    //If limit met, reset playing flag; then lerp localScale between from/to by value;
                    isPlaying = false;
                    transform.localScale = Vector3.Lerp(from, to, curve.Evaluate(1f));
                }
                break;

            //Backwards dir
            case PlaybackDirection.BACKWARD:
                
                //Update value by DT
                //!@ This script needs updated by unscaled DT optional flag
                value += Time.deltaTime / playbackTime;

                if (value < 1f)
                {
                    //If limit not met, then lerp localScale between to/from by value;
                    transform.localScale = Vector3.Lerp(to, from, curve.Evaluate(value));
                }
                else
                {
                    //If limit met, reset playing flag; then lerp localScale between to/from by value;
                    isPlaying = false;
                    transform.localScale = Vector3.Lerp(to, from, curve.Evaluate(1f));
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

    /// <summary>Same as PlayBackward, but object owning this script self destructs afterwards, as well as an additional object ref to destroy</summary>
    public void PlayBackward_selfdestruct(GameObject go)
    {
        float delay = base.playbackTime;
        float shortdelay = delay - .01f;
        base.PlayBackward();                //Playbackwards, then 
        Destroy(go, shortdelay);            //Destroy GO by shorter delay
        Destroy(this.gameObject, delay);    //Destroy this, by longer delay
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
        //Reset local scale to V3.1 (ID)
        transform.localScale = Vector3.one;
    }
}