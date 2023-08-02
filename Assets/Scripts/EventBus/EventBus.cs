using UnityEngine;

public abstract class EventBus<T> where T : Event {
    private static event System.Action<T> onEventRaised;

    public static void Subscribe(System.Action<T> action) {
        onEventRaised += action;
    }

    public static void Unsubscribe(System.Action<T> action) {
        onEventRaised -= action;
    }

    public static void Raise(T eventToRaise) {
        onEventRaised?.Invoke(eventToRaise);
    }
}

public abstract class Event {
}

namespace Events {
    public class EnemyStartHoverEvent : Event {
    }

    public class EnemyStopHoverEvent : Event {
    }
}