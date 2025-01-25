using ByteSheep.Events;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System;

/// <summary>Same as TimingEventsSet, but handles events requiring string input</summary>
public class TimingEventsSet_str: MonoBehaviour
{
    public bool onStart = false;            //StartTime onStart thread?
    public string DaString = "";            //String param to use for current event. Set this to something for onStart event
    public TimingEvent_str[] eventsSet;     //Array of TimingEvent_str to run
    private int currentIndex = 0;           //Array index currently processed

    void Start()
    {
        //If onStart event, fire it with DaString
        if (onStart){StartTimer(DaString);}
    }

    /// <summary>Starts the TES string events</summary>
    /// <param name="str">String event param</param>
    public void StartTimer(string str)
    {
        //Stop all corouts, then reset currentIndex to 0
        StopAllCoroutines();
        currentIndex = 0;

        //if eventsSet array has a size, and this gameObject is active, then startCorout for event[index] with wait yield by its time amount
        if ((eventsSet.Length > 0) && (gameObject.activeInHierarchy))
        {
            StartCoroutine(wait(eventsSet[currentIndex].time));
        }
        DaString = str; //Set DaString to param
    }

    /// <summary>Destroys this gameObject. Call this for self-destruction if necessary as an event in list</summary>
    public void Destroy()
    {
        Destroy(this.gameObject);
    }

    /// <summary>Wait yield coroutine before running events set</summary>
    /// <param name="time">Time to wait/yield</param>
    /// <returns>Delayed corout</returns>
    private IEnumerator wait(float time)
    {
        yield return new WaitForSeconds(time);                  //Yield by timer
        eventsSet[currentIndex].onTimerEnd.Invoke(DaString);    //Invoke current event with DaString

        //if current index is before size limit, and this gameobject is active, then increment index, then start next event
		if((currentIndex < (eventsSet.Length - 1)) && (gameObject.activeSelf))
        {
            currentIndex++;
            StartCoroutine(wait(eventsSet[currentIndex].time));
        }
    }

    /// <summary>
    /// Toggles the game cursor
    /// Call this in events-set as an event
    /// </summary>
    /// <param name="state">Cursor enable state</param>
    public void ToggleCursor(bool state)
    {
        GameManager.instance.ToggleCursor(state);
    }
}

[Serializable]
/// <summary>TimingEvent string event</summary>
public class TimingEvent_str
{
    /// <summary>Time for event</summary>
    public float time;
    /// <summary>String event to fire onTimerEnd</summary>
    public AdvancedStringEvent onTimerEnd;
}
