using System;
using Champions;
using Core;
using UnityEngine;

namespace Entities {
    public class Projectile : MonoBehaviour {
        [SerializeField] protected float projectileSpeed = 0;
        private event Action<IDamageable> OnHit;
        protected IDamager source;
        private Vector3 target;

        private Vector3 startPoint;
        private float range;
        protected bool shouldMove = true;

        public void Init(IDamager source, Vector3 target, Action<IDamageable> OnHit, float projectileSpeed,
            float range) {
            this.source = source;
            this.target = target;
            this.projectileSpeed = projectileSpeed;
            this.OnHit += OnHit;
            this.range = range;

            startPoint = source.gameObject.transform.position;
        }

        protected virtual void Update() {
            if (shouldMove) {
                Move();
            } else {
                Debug.Log("Destroying projectile..." + gameObject.name);
                Destroy(gameObject);
            }
        }

        protected virtual void Move() {
            transform.position = Vector3.MoveTowards(transform.position, target, projectileSpeed * Time.deltaTime);
            Debug.Log("Base Move()");

            float dist = Vector3.Distance(startPoint, transform.position);
            // Debug.Log("Distance: " + dist);

            if (dist > range) {
                // Debug.Log("Projectile out of range, destroying...");
                shouldMove = false;
                OnHit = null;
            }
        }

        protected virtual void OnTriggerEnter(Collider other) {
            IDamageable damageable = DamageableManager.GetInstance().GetDamageable(other);
            if (damageable != null && damageable is not Champion) {
                OnHit?.Invoke(damageable);
                shouldMove = false;
                Destroy(gameObject);
            }
        }
    }
}