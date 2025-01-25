using UnityEngine;
using UnityEngine.Events;

/// <summary>Selects a random event from array, and invokes it</summary>
public class RandomEventSelection : MonoBehaviour
{
    /// <summary>Array of random events to invoke</summary>
    public UnityEvent[] randomEventsSet;

    /// <summary>Invokes a random event. Call this</summary>
    public void InvokeRandom()
    {
        //if array has a size, then invoke random event index
        if (randomEventsSet.Length > 0)
        {
            //GameManager.instance.RandomSeed();
            randomEventsSet[Random.Range(0, randomEventsSet.Length)].Invoke();
        }
    }
}
