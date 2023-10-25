using System;
using EventBus;
using UnityEngine;

// Scriptable object attribute
namespace Champions {
    [CreateAssetMenu(fileName = "ChampionStatistics", menuName = "Champion/ChampionStatistics", order = 0)]
    public class ChampionStatistics : ScriptableObject {

        #region editor variables

        [SerializeField] private float m_MaxHealth;
        [SerializeField] private float m_MaxMana;
        [SerializeField] private float m_HealthRegen;
        [SerializeField] private float m_ManaRegen;
        [SerializeField] private float m_AttackSpeed;
        [SerializeField] private float m_MovementSpeed;
        [SerializeField] private float m_AttackRange; // Bullet lifetime
        [SerializeField] private float m_AttackDamage;

        [SerializeField] [Tooltip("This value has to be normalized")]
        private float m_CriticalStrikeChance; // NORMALIZED (0-1)

        [SerializeField] [Tooltip("This value has to be normalized")]
        private float m_CriticalStrikeDamage; // NORMALIZED (0-1)

        [SerializeField] private float m_CooldownReduction;

        [SerializeField] private float m_CurrentXP;

        #endregion

        #region internal variables

        private float maxHealth;
        private float currentHealth;
        private float maxMana;
        private float currentMana;
        private float healthRegen;
        private float manaRegen;
        private float initialAttackSpeed;
        private float attackSpeed;
        private float initialMovementSpeed;
        private float movementSpeed;
        private float attackRange;
        private float attackDamage;
        private float criticalStrikeChance;
        private float criticalStrikeDamage;
        private float cooldownReduction;

        private float currentXP;

        #endregion

        public void Initialize() {
            // Debug.Log("HELLO WORLD");

            maxHealth = m_MaxHealth;
            currentHealth = m_MaxHealth;
            maxMana = m_MaxMana;
            currentMana = m_MaxMana;
            healthRegen = m_HealthRegen;
            manaRegen = m_ManaRegen;
            initialAttackSpeed = m_AttackSpeed;
            attackSpeed = m_AttackSpeed;
            initialMovementSpeed = m_MovementSpeed;
            movementSpeed = m_MovementSpeed;
            attackRange = m_AttackRange;
            attackDamage = m_AttackDamage;
            criticalStrikeChance = m_CriticalStrikeChance;
            criticalStrikeDamage = m_CriticalStrikeDamage;
            cooldownReduction = m_CooldownReduction;

            currentXP = m_CurrentXP;
        }

        #region Getters and Setters

        public float MaxHealth {
            get => maxHealth;
            set => maxHealth = value;
        }

        public float CurrentHealth {
            get => currentHealth;
            set => currentHealth = value;
        }

        public float MaxMana {
            get => maxMana;
            set => maxMana = value;
        }

        public float CurrentMana {
            get => currentMana;
            set => currentMana = value;
        }

        public float HealthRegen {
            get => healthRegen;
            set => healthRegen = value;
        }

        public float ManaRegen {
            get => manaRegen;
            set => manaRegen = value;
        }

        public float AttackSpeed {
            get => attackSpeed * 1;
            set => attackSpeed = value;
        }

        public float GetAttackSpeed(float deftnessMultiplier) {
            return attackSpeed * deftnessMultiplier;
        }

        public float InitialMovementSpeed => initialMovementSpeed;

        public float MovementSpeed {
            get => movementSpeed;
            set => movementSpeed = value;
        }

        public float InitialAttackSpeed => initialAttackSpeed;

        public float AttackRange {
            get => attackRange;
            set => attackRange = value;
        }

        public float AttackDamage {
            get => attackDamage;
            set => attackDamage = value;
        }

        public float GetAttackDamage(float overpowerMultiplier) {
            return attackDamage * overpowerMultiplier;
        }

        public float CriticalStrikeChance {
            get => criticalStrikeChance;
            set => criticalStrikeChance = value;
        }

        public float CriticalStrikeDamage {
            get => criticalStrikeDamage;
            set => criticalStrikeDamage = value;
        }

        public float CooldownReduction {
            get => cooldownReduction;
            set => cooldownReduction = value;
        }

        public float CurrentXP {
            get => currentXP;
            set => currentXP = value;
        }

        #endregion

        public float GetStatisticByEnum(Statistic statistic) {
            switch (statistic) {
                case Statistic.MAX_HEALTH:
                    return MaxHealth;
                case Statistic.MAX_MANA:
                    return MaxMana;
                case Statistic.HEALTH_REGEN:
                    return HealthRegen;
                case Statistic.MANA_REGEN:
                    return ManaRegen;
                case Statistic.ATTACK_SPEED:
                    return AttackSpeed;
                case Statistic.MOVEMENT_SPEED:
                    return MovementSpeed;
                case Statistic.ATTACK_RANGE:
                    return AttackRange;
                case Statistic.ATTACK_DAMAGE:
                    return AttackDamage;
                case Statistic.CRITICAL_STRIKE_CHANCE:
                    return CriticalStrikeChance;
                case Statistic.CRITICAL_STRIKE_DAMAGE:
                    return CriticalStrikeDamage;
                case Statistic.COOLDOWN_REDUCTION:
                    return CooldownReduction;
                case Statistic.CURRENT_XP:
                    return CurrentXP;
                default:
                    return 0;
            }
        }

        public void MultiplyStatisticByPercentage(Statistic statistic, float percentage) {
            switch (statistic) {
                case Statistic.MAX_HEALTH:
                    MaxHealth *= percentage;
                    break;
                case Statistic.MAX_MANA:
                    MaxMana *= percentage;
                    break;
                case Statistic.HEALTH_REGEN:
                    HealthRegen *= percentage;
                    break;
                case Statistic.MANA_REGEN:
                    ManaRegen *= percentage;
                    break;
                case Statistic.ATTACK_SPEED:
                    AttackSpeed *= percentage;
                    break;
                case Statistic.MOVEMENT_SPEED:
                    MovementSpeed *= percentage;
                    break;
                case Statistic.ATTACK_RANGE:
                    AttackRange *= percentage;
                    break;
                case Statistic.ATTACK_DAMAGE:
                    Util.Logger.Log("Adding percentage " + percentage + " to attack damage", Util.Logger.Color.RED,
                        Player.GetInstance());
                    AttackDamage *= percentage;

                    Util.Logger.Log("Attack damage is now: " + AttackDamage, Util.Logger.Color.RED,
                        Player.GetInstance());
                    break;
                case Statistic.CRITICAL_STRIKE_CHANCE:
                    CriticalStrikeChance *= percentage;
                    break;
                case Statistic.CRITICAL_STRIKE_DAMAGE:
                    CriticalStrikeDamage *= percentage;
                    break;
                case Statistic.COOLDOWN_REDUCTION:
                    CooldownReduction *= percentage;
                    break;
                case Statistic.CURRENT_XP:
                default:
                    Util.Logger.Log("Statistic not found or could not be edited", Util.Logger.Color.RED,
                        Player.GetInstance());
                    break;
            }
        }

        public void DivideStatisticByPercentage(Statistic statistic, float percentage) {
            switch (statistic) {
                case Statistic.MAX_HEALTH:
                    MaxHealth /= percentage;
                    break;
                case Statistic.MAX_MANA:
                    MaxMana /= percentage;
                    break;
                case Statistic.HEALTH_REGEN:
                    HealthRegen /= percentage;
                    break;
                case Statistic.MANA_REGEN:
                    ManaRegen /= percentage;
                    break;
                case Statistic.ATTACK_SPEED:
                    AttackSpeed /= percentage;
                    break;
                case Statistic.MOVEMENT_SPEED:
                    MovementSpeed /= percentage;
                    break;
                case Statistic.ATTACK_RANGE:
                    AttackRange /= percentage;
                    break;
                case Statistic.ATTACK_DAMAGE:
                    Util.Logger.Log("Adding percentage " + percentage + " to attack damage", Util.Logger.Color.RED,
                        Player.GetInstance());
                    AttackDamage /= percentage;

                    Util.Logger.Log("Attack damage is now: " + AttackDamage, Util.Logger.Color.RED,
                        Player.GetInstance());
                    break;
                case Statistic.CRITICAL_STRIKE_CHANCE:
                    CriticalStrikeChance /= percentage;
                    break;
                case Statistic.CRITICAL_STRIKE_DAMAGE:
                    CriticalStrikeDamage /= percentage;
                    break;
                case Statistic.COOLDOWN_REDUCTION:
                    CooldownReduction /= percentage;
                    break;
                case Statistic.CURRENT_XP:
                default:
                    Util.Logger.Log("Statistic not found or could not be edited", Util.Logger.Color.RED,
                        Player.GetInstance());
                    break;
            }
        }

        public void AddToStatistic(Statistic statistic, float value) {
            switch (statistic) {
                case Statistic.MAX_HEALTH:
                    MaxHealth += value;
                    break;
                case Statistic.MAX_MANA:
                    MaxMana += value;
                    break;
                case Statistic.CURRENT_MANA:
                    CurrentMana += value;
                    if (CurrentMana > MaxMana) CurrentMana = MaxMana;
                    EventBus<UpdateResourceBarEvent>.Raise(new UpdateResourceBarEvent("Mana", CurrentMana, MaxMana));
                    break;
                case Statistic.HEALTH_REGEN:
                    HealthRegen += value;
                    break;
                case Statistic.MANA_REGEN:
                    ManaRegen += value;
                    break;
                case Statistic.ATTACK_SPEED:
                    AttackSpeed += value;
                    break;
                case Statistic.MOVEMENT_SPEED:
                    MovementSpeed += value;
                    break;
                case Statistic.ATTACK_RANGE:
                    AttackRange += value;
                    break;
                case Statistic.ATTACK_DAMAGE:
                    Util.Logger.Log("Adding " + value + " to attack damage", Util.Logger.Color.RED,
                        Player.GetInstance());
                    AttackDamage += value;

                    Util.Logger.Log("Attack damage is now: " + AttackDamage, Util.Logger.Color.RED,
                        Player.GetInstance());
                    break;
                case Statistic.CRITICAL_STRIKE_CHANCE:
                    CriticalStrikeChance += value;
                    break;
                case Statistic.CRITICAL_STRIKE_DAMAGE:
                    CriticalStrikeDamage += value;
                    break;
                case Statistic.COOLDOWN_REDUCTION:
                    CooldownReduction += value;
                    break;
                case Statistic.CURRENT_XP:
                default:
                    Util.Logger.Log("Statistic not found or could not be edited", Util.Logger.Color.RED,
                        Player.GetInstance());
                    break;
            }
        }

        public void DeductFromStatistic(Statistic statistic, float value) {
            switch (statistic) {
                case Statistic.MAX_HEALTH:
                    MaxHealth -= value;
                    break;
                case Statistic.MAX_MANA:
                    MaxMana -= value;
                    break;
                case Statistic.HEALTH_REGEN:
                    HealthRegen -= value;
                    break;
                case Statistic.CURRENT_MANA:
                    CurrentMana -= value;
                    EventBus<UpdateResourceBarEvent>.Raise(new UpdateResourceBarEvent("Mana", CurrentMana, MaxMana));
                    break;
                case Statistic.MANA_REGEN:
                    ManaRegen -= value;
                    break;
                case Statistic.ATTACK_SPEED:
                    AttackSpeed -= value;
                    break;
                case Statistic.MOVEMENT_SPEED:
                    MovementSpeed -= value;
                    break;
                case Statistic.ATTACK_RANGE:
                    AttackRange -= value;
                    break;
                case Statistic.ATTACK_DAMAGE:
                    Util.Logger.Log("Removing " + value + " to attack damage", Util.Logger.Color.RED,
                        Player.GetInstance());
                    AttackDamage -= value;

                    Util.Logger.Log("Attack damage is now: " + AttackDamage, Util.Logger.Color.RED,
                        Player.GetInstance());
                    break;
                case Statistic.CRITICAL_STRIKE_CHANCE:
                    CriticalStrikeChance -= value;
                    break;
                case Statistic.CRITICAL_STRIKE_DAMAGE:
                    CriticalStrikeDamage -= value;
                    break;
                case Statistic.COOLDOWN_REDUCTION:
                    CooldownReduction -= value;
                    break;
                case Statistic.CURRENT_XP:
                default:
                    Util.Logger.Log("Statistic not found or could not be edited", Util.Logger.Color.RED,
                        Player.GetInstance());
                    break;
            }
        }
    }

    [Serializable]
    public enum Statistic {
        MAX_HEALTH,
        CURRENT_HEALTH,
        HEALTH_REGEN,
        MAX_MANA,
        CURRENT_MANA,
        MANA_REGEN,
        ATTACK_SPEED,
        MOVEMENT_SPEED,
        ATTACK_RANGE,
        ATTACK_DAMAGE,
        CRITICAL_STRIKE_CHANCE,
        CRITICAL_STRIKE_DAMAGE,
        COOLDOWN_REDUCTION,
        CURRENT_XP
    }
}