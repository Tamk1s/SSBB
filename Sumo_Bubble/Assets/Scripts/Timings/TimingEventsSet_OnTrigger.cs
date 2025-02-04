using ByteSheep.Events;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System;

/// <summary>Trigger action to start a TimingEventsSet</summary>
public class TimingEventsSet_OnTrigger : MonoBehaviour
{
    /// <summary>TimerSet</summary>
    public TimingEventsSet TES;

    /// <summary>StartTimer onTriggerEnter</summary>
    /// <param name="other">Other collider</param>
    public void OnTriggerEnter(Collider other)
    {
        //!@ Add checkes to ensure player hits it...
        if (TES)
        {
            TES.StartTimer();
        }
    }
}