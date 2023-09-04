using Events;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class Champion : AAbilityHolder {
    [SerializeField] protected Rigidbody m_Rigidbody;

    [SerializeField] protected Vector3 m_MouseHitPoint;
    private Vector3 m_LastKnownDirection = Vector3.zero;

    [SerializeField] protected ChampionStatistics m_ChampionStatistics;

    private ChampionLevelManager m_ChampionLevelManager;

    protected float m_LastAttackTime = 0f;
    private float m_GlobalMovementDirectionAngle = 0f;
    private float m_MovementMultiplier = 1f;
    private float m_DamageMultiplier = 1f;


    [SerializeField] protected bool m_CanMove = true;
    protected bool m_HasAttackCooldown = false;

    public bool CanAttack {
        get {
            return !m_HasAttackCooldown;
        }
        protected set {
            m_HasAttackCooldown = !value;
        }
    }

    public bool IsMoving {
        get {
            return m_Rigidbody.velocity.magnitude > 0.001f;
        }
    }

    private void OnEnable() {
        // Debug.Log("Champion onEnable");

        EventBus<EnemyKilledEvent>.Subscribe(OnEnemyKilledEvent);
    }

    private void OnDisable() {
        // Debug.Log("Champion onDisable");

        EventBus<EnemyKilledEvent>.Unsubscribe(OnEnemyKilledEvent);
    }

    public abstract void OnAutoAttack(Collider collider);

    public abstract void OnAbility(KeyCode keyCode);

    public void TakeDamage(float damage) {
        OnDamageTaken(damage);
    }

    protected virtual void Start() {
        // Debug.Log("Champion start");

        m_ChampionLevelManager = new ChampionLevelManager(this);

        m_ChampionStatistics.Initialize();
    }

    protected virtual void Update() {
        if (m_MouseHitPoint != Vector3.zero) {
            // Debug.Log("Received mouse hit point");
            if (m_CanMove) {
                // Debug.Log("Can move");
                OnMove();
            }
        }

        RegenerateResources();
    }

    private void RegenerateResources() {
        TryRegenerateHealth();
        TryRegenerateMana();
    }

    private float m_LastHealthRegenerationTime = 0f;

    private void TryRegenerateHealth() {
        if (m_ChampionStatistics.CurrentHealth < m_ChampionStatistics.MaxHealth) {
            if (Time.time > m_LastHealthRegenerationTime + 1f) {
                m_ChampionStatistics.CurrentHealth += m_ChampionStatistics.HealthRegen;
                m_LastHealthRegenerationTime = Time.time;
                EventBus<ChampionHealthRegenerated>.Raise(new ChampionHealthRegenerated());
            }
        }
    }

    private float m_LastManaRegenerationTime = 0f;

    private void TryRegenerateMana() {
        if (m_ChampionStatistics.CurrentMana < m_ChampionStatistics.MaxMana) {
            if (Time.time > m_LastManaRegenerationTime + 1f) {
                m_ChampionStatistics.CurrentMana += m_ChampionStatistics.ManaRegen;
                m_LastManaRegenerationTime = Time.time;
                EventBus<ChampionManaRegenerated>.Raise(new ChampionManaRegenerated());
            }
        }
    }

    protected void SetCanMove(bool value) {
        m_CanMove = value;
        // Debug.Log("Move set ton true");
    }

    private float previousAngle = 0f;

    protected virtual void OnMove() {
        if (m_MouseHitPoint == Vector3.zero || !m_CanMove) {
            return;
        }
        // Debug.Log("Moving");
        Vector3 direction = m_MouseHitPoint - transform.position;

        previousAngle = m_GlobalMovementDirectionAngle;
        m_GlobalMovementDirectionAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        if (direction.magnitude < 0.1f) {
            // Debug.Log("Stop moving");
            m_GlobalMovementDirectionAngle = previousAngle;
            m_MouseHitPoint = Vector3.zero;
            m_Rigidbody.velocity = Vector3.zero;
            return;
        }

        m_Rigidbody.velocity = direction.normalized * (m_ChampionStatistics.MovementSpeed * m_MovementMultiplier);
        m_LastKnownDirection = direction.normalized;
    }

    protected virtual void OnDamageTaken(float damage) {
        m_ChampionStatistics.CurrentHealth -= damage;
        if (m_ChampionStatistics.CurrentHealth <= 0) {
            OnDeath();
        }
    }

    protected virtual void OnDeath() {
        // Death logic

        Destroy(gameObject);
    }

    protected void DrawDirectionRays() {
        // Direction ray
        Debug.DrawRay(transform.position, GetCurrentMovementDirection() * 10f, Color.red);

        // Right ray
        Vector3 rightDirection = Quaternion.Euler(0, 45, 0) * GetCurrentMovementDirection();
        // Debug.DrawRay(transform.position, rightDirection * 10f, Color.green);
        // Draw the ray until the edge of the viewport
        Debug.DrawRay(transform.position, rightDirection * 10f, Color.green, 0);

        // Left ray
        Vector3 leftDirection = Quaternion.Euler(0, -45, 0) * GetCurrentMovementDirection();
        Debug.DrawRay(transform.position, leftDirection * 10f, Color.blue, 0);
    }

    protected float CalculateDamage() {
        float damage = m_ChampionStatistics.AttackDamage;

        if (Random.value < m_ChampionStatistics.CriticalStrikeChance) {
            damage *= (1 + m_ChampionStatistics.CriticalStrikeDamage);
        }

        return damage;
    }

    public Vector3 GetCurrentMovementDirection() {
        return m_Rigidbody.velocity.normalized == Vector3.zero ? m_LastKnownDirection : m_Rigidbody.velocity.normalized;
    }

    public float GetGlobalDirectionAngle() {
        // return the angle but instead of -180-180 i want it to be 0-360
        return m_GlobalMovementDirectionAngle < 0 ? m_GlobalMovementDirectionAngle + 360 : m_GlobalMovementDirectionAngle;
    }

    public void SetMouseHitPoint(Vector3 point) {
        m_MouseHitPoint = point;
    }

    protected void SetGlobalDirectionAngle(float angle) {
        m_GlobalMovementDirectionAngle = angle;
    }

    protected void SetMovementDebuff(float v) {
        m_MovementMultiplier -= v; // for example -> v = 0.3f = 30% debuff so the multiplier will be 0.7f
    }

    public void SetMovementMultiplier(float v) {
        m_MovementMultiplier = v; // hard set the multiplier
    }

    protected void ResetMovementMultiplier() {
        m_MovementMultiplier = 1f; // reset the multiplier
    }

    public void CleanseAllDebuffs() {
        ResetMovementMultiplier();
    }

    protected void SetDamageMultiplier(float v) {
        m_DamageMultiplier = v; // hard set the multiplier
    }

    protected void ResetDamageMultiplier() {
        m_DamageMultiplier = 1f;
    }

    private void OnEnemyKilledEvent(EnemyKilledEvent e) {
        // TODO: XP
        m_ChampionStatistics.CurrentXP += e.m_Enemy.GetXP();

        m_ChampionLevelManager.CheckForLevelUp();
    }

    public float GetCurrentHealth() => m_ChampionStatistics.CurrentHealth;
    public float GetMaxHealth() => m_ChampionStatistics.MaxHealth;
    public float GetAttackSpeed() => m_ChampionStatistics.AttackSpeed;
    public float GetLastAttackTime() => m_LastAttackTime;
    protected float GetCurrentMovementMultiplier() => m_MovementMultiplier;
    protected float GetDamageMultiplier() => m_DamageMultiplier;
    public Rigidbody GetRigidbody() => m_Rigidbody;
    public ChampionStatistics GetChampionStatistics() => m_ChampionStatistics;

}