using Events;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Util;
using Random = UnityEngine.Random;

public abstract class Champion : AAbilityHolder, IDamageable, IDebuffer {
    [SerializeField] protected Rigidbody m_Rigidbody;

    [SerializeField] protected Vector3 m_MouseHitPoint;
    private Vector3 m_LastKnownDirection = Vector3.zero;

    [SerializeField] protected ChampionStatistics m_ChampionStatistics;

    private ChampionLevelManager m_ChampionLevelManager;

    protected float m_LastAttackTime = 0f;
    private float m_GlobalMovementDirectionAngle = 0f;
    private float m_MovementMultiplier = 1f;
    private float m_DamageMultiplier = 1f;
    private float m_PreviousAngle = 0f;


    [SerializeField] protected bool m_CanMove = true;
    protected bool m_HasAttackCooldown = false;
    private bool m_NextAttackWillCrit = false;

    public bool IsBurning { get; set; }
    public bool IsFragile { get; set; }
    public float FragileStacks { get; set; }

    public float LastFragileApplyTime { get; set; }

    public List<IDamageable> AffectedEntities { get; set; } = new();
    public List<Debuff> Debuffs { get; } = new();


    public bool CanAttack {
        get => !m_HasAttackCooldown;
        protected set => m_HasAttackCooldown = !value;
    }

    public bool IsMoving => m_Rigidbody.velocity.magnitude > 0.001f;

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

    public void TakeFlatDamage(float damage) {
        OnDamageTaken(damage);
    }

    public void TakeBurnDamage(float damage, float interval, float time) {
        IsBurning = true;
        StartCoroutine(BurnDamageCoroutine(damage, interval, time));
    }

    public void RemoveDebuff(Debuff debuff) {
        Debuffs.Remove(debuff);

        throw new NotImplementedException();
    }

    public void ApplyDebuff(Debuff debuff) {
        Debuffs.Add(debuff);

        switch (debuff.GetDebuffType()) {
            case Debuff.DebuffType.SLOW:
                ApplySlow(debuff);
                break;
        }
    }

    private void ApplySlow(Debuff debuff) {
        m_ChampionStatistics.MovementSpeed *= 1 - debuff.GetValue();

        if (debuff.GetDuration() < 0) {
            return;
        }

        Utilities.InvokeDelayed(
            () => {
                m_ChampionStatistics.MovementSpeed = m_ChampionStatistics.InitialMovementSpeed;
                Debuffs.Remove(debuff);
            },
            debuff.GetDuration(), this);
    }

    private IEnumerator BurnDamageCoroutine(float damage, float interval, float time) {
        float startTime = Time.time;
        while (Time.time < startTime + time) {
            OnDamageTaken(damage);
            yield return new WaitForSeconds(interval);
        }

        IsBurning = false;
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

        // check for fragile stacks
        if (Time.time > LastFragileApplyTime + 10f) {
            // 10 for the duration of the stacks
            FragileStacks = 0;
        }
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

    public void Stop() {
        m_Rigidbody.velocity = Vector3.zero;
        m_MouseHitPoint = Vector3.zero;
    }

    protected virtual void OnMove() {
        if (m_MouseHitPoint == Vector3.zero || !m_CanMove) {
            return;
        }

        // Debug.Log("Moving");
        Vector3 direction = m_MouseHitPoint - transform.position;

        m_PreviousAngle = m_GlobalMovementDirectionAngle;
        m_GlobalMovementDirectionAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        if (direction.magnitude < 0.1f) {
            // Debug.Log("Stop moving");
            m_GlobalMovementDirectionAngle = m_PreviousAngle;
            m_MouseHitPoint = Vector3.zero;
            m_Rigidbody.velocity = Vector3.zero;
            return;
        }

        m_Rigidbody.velocity = direction.normalized * (m_ChampionStatistics.MovementSpeed * m_MovementMultiplier);
        m_LastKnownDirection = direction.normalized;
    }

    protected virtual void OnDamageTaken(float damage) {
        damage = IsFragile ? damage * 1 + FragileStacks / 10 : damage;
        m_ChampionStatistics.CurrentHealth -= damage;
        EventBus<ChampionDamageTakenEvent>.Raise(new ChampionDamageTakenEvent());
        if (m_ChampionStatistics.CurrentHealth <= 0) {
            OnDeath();
        }
    }

    protected virtual void OnDeath() {
        // Death logic

        Destroy(gameObject);
    }

    public void AddFragileStacks(float stacks) {
        if (FragileStacks < 1) IsFragile = true;
        FragileStacks += stacks;
    }

    public void RemoveFragileStacks(float stacks) {
        if (FragileStacks < stacks) {
            FragileStacks = 0;
            IsFragile = false;
        }
        else {
            FragileStacks -= stacks;
        }
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

        if (Random.value < m_ChampionStatistics.CriticalStrikeChance || m_NextAttackWillCrit) {
            damage *= (1 + m_ChampionStatistics.CriticalStrikeDamage);
        }

        if (m_NextAttackWillCrit) m_NextAttackWillCrit = false;

        return damage;
    }

    public Vector3 GetCurrentMovementDirection() {
        return m_Rigidbody.velocity.normalized == Vector3.zero ? m_LastKnownDirection : m_Rigidbody.velocity.normalized;
    }

    public float GetGlobalDirectionAngle() {
        // return the angle but instead of -180-180 i want it to be 0-360
        return m_GlobalMovementDirectionAngle < 0
            ? m_GlobalMovementDirectionAngle + 360
            : m_GlobalMovementDirectionAngle;
    }

    public void SetMouseHitPoint(Vector3 point) {
        m_MouseHitPoint = point;
    }

    protected void SetGlobalDirectionAngle(float angle) {
        m_GlobalMovementDirectionAngle = angle;
    }

    protected void SetCanMove(bool value) {
        m_CanMove = value;
        // Debug.Log("Move set ton true");
    }

    public void CleanseAllDebuffs() {
        throw new NotImplementedException();
    }

    protected void SetDamageMultiplier(float v) {
        m_DamageMultiplier = v; // hard set the multiplier
    }

    protected void ResetDamageMultiplier() {
        m_DamageMultiplier = 1f;
    }

    public void SetNextAttackWillCrit(bool b) {
        m_NextAttackWillCrit = b;
    }

    private void OnEnemyKilledEvent(EnemyKilledEvent e) {
        // TODO: XP
        m_ChampionStatistics.CurrentXP += e.m_Enemy.GetXP();
        m_ChampionLevelManager.CheckForLevelUp();
        EventBus<UpdateXPBarEvent>.Raise(new UpdateXPBarEvent(m_ChampionStatistics.CurrentXP, m_ChampionLevelManager.CurrentLevelXP));

    }

    public float GetCurrentHealth() => m_ChampionStatistics.CurrentHealth;
    public float GetMaxHealth() => m_ChampionStatistics.MaxHealth;
    public float GetAttackSpeed() => m_ChampionStatistics.AttackSpeed;
    public float GetLastAttackTime() => m_LastAttackTime;
    protected float GetCurrentMovementMultiplier() => m_MovementMultiplier;
    protected float GetDamageMultiplier() => m_DamageMultiplier;
    public Rigidbody GetRigidbody() => m_Rigidbody;
    public ChampionStatistics GetChampionStatistics() => m_ChampionStatistics;
    public ChampionLevelManager GetChampionLevelManager() => m_ChampionLevelManager;
}