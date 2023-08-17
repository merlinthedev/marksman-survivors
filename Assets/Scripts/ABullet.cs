using UnityEngine;

public abstract class ABullet : MonoBehaviour {
    [SerializeField] private float m_TravelSpeed = 30f;
    [SerializeField] private float m_BulletLifeTime = 2f;
    private float m_BulletSpawnTime = 0f;
    protected float m_Damage;

    private bool m_ShouldMove = false;

    private Vector3 m_Direction;
    private Vector3 m_Target;

    public virtual void SetTarget(Vector3 target) {
        m_Target = target;
        m_ShouldMove = true;
        m_Direction = (m_Target - transform.position).normalized;
        m_BulletSpawnTime = Time.time;
    }

    public virtual void SetDamage(float damage) {
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


    private void Move() {
        transform.Translate(m_Direction.normalized * (m_TravelSpeed * Time.deltaTime));
    }

    protected private virtual void OnTriggerEnter(Collider other) {
        if (!other.gameObject.CompareTag("Enemy")) return;
        Enemy enemy = other.gameObject.GetComponent<Enemy>();
        enemy.TakeDamage(m_Damage);
        Debug.Log("Hit an enemy");

        Destroy(gameObject);
    }

}