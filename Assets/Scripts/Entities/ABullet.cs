using BuffsDebuffs.Stacks;
using UnityEngine;

namespace Entities {
    public abstract class ABullet : MonoBehaviour {
        protected IDamager sourceEntity;
        [SerializeField] private float m_TravelSpeed = 30f;
        [SerializeField] private float m_BulletLifeTime = 2f;
        private float bulletSpawnTime = 0f;
        protected float damage;

        private bool shouldMove = false;

        private Vector3 direction;
        private Vector3 target;

        public void SetSourceEntity(IDamager sourceEntity) {
            this.sourceEntity = sourceEntity;
        }

        public virtual void SetTarget(Vector3 target) {
            this.target = target;
            shouldMove = true;
            direction = (this.target - transform.position).normalized;
            bulletSpawnTime = Time.time;
        }

        public virtual void SetDamage(float damage) {
            this.damage = damage;
        }

        private void Update() {
            if (Time.time > bulletSpawnTime + m_BulletLifeTime) {
                shouldMove = false;
            }

            if (shouldMove) {
                Move();
            }
            else {
                Destroy(gameObject);
            }
        }


        private void Move() {
            //transform.Translate(direction.normalized * (m_TravelSpeed * Time.deltaTime));
            transform.position = Vector3.MoveTowards(transform.position, target, m_TravelSpeed * Time.deltaTime);
        }

        protected private virtual void OnTriggerEnter(Collider other) {
            // Debug.Log("ABullet base OnTriggerEnter called");
            if (other.gameObject.CompareTag("Enemy")) {
                IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
                // damageable.TakeFlatDamage(damage);
                // Debug.Log("Hit an enemy");
                sourceEntity.DealDamage(damageable, damage);

                Destroy(gameObject);
            }
        }
    }
}