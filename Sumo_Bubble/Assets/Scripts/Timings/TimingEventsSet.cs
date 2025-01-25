using ByteSheep.Events;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System;

/// <summary>
/// An array/set of timingEvents in Inspector
/// Used for handling a set of events in a list with yield times
/// </summary>
public class TimingEventsSet : MonoBehaviour
{
    public bool onStart = false;        //Star time onStart?
    public TimingEvent[] eventsSet;     //Set of timingEvents
    private int currentIndex = 0;       //Current index

    void Start()
    {
        //If onstart flag set, startTimer
        if (onStart) { StartTimer(); }
    }

    /// <summary>Starts the timer for this set</summary>
    public void StartTimer()
    {
        StopAllCoroutines();    //Stop all ocrouts
        currentIndex = 0;       //Reset index to 0

        //if eventSet has a size, and this object is active, then start this event[index]
        if ((eventsSet.Length > 0) && (gameObject.activeInHierarchy))
        {
            StartCoroutine(wait(eventsSet[currentIndex].time));
        }
    }

    /// <summary>
    /// Destroys this gameobject.
    /// Call/use this as an event for self-destruction in eventSet
    /// </summary>
    public void Destroy()
    {
        Destroy(this.gameObject);
    }

    /// <summary>Wait yield coroutine before running events set</summary>
    /// <param name="time">Time to wait/yield</param>
    /// <returns>Delayed corout</returns>
    private IEnumerator wait(float time)
    {
        yield return new WaitForSeconds(time);          //Yield by timer
        eventsSet[currentIndex].onTimerEnd.Invoke();    //Invoke current event with DaString

        //if current index is before size limit, and this gameobject is active, then increment index, then start next event
        if ((currentIndex < eventsSet.Length - 1) && (gameObject.activeSelf))
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

    //!@

    #region LoadScene_Shim
    /// <summary>AlphaTween fade in/out the transition sprite (LevelScreen)</summary>
    /// <param name="time">Playback Time/direction (signed)</param>
    public void FadeTransition(float time)
    {
        //If LevelScreen singleton exists, then run FadeTransition(time)
        bool exists = false;
        LevelScreen LS = GetLSc(ref exists);
        if (exists){LS.FadeTransition(time);}
    }

    /// <summary>Loads stored scene in LoadScene singleton, with delay</summary>
    /// <param name="delay">Time delay</param>
    public void LoadSceneDelayed(float delay)
    {
        //If LoadScene singleton exists, then LoadSceneDelayed(delay)
        bool exists = false;
        LoadScene inst = GetLoadScene(ref exists);
        if (exists) { inst.LoadSceneDelayed(delay); }
    }

    /// <summary>Loads MainMenu scene LoadScene singleton, with delay</summary>
    /// <param name="delay">Time delay</param>
    public void LoadMainMenuDelayed(float delay)
    {
        //If LoadScene singleton exists, then LoadMainMenuDelayed(delay)
        bool exists = false;
        LoadScene inst = GetLoadScene(ref exists);
        if (exists) { inst.LoadMainMenuDelayed(delay); }
    }

    /// <summary>Reloads current scene from LoadScene singleton, wtih delay</summary>
    /// <param name="delay">Time delay</param>
    public void ReloadScene(float delay)
    {
        //If LoadScene singleton exists, then reloadScene(delay)
        bool exists = false;
        LoadScene inst = GetLoadScene(ref exists);
        if (exists) { inst.ReloadScene(delay); }
    }
    /// <summary>Sets the stored scene name for LoadScene singleton</summary>
    /// <param name="scene">New scene string name</param>
    public void SetScene(string scene)
    {
        //If LoadScene singleton exists, then SetScene(scene)
        bool exists = false;
        LoadScene inst = GetLoadScene(ref exists);
        if (exists) { inst.SetScene(scene); }
    }

    /// <summary>Retrieves the LoadScene singleton ref (if exists), and if that reference exists (ByRef)</summary>
    /// <param name="exists">Does LoadScene singleton exist (ByRef)</param>
    /// <returns>LoadScene singleton (if exists)</returns>
    private LoadScene GetLoadScene(ref bool exists)
    {
        LoadScene inst = LoadScene.instance;    //Get instance
        exists = (inst != null);                //Get existance
        return inst;
    }

    /// <summary>Get the LevelSceen singleton ref (if exists)</summary>
    /// <param name="exists">LevelScreen exists?</param>
    /// <returns>LevelScreen instance</returns>
    private LevelScreen GetLSc(ref bool exists)
    {
        LevelScreen LSc = LevelScreen.instance;
        exists = (LSc != null);
        return LSc;
    }
    #endregion
}

[Serializable]
/// <summary>TimingEvent event</summary>
public class TimingEvent
{
    /// <summary>Time for event</summary>
    public float time;
    /// <summary>Event to fire onTimerEnd</summary>
    public AdvancedEvent onTimerEnd;    
}
