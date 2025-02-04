using UnityEngine;

/// <summary>Tween between 2 positions</summary>
public class PositionTween : TweenBase
{
    #region Setup
    [Header("Main params")]
    public bool global = false;             //!@ Use global coords for dynamic tween (Tto/Tfrom)? If so, you must use 3D coords and transform vs. Rectransform for now.
    public Vector3 from;                    //!@ Extended to Vector3D
    public Vector3 to;                      //!@ Extended to Vector3D

    [Header("Optional params")]
    //If CombatParticipant2 set, then modify this to make the character face the direction of travel
    //!@ FIX ME
    //public CombatParticipant2 comb = null;
    //public bool comb_XAxis_Only = false;    //When changing the character's direction, should only face x-axis directions? (West/East directions)
    //Dynamic to/from positions
    public Transform Tfrom;                 //From transform
    public Transform Tto;                   //To transform
    public RectTransform rectTransform;     //Optional RectTransform for position
    #endregion

    #region Handle
    private void Update()
    {
        //Only update thread if playing
        if (!isPlaying){return;}

        Vector3 A;  //from vector for dynamic
        Vector3 B;  //to vector for dynamic

        Vector3 oldPos; //The previous position
        Vector3 pos;    //The current position

        //Handle playback direction
        switch (playbackDirection)
        {
            //Fwds dir
            case PlaybackDirection.FORWARD:
                //Increment value. Increase by DT
                value += (Time.deltaTime / playbackTime);

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
                        A = Tfrom.transform.localPosition;
                        B = Tto.transform.localPosition;
                    }
                    else
                    {
                        A = Tfrom.transform.position;
                        B = Tto.transform.position;
                    }
                }

                //Do the lerp
                if (value < 1f)
                {
                    //If limit not met, then update positions
                    if (rectTransform)
                    {
                        //If rectTransform, then update anchoredPos
                        //Save old position
                        oldPos = rectTransform.anchoredPosition;
                        rectTransform.anchoredPosition = Vector2.Lerp(A, B, curve.Evaluate(value));
                        //Save new postion
                        pos = rectTransform.anchoredPosition;
                        //Handle character movement
                        HandleMovement(oldPos, pos, true);
                    }
                    else
                    {
                        //Regular transforms
                        //Use appropriate local/global pos
                        if (global == false)
                        {
                            oldPos = transform.localPosition;
                            transform.localPosition = Vector3.Lerp(A, B, curve.Evaluate(value));
                            pos = transform.localPosition;
                            HandleMovement(oldPos, pos, true);
                        }
                        else
                        {
                            oldPos = transform.position;
                            transform.position = Vector3.Lerp(A, B, curve.Evaluate(value));
                            pos = transform.position;
                            HandleMovement(oldPos, pos, true);
                        }
                    }
                }
                else
                {
                    //If limit met, reset playing flag
                    isPlaying = false;

                    //Do the lerp
                    if (rectTransform)
                    {
                        //If rectTransform, then update anchoredPos
                        oldPos = rectTransform.anchoredPosition;
                        rectTransform.anchoredPosition = Vector2.Lerp(A, B, curve.Evaluate(1f));
                        pos = rectTransform.anchoredPosition;
                        HandleMovement(oldPos, pos, false);
                    }
                    else
                    {
                        //Use appropriate local/global pos
                        if (global == false)
                        {
                            oldPos = transform.localPosition;
                            transform.localPosition = Vector3.Lerp(A, B, curve.Evaluate(1f));
                            pos = transform.localPosition;
                            HandleMovement(oldPos, pos, false);
                        }
                        else
                        {
                            oldPos = transform.position;
                            transform.position = Vector3.Lerp(A, B, curve.Evaluate(1f));
                            pos = transform.position;
                            HandleMovement(oldPos, pos, false);
                        }
                    }
                }
                break;

            //Bck dir
            case PlaybackDirection.BACKWARD:
                //Increment value. Increase by DT
                value += (Time.deltaTime / playbackTime);

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
                        A = Tfrom.transform.localPosition;
                        B = Tto.transform.localPosition;
                    }
                    else
                    {
                        A = Tfrom.transform.position;
                        B = Tto.transform.position;
                    }
                }

                //Do the lerp
                if (value < 1f)
                {
                    //Do the lerp
                    if (rectTransform)
                    {
                        //If limit not met, then update positions
                        oldPos = rectTransform.anchoredPosition;
                        rectTransform.anchoredPosition = Vector2.Lerp(B, A, curve.Evaluate(value));
                        pos = rectTransform.anchoredPosition;
                        HandleMovement(oldPos, pos, true);
                    }
                    else
                    {
                        //Regular transforms
                        //Use appropriate local/global pos
                        if (global == false)
                        {
                            oldPos = transform.localPosition;
                            transform.localPosition = Vector3.Lerp(B, A, curve.Evaluate(value));
                            pos = transform.localPosition;
                            HandleMovement(oldPos, pos, true);
                        }
                        else
                        {
                            oldPos = transform.position;
                            transform.position = Vector3.Lerp(B, A, curve.Evaluate(value));
                            pos = transform.position;
                            HandleMovement(oldPos, pos, true);
                        }
                    }
                }
                else
                {
                    //If limit met, reset playing flag
                    isPlaying = false;

                    //Do the lerp
                    if (rectTransform)
                    {
                        //If rectTransform, then update anchoredPos
                        oldPos = rectTransform.anchoredPosition;
                        rectTransform.anchoredPosition = Vector2.Lerp(B, A, curve.Evaluate(1f));
                        pos = rectTransform.anchoredPosition;
                        HandleMovement(oldPos, pos, false);
                    }
                    else
                    {
                        //Use appropriate local/global pos
                        if (global == false)
                        {
                            oldPos = transform.localPosition;
                            transform.localPosition = Vector3.Lerp(B, A, curve.Evaluate(1f));
                            pos = transform.localPosition;
                            HandleMovement(oldPos, pos, false);
                        }
                        else
                        {
                            oldPos = transform.position;
                            transform.position = Vector3.Lerp(B, A, curve.Evaluate(1f));
                            pos = transform.position;
                            HandleMovement(oldPos, pos, false);
                        }
                    }
                }
                break;
        }
    }
    #endregion

    #region BaseOverrides
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
        //SetWalk(false);         //Stop walking! !@ FIX ME
        base.StopTween(reset);
    }

    /// <summary>Callback onReset of tween</summary>
    public override void OnReset()
    {
        //SetWalk(false);         //Stop walking!  !@ FIX ME
        transform.localPosition = Vector3.zero;
    }
    #endregion

    #region HelperFuncs    
    /// <summary>Handle character movement</summary>
    /// <param name="A">PtA</param>
    /// <param name="B">PtB</param>
    /// <param name="move">Walk?</param>
    private void HandleMovement(Vector3 A, Vector3 B, bool move)
    {
        //!@ FIX ME
        //If the combatParticipant2 comp is set
        /*
        if (comb != null)
        {
            //Get the normalized vector btwn PtA & B
            Vector2 posA = A.ToVector2_XZ();
            Vector2 posB = B.ToVector2_XZ();
            Vector2 delta = (posB - posA);
            delta = delta.normalized;
            //Set the movement direction/type
            comb.combAnim.SetMovement_Anim(delta, comb_XAxis_Only, move);
        }
        */
    }

    //!@ FIX ME
    /*
    /// <summary>Set the walk flag</summary>
    /// <param name="move">Walking?</param>
    private void SetWalk(bool move)
    {
        //If component set, then toggle setWalk
        if (comb != null){comb.combAnim.SetWalk(move);}
    }
    */
    #endregion
}