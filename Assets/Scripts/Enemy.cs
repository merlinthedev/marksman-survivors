using Events;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour {
    private Transform m_Target;
    [SerializeField] private GameObject m_EnemyDamageNumberPrefab;
    private Canvas m_Canvas;

    private Rigidbody m_Rigidbody;
    [SerializeField] private float m_MovementSpeed = 7f;


    private float m_Health = 250f;
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
        m_Health -= damage;

        ShowDamageUI(damage);

        if (m_Health <= 0) {
            m_CanMove = false;
            m_Rigidbody.velocity = Vector3.zero;

            Invoke(nameof(Die), 0.5f);
        }
    }

    private void Die() {
        Destroy(gameObject);
    }

    private void ShowDamageUI(float damage) {
        // Instantiate the damage number prefab as a child of the canvas
        EnemyDamageNumberHelper enemyDamageNumberHelper = Instantiate(m_EnemyDamageNumberPrefab, m_Canvas.transform)
            .GetComponent<EnemyDamageNumberHelper>();

        enemyDamageNumberHelper.Initialize(damage.ToString());
    }


    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Player")) {
            if (m_IsDead) return;
            // Debug.Log("Hit the player!");
            Player player = other.gameObject.GetComponent<Player>();
            player.TakeDamage(70f);
        }
    }
}