using UnityEngine;

// Scriptable object attribute
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
        get => attackSpeed;
        set => attackSpeed = value;
    }

    public float InitialMovementSpeed {
        get => initialMovementSpeed;
    }

    public float MovementSpeed {
        get => movementSpeed;
        set => movementSpeed = value;
    }

    public float InitialAttackSpeed {
        get => initialAttackSpeed;
    }

    public float AttackRange {
        get => attackRange;
        set => attackRange = value;
    }

    public float AttackDamage {
        get => attackDamage;
        set => attackDamage = value;
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
}