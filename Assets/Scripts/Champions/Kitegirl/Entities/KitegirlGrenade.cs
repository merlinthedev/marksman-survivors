using System;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class KitegirlGrenade : MonoBehaviour, IThrowable {

    private Vector3 m_TargetPoint;

    [SerializeField] private Collider m_Collider;
    [SerializeField] private Rigidbody m_Rigidbody;

    [SerializeField] private float m_DetonateTime = 2f;
    [SerializeField] private float m_DamageRadius = 5f;
    [SerializeField] private float m_Damage = 45f;
    private float m_ThrownTime = 0f;

    private bool m_DestinationReached = false;
    private bool m_RegularDetonationCancelled = false;


    public void OnThrow() {
        m_ThrownTime = Time.time;
        m_Rigidbody.useGravity = false;
        // Debug.Log("OnThrow()", this);

        m_Rigidbody.velocity = (m_TargetPoint - transform.position).normalized * 10f; // TODO: REFACTOR THIS, IT IS TERRIBLE


        Utilities.InvokeDelayed(() => Detonate(), m_DetonateTime, this);
    }


    private void Update() {
        if (!m_DestinationReached) {
            float distance = (m_TargetPoint - transform.position).magnitude;
            if (distance < 0.1f) {
                m_DestinationReached = true;
                m_Rigidbody.velocity = Vector3.zero;
            }
        }
    }

    public void SetTargetPoint(Vector3 point) {
        m_TargetPoint = point;
    }

    private void Detonate(bool shot = false) {
        if (m_RegularDetonationCancelled) return;
        // Debug.Log("Detonating");

        List<Enemy> enemiesInRange = EnemyManager.GetInstance().GetEnemiesInArea(transform.position, m_DamageRadius);

        foreach (Enemy enemy in enemiesInRange) {
            enemy.TakeDamage((shot ? 2f : 1f) * m_Damage);
        }

        Destroy(gameObject);
    }

    public void EarlyDetonate() {
        // Debug.Log("Early detonating");
        Detonate(true);

        m_RegularDetonationCancelled = true;
    }
}