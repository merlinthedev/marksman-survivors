using System.Collections.Generic;
using UnityEngine;

public class KitegirlSmokescreen : MonoBehaviour, IThrowable {
    [SerializeField] private Rigidbody m_Rigidbody;

    private AEntity m_SourceEntity;

    private Vector3 m_TargetPoint;

    [SerializeField] private float m_SmokescreenDuration = 5f;
    [SerializeField] private float m_FragileStacks = 10f;
    [SerializeField] private float m_SlowPercentage = 0.33f; // Normalized! 0-1
    private float m_DestinationReachedTime = 0f;

    private bool m_DestinationReached = false;

    private Dictionary<Enemy, Debuff> m_AffectedEnemies = new();

    public void OnThrow(Vector3 point, AEntity sourceEntity) {
        m_TargetPoint = point;
        m_SourceEntity = sourceEntity;
        m_Rigidbody.useGravity = false;
        // Debug.Log("OnThrow()", this);

        m_Rigidbody.velocity =
            (m_TargetPoint - transform.position).normalized * 10f;
    }

    private void Update() {
        if (!m_DestinationReached) {
            float distance = (m_TargetPoint - transform.position).magnitude;
            if (distance < 0.1f) {
                m_DestinationReached = true;
                m_DestinationReachedTime = Time.time;
                m_Rigidbody.velocity = Vector3.zero;
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Enemy")) {
            Enemy enemy = EnemyManager.GetInstance().GetEnemy(other);
            Debuff debuff = Debuff.CreateDebuff(m_SourceEntity as Champion, Debuff.DebuffType.SLOW, -1,
                m_SlowPercentage);
            m_AffectedEnemies.Add(enemy, debuff);
            enemy.ApplyDebuff(debuff); // -1 because we dont want the debuff to run out because of time, we want to
            //                              manually remove it from the entity
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Enemy")) {
            Enemy enemy = EnemyManager.GetInstance().GetEnemy(other);
            if (m_AffectedEnemies.ContainsKey(enemy)) {
                enemy.RemoveDebuff(m_AffectedEnemies[enemy]);
                m_AffectedEnemies.Remove(enemy);
            }
        }
    }
}