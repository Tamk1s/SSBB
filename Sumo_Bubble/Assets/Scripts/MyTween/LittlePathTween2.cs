using UnityEngine;

/// <summary>
/// Little path tween. Used for arc/parabola motions up/down, and left/right x-movement
/// This one is a bit different from the other tweens
/// </summary>
public class LittlePathTween2 : TweenBase
{
    public float xScale = 1f;   //xScalar for path
    public float yScale = 1f;   //yScalar for path
    private Vector3 origin;     //Origin for parabola displacement

    /// <summary>Handle path movement from origin by lerped T time value</summary>
    private void Update()
    {
        //Only update thread if playing
        if (!isPlaying){return;}

        //Handle playback direction
        switch (playbackDirection)
        {
            //Fwds dir
            case PlaybackDirection.FORWARD:
                //Increase by deltaTime
                value += (Time.deltaTime / playbackTime);

                //x-movement towards the right (value * xScale)
                if (value < 1f)
                {
                    //If limit not met, then lerp localPos from origin + between from/to by value offset
                    transform.localPosition = origin + new Vector3(value * xScale, curve.Evaluate(value) * yScale);
                }
                else
                {
                    //If limit met, then reset playing flag. Then lerp localPos from origin + between from/to by value offset
                    isPlaying = false;
                    transform.localPosition  = origin + new Vector3(value * xScale, curve.Evaluate(1f) * yScale);
                }
                break;

            //Bck dir
            case PlaybackDirection.BACKWARD:
                //Increase by deltaTime
                value += (Time.deltaTime / playbackTime);

                //x-movement towards the left (-value * xScale)
                if (value < 1f)
                {
                    //If limit not met, then lerp localPos from origin + between from/to by value offset
                    transform.localPosition = origin + new Vector3(-value * xScale, curve.Evaluate(value) * yScale);
                }
                else
                {
                    //If limit met, then reset playing flag. Then lerp localPos from origin + between from/to by value offset
                    isPlaying = false;
                    transform.localPosition = origin + new Vector3(-value * xScale, curve.Evaluate(1f) * yScale);
                }
                break;
        }
    }

    /// <summary>Plays tween fwd</summary>
    public override void PlayForward()
    {
        base.PlayForward();
        origin = transform.localPosition;       //Store the origin position, for arc displacements
    }

    /// <summary>Plays tween bck</summary>
    public override void PlayBackward()
    {
        base.PlayBackward();
        origin = transform.localPosition;       //Store the origin position, for arc displacements
    }
}
