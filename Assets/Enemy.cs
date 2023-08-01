using System;
using UnityEngine;

public class Enemy : MonoBehaviour {
    private Transform m_Target;

    private Rigidbody m_Rigidbody;
    [SerializeField] private float m_MovementSpeed = 7f;

    private float m_Health = 250f;

    private void Start() {
        m_Rigidbody = GetComponent<Rigidbody>();

        if (m_Rigidbody == null) {
            throw new Exception("Missing rigidbody");
        }
    }


    public void SetTarget(Transform target) {
        m_Target = target;
    }

    private void Update() {
        if (m_Target != null) {
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
        m_Health -= damage;

        if (m_Health <= 0) {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Player")) {
            // Debug.Log("Hit the player!");
            Player player = other.gameObject.GetComponent<Player>();
            player.TakeDamage(70f);
        }
    }
}