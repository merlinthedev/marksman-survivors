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

    public class EnemyHitEvent : Event {
        public Collider m_Collider { get; private set; }
        public Enemy m_Enemy { get; private set; }
        public Vector3 m_BulletHitPosition { get; private set; }

        public EnemyHitEvent(Collider collider, Enemy enemy, Vector3 bulletHitPosition) {
            m_Collider = collider;
            m_Enemy = enemy;
            m_BulletHitPosition = bulletHitPosition;
        }
    }

    public class EnemyKilledEvent : Event {
        public Collider m_Collider { get; private set; }
        public Enemy m_Enemy { get; private set; }
        public Vector3 m_EnemyDeathPosition { get; private set; }

        public EnemyKilledEvent(Collider collider, Enemy enemy, Vector3 enemyDeathPosition) {
            m_Collider = collider;
            m_Enemy = enemy;
            m_EnemyDeathPosition = enemyDeathPosition;
        }
    }
}