using UnityEngine;

public class Bullet : MonoBehaviour {
    private Collider m_Target;
    private float m_Damage;
    [SerializeField] private float m_TravelSpeed = 30f;

    public void SetTarget(Collider target) {
        m_Target = target;
    }
    
    public void SetDamage(float damage) {
        m_Damage = damage;
    }

    private void Update() {
        if (m_Target != null) {
            Move();
        } else {
            Destroy(gameObject);
        }
    }

    private void Move() {
        Vector3 direction = m_Target.gameObject.transform.position - transform.position;

        transform.Translate(direction.normalized * (m_TravelSpeed * Time.deltaTime));
    }

    private void OnTriggerEnter(Collider other) {
        if (other != m_Target) return;
        Enemy enemy = other.gameObject.GetComponent<Enemy>();
        enemy.TakeDamage(m_Damage);
        // Debug.Log("Hit an enemy");
        Destroy(gameObject);
    }
}