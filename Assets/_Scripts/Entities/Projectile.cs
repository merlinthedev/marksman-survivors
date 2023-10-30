using _Scripts.Champions;
using _Scripts.Core;
using System;
using UnityEngine;

namespace _Scripts.Entities {
    public class Projectile : MonoBehaviour {
        [SerializeField] protected float projectileSpeed = 0;
        public bool piercing;
        private int pierceCount = 0;
        public int maxPierce = 1;
        private event Action<IDamageable> OnHit;
        protected IDamager source;
        private Vector3 target;

        private Vector3 startPoint;
        private float range;
        protected bool shouldMove = true;

        public void Init(IDamager source, Vector3 target, Action<IDamageable> OnHit, float projectileSpeed,
            float range, bool piercing = false, int maxPierce = 0) {
            this.source = source;
            this.target = target;
            this.projectileSpeed = projectileSpeed;
            this.OnHit += OnHit;
            this.range = range;
            this.piercing = piercing;
            this.maxPierce = maxPierce;

            startPoint = source.gameObject.transform.position;
        }

        private void Update() {
            if (shouldMove) {
                Move();
            } else {
                // Debug.Log("Destroying projectile...");
                Destroy(gameObject);
            }
        }

        protected virtual void Move() {
            transform.position = Vector3.MoveTowards(transform.position, target, projectileSpeed * Time.deltaTime);

            float dist = Vector3.Distance(startPoint, transform.position);
            // Debug.Log("Distance: " + dist);

            if (dist > range) {
                // Debug.Log("Projectile out of range, destroying...");
                shouldMove = false;
                OnHit = null;
            }
            else if (piercing && pierceCount > maxPierce && maxPierce > 0) {
                shouldMove = false;
                OnHit = null;
            }
        }

        protected virtual void OnTriggerEnter(Collider other) {
            IDamageable damageable = DamageableManager.GetInstance().GetDamageable(other);
            if (damageable != null && damageable is not Champion) {
                OnHit?.Invoke(damageable);
                if(!piercing) shouldMove = false;
            }
            if(piercing) pierceCount++;
        }
    }
}