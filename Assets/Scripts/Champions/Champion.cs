using System;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public abstract class Champion : AAbilityHolder {

    [SerializeField] protected Rigidbody m_Rigidbody;

    [SerializeField] protected Vector3 m_MouseHitPoint;

    [SerializeField] protected bool m_CanMove = true;

    [SerializeField] private float m_AttackSpeed;
    [SerializeField] protected float m_MovementSpeed;
    protected float m_LastAttackTime = 0f;
    [SerializeField] protected float m_Damage;
    [SerializeField] protected float m_MaxHealth;
    [SerializeField] protected float m_CurrentHealth;
    public bool CanAttack => Time.time > m_LastAttackTime + (1f / m_AttackSpeed);

    // Utility fields
    private Vector3 m_LastKnownDirection = Vector3.zero;
    private float m_GlobalMovementDirectionAngle = 0f;

    public abstract void OnAutoAttack();

    public void TakeDamage(float damage) {
        OnDamageTaken(damage);
    }

    private void Start() {
        m_CurrentHealth = m_MaxHealth;
    }

    protected virtual void Update() {
        if (m_MouseHitPoint != Vector3.zero && m_CanMove) {
            OnMove();
        }
    }

    protected void SetCanMove(bool value) {
        m_CanMove = value;
    }

    protected virtual void OnMove() {
        Debug.Log("Moving");
        Vector3 direction = m_MouseHitPoint - transform.position;

        m_GlobalMovementDirectionAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        if (direction.magnitude < 0.1f) {
            m_MouseHitPoint = Vector3.zero;
            m_Rigidbody.velocity = Vector3.zero;
            return;
        }

        m_Rigidbody.velocity = direction.normalized * m_MovementSpeed;
        m_LastKnownDirection = direction.normalized;
    }

    protected virtual void OnDamageTaken(float damage) {
        m_CurrentHealth -= damage;
        if (m_CurrentHealth <= 0) {
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

    public Vector3 GetCurrentMovementDirection() {
        return m_Rigidbody.velocity.normalized == Vector3.zero ? m_LastKnownDirection : m_Rigidbody.velocity.normalized;
    }

    private float GetGlobalDirectionAngle() {
        return m_GlobalMovementDirectionAngle;
    }

    public void SetMouseHitPoint(Vector3 point) {
        m_MouseHitPoint = point;
    }

    public float GetCurrentHealth() => m_CurrentHealth;
    public float GetMaxHealth() => m_MaxHealth;
    public float GetAttackSpeed() => m_AttackSpeed;
    public float GetLastAttackTime() => m_LastAttackTime;

}