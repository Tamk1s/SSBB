//shoulda switch to quickevents before...

using ByteSheep.Events;
using UnityEngine;

/// <summary>Invokes AdvancedEvents via InvokeEm function</summary>
public class QuickEventInvoke : MonoBehaviour
{
    /// <summary>advancedEvent to invoke</summary>
    public AdvancedEvent advancedEvent;

    /// <summary>Call this to invoke da advancedEvent!</summary>
    public void InvokeEm()
    {
        advancedEvent.Invoke();
    }
}
