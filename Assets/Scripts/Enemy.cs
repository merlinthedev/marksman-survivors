using Events;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {
    private Transform m_Target;
    [SerializeField] private GameObject m_EnemyDamageNumberPrefab;
    private Canvas m_Canvas;

    private Rigidbody m_Rigidbody;
    private Collider m_Collider;
    [SerializeField] private float m_MovementSpeed = 7f;


    private float m_MaxHealth = 250f;
    private float m_CurrentHealth = 250f;
    [SerializeField] private Image m_HealthBar;
    private float m_InitialHealthBarWidth;
    private bool m_CanMove = true;

    private bool m_IsDead {
        get => !m_CanMove;
    }

    private void OnMouseEnter() {
        EventBus<EnemyStartHoverEvent>.Raise(new EnemyStartHoverEvent());
    }

    private void OnMouseExit() {
        EventBus<EnemyStopHoverEvent>.Raise(new EnemyStopHoverEvent());
    }

    private void Start() {
        m_Rigidbody = GetComponent<Rigidbody>();
        if (m_Rigidbody == null) {
            throw new Exception("Missing rigidbody");
        }

        m_Canvas = GetComponentInChildren<Canvas>();
        if (m_Canvas == null) {
            throw new Exception("Missing canvas");
        }

        m_Collider = GetComponent<Collider>();
        if (m_Collider == null) {
            throw new Exception("Missing collider");
        }

        m_InitialHealthBarWidth = m_HealthBar.rectTransform.sizeDelta.x;
        UpdateHealthBar();
    }


    public void SetTarget(Transform target) {
        m_Target = target;
    }

    private void Update() {
        if (m_Target != null && m_CanMove) {
            Move();
        }
    }

    private void Move() {
        Vector3 direction = m_Target.position - transform.position;

        if (direction.magnitude < 0.1f) {
            m_Target = null;
            return;
        }

        m_Rigidbody.velocity = direction.normalized * m_MovementSpeed;
    }

    public void TakeDamage(float damage) {
        if (m_IsDead) return;
        m_CurrentHealth -= damage;

        ShowDamageUI(damage);
        UpdateHealthBar();

        if (m_CurrentHealth <= 0) {
            m_CanMove = false;
            m_Rigidbody.velocity = Vector3.zero;

            Invoke(nameof(Die), 0.5f);
        }
    }

    private void Die() {
        EventBus<EnemyKilledEvent>.Raise(new EnemyKilledEvent(m_Collider, this, transform.position));

        Destroy(gameObject);
    }

    private void ShowDamageUI(float damage) {
        // Instantiate the damage number prefab as a child of the canvas
        EnemyDamageNumberHelper enemyDamageNumberHelper = Instantiate(m_EnemyDamageNumberPrefab, m_Canvas.transform)
            .GetComponent<EnemyDamageNumberHelper>();

        enemyDamageNumberHelper.Initialize(damage.ToString());
    }

    private void UpdateHealthBar() {
        float healthPercentage = m_CurrentHealth / m_MaxHealth;

        m_HealthBar.rectTransform.sizeDelta = new Vector2(
            healthPercentage * m_InitialHealthBarWidth,
            m_HealthBar.rectTransform.sizeDelta.y
        );
    }

    private float m_LastAttackTime = 0f;
    [SerializeField] private float m_AttackCooldown = 3f;

    private void OnCollisionStay(Collision other) {
        if (other.gameObject.CompareTag("Player")) {
            if (m_IsDead) return;

            Player player = other.gameObject.TryGetComponent(out Player playerComponent)
                ? playerComponent
                : throw new Exception("Missing player component");

            if (Time.time > m_LastAttackTime + m_AttackCooldown) {
                player.TakeDamage(70f);
                m_LastAttackTime = Time.time;
            }
        }
    }
}