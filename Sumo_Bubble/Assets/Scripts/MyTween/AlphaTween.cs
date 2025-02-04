using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>Tweens alpha values for transparency of UI stuff (SpriteRender,MaskableGraphic,CanvasGroup</summary>
public class AlphaTween : TweenBase
{
    public float from;                              //From alpha value
    public float to;                                //To alpha value

    private SpriteRenderer SR;                      //SpriteRenderer    component
    private MaskableGraphic image;                  //Image ~
    private CanvasGroup group;                      //CanvasGroup~

    private void Start()
    {
        //Get components if exist
        group = GetComponent<CanvasGroup>();
        image = GetComponent<MaskableGraphic>();
        SR = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Handles alpha tweens for UI components
    /// </summary>
    private void Update()
    {
        //Only update thread if playing
        if (!isPlaying){return;}

        //Handle playback direction
        switch (playbackDirection)
        {
            //Fwds dir
            case PlaybackDirection.FORWARD:
                //Update value by unscaledDT
                value += (Time.unscaledDeltaTime / playbackTime);

                if (value < 1f)
                {
                    //If limit not met, then lerp alpha between from/to by value for appropriate existing component

                    //If CanvasGroup exists, then lerp it's alpha; elseif Image component, use same color but new lerped alpha; elseif SpriteRenderer exists, then lerp its alpha color
                    if (group){group.alpha = Mathf.Lerp(from, to, curve.Evaluate(value));}
                    else if (image){image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(from, to, curve.Evaluate(value)));}
                    else if (SR){SR.color = new Color(SR.color.r, SR.color.g, SR.color.b, Mathf.Lerp(from, to, curve.Evaluate(value)));}
                }
                else
                {
                    //If limit met, then reset playing flag. Then lerp alpha between from/to by value for appropriate existing component
                    isPlaying = false;

                    //If CanvasGroup exists, then lerp it's alpha; elseif Image component, use same color but new lerped alpha; elseif SpriteRenderer exists, then lerp its alpha color
                    if (group){group.alpha = Mathf.Lerp(from, to, curve.Evaluate(1f));}
                    else if (image){image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(from, to, curve.Evaluate(1f)));}
                    else if (SR){SR.color = new Color(SR.color.r, SR.color.g, SR.color.b, Mathf.Lerp(from, to, curve.Evaluate(1f)));}
                }
                break;

            //Bck dir
            case PlaybackDirection.BACKWARD:
                //Update value by unscaledDT
                value += (Time.unscaledDeltaTime / playbackTime);

                if (value < 1f)
                {
                    //If limit not met, then lerp alpha between to/from by value for appropriate existing component

                    //If CanvasGroup exists, then lerp it's alpha; elseif Image component, use same color but new lerped alpha; elseif SpriteRenderer exists, then lerp its alpha color
                    if (group){group.alpha = Mathf.Lerp(to, from, curve.Evaluate(value));}
                    else if(image){image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(to, from, curve.Evaluate(value)));}
                    else if(SR){SR.color = new Color(SR.color.r,SR.color.g,SR.color.b, Mathf.Lerp(to, from, curve.Evaluate(value)));}
                }
                else
                {
                    //If limit met, then reset playing flag. Then lerp alpha between from/to by value for appropriate existing component
                    isPlaying = false;

                    //If CanvasGroup exists, then lerp it's alpha; elseif Image component, use same color but new lerped alpha; elseif SpriteRenderer exists, then lerp its alpha color
                    if (group){group.alpha = Mathf.Lerp(to, from, curve.Evaluate(1f));}
                    else if(image){image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(to, from, curve.Evaluate(1f)));}
                    else if(SR){SR.color = new Color(SR.color.r,SR.color.g,SR.color.b, Mathf.Lerp(to, from, curve.Evaluate(1f)));}
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
        //Reset appropriate component's alpha/color tints

        //If CanvasGroup, then fully opaque; elseif Image, then white; elseif Sprite, then white
        if (group) { group.alpha = 1f; }
        else if (image) { image.color = Color.white; }
        else if (SR) { SR.color = Color.white; }
    }
}