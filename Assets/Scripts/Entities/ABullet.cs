using UnityEngine;

namespace Entities {
    public abstract class ABullet : MonoBehaviour {
        protected IDamager sourceEntity;
        protected IDamageable targetEntity;
        [SerializeField] private float m_TravelSpeed = 30f;
        [SerializeField] private float m_BulletLifeTime = 2f;
        private float bulletSpawnTime = 0f;
        protected bool isFake = false;
        protected float damage;

        protected bool shouldMove = false;

        private Vector3 direction;

        public void SetSourceEntity(IDamager sourceEntity) {
            this.sourceEntity = sourceEntity;
        }

        public void SetTarget(IDamageable target) {

            shouldMove = true;
            
            // if (isFake) {
            //     direction = (transform.position - target.GetTransform().position).normalized;
            // }
            // else {
            //     direction = (sourceEntity.currentTarget.GetTransform().position - transform.position).normalized;
            // }

            bulletSpawnTime = Time.time;
            // clone the target into targetEntity so that we don't get null reference when the original target is set to null
            targetEntity = target;
        }

        public void SetDamage(float damage) {
            this.damage = damage;
        }

        public void IsFake() {
            isFake = true;
        }

        private void Update() {
            if (Time.time > bulletSpawnTime + m_BulletLifeTime) {
                shouldMove = false;
            }

            if (shouldMove) {
                Move();
            } else {
                Destroy(gameObject);
            }
        }


        private void Move() {
            if (targetEntity == null) {
                shouldMove = false;
                return;
            }

            //transform.Translate(direction.normalized * (m_TravelSpeed * Time.deltaTime));
            transform.position = Vector3.MoveTowards(transform.position,
                targetEntity.GetTransform().position,
                m_TravelSpeed * Time.deltaTime);
            
            if(isFake && transform.position == targetEntity.GetTransform().position) {
                Destroy(gameObject);
            }
        }

        private protected virtual void OnTriggerEnter(Collider other) {
            if(isFake) Destroy(gameObject);
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