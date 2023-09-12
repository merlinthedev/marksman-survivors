using Champions.Abilities;
using UnityEngine;

namespace EventBus {
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

    public abstract class Event { }

    public class EnemyStartHoverEvent : Event { }

    public class EnemyStopHoverEvent : Event { }

    public class EnemyHitEvent : Event {
        public Collider m_Collider { get; private set; }
        public Enemy.Enemy m_Enemy { get; private set; }
        public Vector3 m_BulletHitPosition { get; private set; }

        public EnemyHitEvent(Collider collider, Enemy.Enemy enemy, Vector3 bulletHitPosition) {
            m_Collider = collider;
            m_Enemy = enemy;
            m_BulletHitPosition = bulletHitPosition;
        }
    }

    public class EnemyKilledEvent : Event {
        public Collider m_Collider { get; private set; }
        public Enemy.Enemy m_Enemy { get; private set; }
        public Vector3 m_EnemyDeathPosition { get; private set; }

        public EnemyKilledEvent(Collider collider, Enemy.Enemy enemy, Vector3 enemyDeathPosition) {
            m_Collider = collider;
            m_Enemy = enemy;
            m_EnemyDeathPosition = enemyDeathPosition;
        }
    }

    public class ChampionHealthRegenerated : Event { }

    public class ChampionManaRegenerated : Event { }

    public class ChampionAbilitiesHookedEvent : Event { }

    public class ChampionDamageTakenEvent : Event {
    }

    public class ChampionLevelUpEvent : Event {
        public int m_CurrentLevel { get; private set; }
        public int m_PreviousLevel { get; private set; }

        public ChampionLevelUpEvent(int currentLevel, int previousLevel) {
            m_CurrentLevel = currentLevel;
            m_PreviousLevel = previousLevel;
        }
    }

    public class UpdateResourceBarEvent : Event {

        public string m_Type { get; private set; }
        public float m_Current { get; private set; }
        public float m_Total { get; private set; }

        public UpdateResourceBarEvent(string type, float current, float total) {
            m_Type = type;
            m_Current = current;
            m_Total = total;
        }
    }

    public class ChampionAbilityUsedEvent : Event {
        public AAbility m_Ability { get; private set; }

        public KeyCode m_KeyCode { get; private set; }
        public float m_Duration { get; private set; }

        public ChampionAbilityUsedEvent(AAbility ability) {
            m_Ability = ability;
        }

        public ChampionAbilityUsedEvent(KeyCode keyCode, float duration) {
            m_KeyCode = keyCode;
            m_Duration = duration;
            m_Ability = null;
        }
    }
}