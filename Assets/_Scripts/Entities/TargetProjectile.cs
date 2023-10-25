using _Scripts.Core;
using UnityEngine;

namespace _Scripts.Entities {
    public class TargetProjectile : Projectile {
        private IDamageable target;

        private event System.Action<IDamageable> OnHit;


        public void Init(IDamager source, IDamageable target, System.Action<IDamageable> OnHit, float projectileSpeed) {
            this.source = source;
            this.target = target;
            this.projectileSpeed = projectileSpeed;
            this.OnHit += OnHit;
        }

        protected override void Move() {
            transform.position = Vector3.MoveTowards(transform.position, target.GetTransform().position, this.projectileSpeed * Time.deltaTime);

            Debug.Log("Override Move()");

        }

        protected override void OnTriggerEnter(Collider other) {
            IDamageable damageable = DamageableManager.GetInstance().GetDamageable(other);
            Debug.Log("Collision...");
            if (damageable == target) {
                OnHit?.Invoke(damageable);
                this.shouldMove = false;

                Debug.Log("Destroying projectile..." + gameObject.name);
                Destroy(gameObject);
            } else {
                Debug.Log("Not target...");
            }
        }
    }
}