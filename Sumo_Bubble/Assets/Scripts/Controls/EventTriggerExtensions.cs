using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// Extension for Event Triggers to AddListenter
/// https://discussions.unity.com/t/how-do-you-add-an-ui-eventtrigger-by-script/125158/6
/// </summary>
public static class EventTriggerExtensions
{        
    public static void AddListener(this EventTrigger eventTrigger, EventTriggerType triggerType,
        UnityAction<BaseEventData> call)
    {
        if (eventTrigger == null)
            throw new System.ArgumentNullException(nameof(eventTrigger));
        if (call == null)
            throw new System.ArgumentNullException(nameof(call));

        EventTrigger.Entry entry = eventTrigger.triggers.Find(e => e.eventID == triggerType);
        if (entry == null)
        {
            entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
            eventTrigger.triggers.Add(entry);
        }

        entry.callback.AddListener(call);
    }

    public static void RemoveListener(this EventTrigger eventTrigger, EventTriggerType triggerType,
        UnityAction<BaseEventData> call)
    {
        if (eventTrigger == null)
            throw new System.ArgumentNullException(nameof(eventTrigger));
        if (call == null)
            throw new System.ArgumentNullException(nameof(call));

        EventTrigger.Entry entry = eventTrigger.triggers.Find(e => e.eventID == triggerType);
        entry?.callback.RemoveListener(call);
    }

    public static void RemoveAllListeners(this EventTrigger eventTrigger, EventTriggerType triggerType)
    {
        if (eventTrigger == null)
            throw new System.ArgumentNullException(nameof(eventTrigger));

        EventTrigger.Entry entry = eventTrigger.triggers.Find(e => e.eventID == triggerType);
        entry?.callback.RemoveAllListeners();
    }
}