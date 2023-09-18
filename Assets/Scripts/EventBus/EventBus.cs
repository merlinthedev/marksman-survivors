using System.Collections.Generic;
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

    public class ChampionDamageTakenEvent : Event { }

    public class ChampionAbilityUsedEvent : Event {
        public AAbility AbstractAbility { get; private set; }

        public KeyCode KeyCode { get; private set; }
        public float Duration { get; private set; }

        public ChampionAbilityUsedEvent(AAbility ability) {
            AbstractAbility = ability;
        }

        public ChampionAbilityUsedEvent(KeyCode keyCode, float duration) {
            KeyCode = keyCode;
            Duration = duration;
            AbstractAbility = null;
        }
    }

    public class ChampionLevelUpEvent : Event {
        public int m_CurrentLevel { get; private set; }
        public int m_PreviousLevel { get; private set; }
        public List<AAbility> m_ChampionAbilities { get; private set; }

        public ChampionLevelUpEvent(int currentLevel, int previousLevel, List<AAbility> championAbilities) {
            m_CurrentLevel = currentLevel;
            m_PreviousLevel = previousLevel;
            m_ChampionAbilities = championAbilities;
        }
    }

    public class ChampionAbilityChosenEvent : Event {
        public AAbility Ability { get; private set; }
        
        public ChampionAbilityChosenEvent(AAbility ability) {
            Ability = ability;
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
}