using System.Collections;
using UnityEngine;

/// <summary>Set of RandomTimingEvents, for array execution</summary>
public class RandomTimingEventsSet : MonoBehaviour
{
    public TimingEvent[] eventsSet; //Array of TimingEvents

    [HideInInspector]public float from = 1f;            //Min value in range
    [HideInInspector]public float to = 2f;              //Max value in range
    [HideInInspector]public float offsetFrom = 1f;      //Min offset in range
    [HideInInspector]public float offsetTo = 2f;        //Max offset in range

    public bool onStart = false;                        //onStart, run events?
    public bool repeat = false;                         //Repeat events    
    private int currentIndex = 0;                       //Current index being run

    private void Start()
    {
        //if onStart flag, then CallWithDefaultOffset()
        if (onStart){CallWithDefaultOffset();}
    }

    /// <summary>Callevents using default offsets</summary>
    public void CallWithDefaultOffset()
    {
        CallEvents(Random.Range(offsetFrom, offsetTo));
    }

    /// <summary>Invoke ExecuteEvents() using offset</summary>
    /// <param name="offset">Offset</param>
    public void CallEvents(float offset)
    {
        Invoke("ExecuteEvents", offset);
    }

    /// <summary>
    /// Executve events in set/random range
    /// </summary>
    public void ExecuteEvents()
    {
        //Reset currentIndex
        currentIndex = 0;

        //If eventSet has a size, and this gameobject is active, then invoke this current index event with yield wait
        if ((eventsSet.Length > 0) && (gameObject.activeInHierarchy))
        {StartCoroutine(wait(eventsSet[currentIndex].time));}

        //If repeat flag set, then run another random event (repeat)
        if (repeat){CallEvents(Random.Range(from, to));}
    }

    /// <summary>Stops repeating</summary>
    public void StopRepeating()
    {
        //Stop all corouts, and cancelInvoke()
        StopAllCoroutines();
        CancelInvoke();
    }

    private IEnumerator wait(float time)
    {
        //Yield wait by time, invoke onTimerEnd event for current event
        yield return new WaitForSeconds(time);
        eventsSet[currentIndex].onTimerEnd.Invoke();

        //If currentIndex is below max AND if this gameobject is active, then increment index and invoke next event
        if ((currentIndex < (eventsSet.Length - 1)) && (gameObject.activeInHierarchy))
        {
            currentIndex++;
            StartCoroutine(wait(eventsSet[currentIndex].time));
        }
    }
}
