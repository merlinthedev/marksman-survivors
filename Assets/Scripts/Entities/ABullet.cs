using BuffsDebuffs.Stacks;
using UnityEngine;

namespace Entities {
    public abstract class ABullet : MonoBehaviour {
        protected IEntity sourceEntity;
        [SerializeField] private float m_TravelSpeed = 30f;
        [SerializeField] private float m_BulletLifeTime = 2f;
        private float bulletSpawnTime = 0f;
        protected float damage;

        private bool shouldMove = false;

        private Vector3 direction;
        private Vector3 target;

        public void SetSourceEntity(IEntity sourceEntity) {
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
            transform.Translate(direction.normalized * (m_TravelSpeed * Time.deltaTime));
        }

        protected private virtual void OnTriggerEnter(Collider other) {
            // Debug.Log("ABullet base OnTriggerEnter called");
            if (other.gameObject.CompareTag("Enemy")) {
                Enemy.Enemy enemy = other.gameObject.GetComponent<Enemy.Enemy>();
                enemy.TakeFlatDamage(damage);
                // Debug.Log("Hit an enemy");

                Destroy(gameObject);
            }
        }
    }
}