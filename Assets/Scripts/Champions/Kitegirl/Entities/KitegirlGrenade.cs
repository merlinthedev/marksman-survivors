using System.Collections.Generic;
using UnityEngine;
using Util;

public class KitegirlGrenade : MonoBehaviour, IThrowable {

    private Vector3 m_TargetPoint;

    [SerializeField] private float m_DetonateTime = 2f;
    [SerializeField] private float m_DamageRadius = 5f;
    [SerializeField] private float m_Damage = 45f;
    private float m_ThrownTime = 0f;

    public void OnThrow() {
        m_ThrownTime = Time.time;

        Utilities.InvokeDelayed(Detonate, m_DetonateTime, this);
    }

    public void SetTargetPoint(Vector3 point) {
        m_TargetPoint = point;
    }

    private void Detonate() {
        Debug.Log("Detonating");

        List<Enemy> enemiesInRange = EnemyManager.GetInstance().GetEnemiesInArea(transform.position, m_DamageRadius);

        foreach (Enemy enemy in enemiesInRange) {
            enemy.TakeDamage(m_Damage);
        }

        Destroy(gameObject);
    }
}