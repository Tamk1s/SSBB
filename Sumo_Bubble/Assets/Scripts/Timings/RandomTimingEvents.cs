using UnityEngine;
using UnityEngine.Events;

/// <summary>RandomTimingEvents base class</summary>
public class RandomTimingEvents : MonoBehaviour
{
    public UnityEvent events;   //Event
    public AudioClip[] clips;   //AudioClips for RandomClips (if any)

    [HideInInspector]public float from = 1f;        //Min value in range
    [HideInInspector]public float to = 2f;          //Max random value in range
    [HideInInspector]public float offsetFrom = 1f;  //Min offset
    [HideInInspector]public float offsetTo = 2f;    //Max offset

    public bool onStart = false;                    //Call events onStart?
    public bool repeat = false;                     //Repeat random event selection?

    void Start()
    {
        //If onStart flag set, then CallWithDefaultOffset()
        if (onStart){CallWithDefaultOffset();}
    }

    /// <summary>Call event with defaultOffset</summary>
    public void CallWithDefaultOffset()
    {
        //Random event 
        CallEvents(Random.Range(offsetFrom, offsetTo));
    }

    /// <summary>Execute events in random range + offset</summary>
    /// <param name="offset">Offset value</param>
    public void CallEvents(float offset = 0f)
    {
        Invoke("ExecuteEvents", Random.Range(from, to) + offset);
    }

    /// <summary>ExecuteEvents, and if repeat flag, callEvents()</summary>
    public void ExecuteEvents()
    {
        //Invoke events, then re-call if repeat flag
        events.Invoke();
        if (repeat){CallEvents();}
    }

    /// <summary>Stop repeating events (CancelInvoke())</summary>
    public void StopRepeating()
    {
        CancelInvoke();
    }

    //!@ Change me to get Audio() script instead
    /// <summary>Play a random clip in pool</summary>
    public void RandomClip()
    {
        //Get AudioSource from this gameobject; if exists
        AudioSource AS = this.gameObject.GetComponent<AudioSource>();
        if (AS)
        {
            //Get max clip index size
            byte max=(byte)(clips.GetLength(0));
            //Get random index from range, then clip by [index]
            //Play sfx oneShot
            byte ind = (byte)(UnityEngine.Random.Range(0, max));
            AudioClip clip=clips[ind];
            AS.PlayOneShot(clip);
        }
    }
}
