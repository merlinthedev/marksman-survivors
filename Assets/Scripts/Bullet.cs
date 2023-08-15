using Events;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    private Vector3 m_Target;
    private float m_Damage;
    [SerializeField] private float m_TravelSpeed = 30f;
    private bool m_ShouldMove = false;
    [SerializeField] private float m_BulletLifeTime = 2f;
    private float m_BulletSpawnTime = 0f;
    private bool m_ShouldChain = false;

    public void SetTarget(Vector3 target) {
        m_Target = target;
        m_ShouldMove = true;
        direction = (m_Target - transform.position).normalized;
        m_BulletSpawnTime = Time.time;
    }

    public void SetDamage(float damage) {
        m_Damage = damage;
    }

    private void Update() {
        if (Time.time > m_BulletSpawnTime + m_BulletLifeTime) {
            m_ShouldMove = false;
        }

        if (m_ShouldMove) {
            Move();
        } else {
            Destroy(gameObject);
        }
    }

    private Vector3 direction;

    private void Move() {
        transform.Translate(direction.normalized * (m_TravelSpeed * Time.deltaTime));
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.gameObject.CompareTag("Enemy")) return;
        Enemy enemy = other.gameObject.GetComponent<Enemy>();
        enemy.TakeDamage(m_Damage);
        // Debug.Log("Hit an enemy");

        // EventBus<EnemyHitEvent>.Raise(new EnemyHitEvent(other, enemy, gameObject.transform.position));

        if (m_ShouldChain) {
            FindChainTargets(gameObject.transform.position);
        }

        Destroy(gameObject);
    }

    private List<Enemy> m_ChainTargets = new List<Enemy>();

    private void FindChainTargets(Vector3 bulletHitPosition) {
        // TODO: Find all enemies in range of the bullet hit position
        m_ChainTargets.Add(EnemyManager.GetInstance().GetClosestEnemy(bulletHitPosition));
    }

    public void SetShouldChain(bool value) {
        m_ShouldChain = value;
    }
}