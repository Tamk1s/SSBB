using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>LevelScreen. For HUD, transition fades, Level/area TitleCards, etc.</summary>
public class LevelScreen : MonoBehaviour
{
    /// <summary>Singleton instance</summary>
    public static LevelScreen instance = null;
    /// <summary>Are we ready?</summary>
    private bool ready = false;

    /// <summary>AlphaTween component</summary>
    public AlphaTween fade;

    public void Start()
    {
        if (!instance) { instance = this; }

        //If singleton instance found, then fade in
        ready = (instance != null);
        if (ready) { FadeTransition(-3f); }
    }

    /// <summary>AlphaTween fade in/out the transition sprite</summary>
    /// <param name="time">Playback Time/direction (signed)</param>
    public void FadeTransition(float time)
    {
        float s = Mathf.Sign(time);     //Get the sign of param
        float t = Mathf.Abs(time);      //Get absVal(time)
        bool fwd = (s >= 0f);           //If sign>=0f (0/+), then fwds; else backwarts
        fade.playbackTime = t;    //Set the playbackTime

        //Play direction appropriately
        if (fwd) { fade.PlayForward(); }
        else { fade.PlayBackward(); }
    }
}