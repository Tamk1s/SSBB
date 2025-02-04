using UnityEngine;

public class RotationTween : TweenBase
{
    public bool global = false;                     //!@ Use global coords for dynamic tween (Tto/Tfrom)? If so, you must use 3D coords and transform vs. Rectransform for now.
    public Vector3 from;                            //Start Quaternion.ToEuler(V3)
    public Vector3 to;                              //End Quaternion.ToEuler(V3)

    //!@ Dynamic to/from positions
    public Transform Tfrom;                         //From transform
    public Transform Tto;                           //To transform

    private RectTransform rectTransform;

    /// <summary>Start thread. Get rectTransform</summary>
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>Update RotationTween</summary>
    private void Update()
    {
        //Only update thread if playing
        if (!isPlaying){return;}

        Vector3 A;  //from euler vector for dynamic
        Vector3 B;  //to euler vector for dynamic

        //Handle playback direction
        switch (playbackDirection)
        {
            //Fwds dir
            case PlaybackDirection.FORWARD:
                //Update value by unscaledDT
                value += (Time.unscaledDeltaTime / playbackTime);

                if (!Tfrom && !Tto)
                {
                    //if not using dynamic tween, set A & B as from/to respectively
                    A = from;
                    B = to;
                }
                else
                {
                    //if using dynamic
                    //set A & B as Tfrom/Tto respectively, and use either local/global pos based on global flag
                    if (global == false)
                    {
                        A = Tfrom.transform.localEulerAngles;
                        B = Tto.transform.localEulerAngles;
                    }
                    else
                    {
                        A = Tfrom.transform.eulerAngles;
                        B = Tto.transform.eulerAngles;
                    }
                }

                if (value < 1f)
                {
                    //If limit not met, then lerp sizeDelta between from/to by value;
                    if (rectTransform)
                    {
                        //If rectTransform exists, then adjust localEulerAngles
                        rectTransform.localEulerAngles = Vector3.Lerp(A, B, curve.Evaluate(value));
                    }
                    else
                    {
                        //Use appropriate local/global rot for any 3D stuff
                        if (global == false)
                        {transform.localEulerAngles = Vector3.Lerp(A, B, curve.Evaluate(value));}
                        else{transform.eulerAngles = Vector3.Lerp(A, B, curve.Evaluate(value));}
                    }
                }
                else
                {
                    //If limit met, reset playing flag; then lerp rotations
                    isPlaying = false;

                    if (rectTransform)
                    {
                        //If rectTransform exists, then adjust localEulerAngles
                        rectTransform.localEulerAngles = Vector3.Lerp(A, B, curve.Evaluate(1f));
                    }
                    else
                    {
                        //Use appropriate local/global rot for any 3D stuff
                        if (global == false)
                        {transform.localEulerAngles = Vector3.Lerp(A, B, curve.Evaluate(1f));}
                        else{transform.eulerAngles = Vector3.Lerp(A, B, curve.Evaluate(1f));}
                    }
                }
                break;

            //Bck dir
            case PlaybackDirection.BACKWARD:
                //Update value by unscaledDT
                value += (Time.unscaledDeltaTime / playbackTime);

                if (!Tfrom && !Tto)
                {
                    //if not using dynamic, set A & B as from/to respectively
                    A = from;
                    B = to;
                }
                else
                {
                    //if using dynamic
                    //set A & B as Tfrom/Tto respectively, and use either local/global pos based on global flag
                    if (global == false)
                    {
                        A = Tfrom.transform.localEulerAngles;
                        B = Tto.transform.localEulerAngles;
                    }
                    else
                    {
                        A = Tfrom.transform.eulerAngles;
                        B = Tto.transform.eulerAngles;
                    }
                }

                if (value < 1f)
                {
                    //If limit not met, then lerp sizeDelta between from/to by value;
                    if (rectTransform)
                    {
                        //If rectTransform exists, then adjust localEulerAngles
                        rectTransform.localEulerAngles = Vector3.Lerp(B, A, curve.Evaluate(value));
                    }
                    else
                    {
                        //Use appropriate local/global rot for any 3D stuff
                        if (global == false)
                        {transform.localEulerAngles = Vector3.Lerp(B, A, curve.Evaluate(value));}
                        else{transform.eulerAngles = Vector3.Lerp(B, A, curve.Evaluate(value));}
                    }
                }
                else
                {
                    //If limit met, reset playing flag; then lerp rotations
                    isPlaying = false;
                    if (rectTransform)
                    {
                        //If rectTransform exists, then adjust localEulerAngles
                        rectTransform.localEulerAngles = Vector3.Lerp(B, A, curve.Evaluate(1f));
                    }
                    else
                    {
                        //Use appropriate local/global rot for any 3D stuff
                        if (global == false)
                        {transform.localEulerAngles = Vector3.Lerp(B, A, curve.Evaluate(1f));}
                        else{transform.eulerAngles = Vector3.Lerp(B, A, curve.Evaluate(1f));}
                    }
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
        //Reset angles on appropriate component; if rectTrans exists, then that; else regular transform
        if (rectTransform){rectTransform.eulerAngles = Vector3.zero;}
        else{transform.eulerAngles = Vector3.zero;}
    }
}