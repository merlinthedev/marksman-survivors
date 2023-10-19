using System;
using Champions;
using Core;
using UnityEngine;

namespace Entities {
    public class Projectile : MonoBehaviour {
        [SerializeField] private float projectileSpeed;
        private event Action<IDamageable> OnHit;
        private IDamager source;
        private Vector3 target;

        private bool shouldMove = true;

        public void Init(IDamager source, Vector3 target, Action<IDamageable> OnHit) {
            this.source = source;
            this.target = target;
            this.OnHit += OnHit;
        }

        private void Update() {
            if (shouldMove) {
                Move();
            }
        }

        private void Move() {
            transform.position = Vector3.MoveTowards(transform.position, target, projectileSpeed * Time.deltaTime);

            float dist = Vector3.Distance(transform.position, target);
            if (dist <= 0.1f) {
                shouldMove = false;
                OnHit = null;
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other) {
            IDamageable damageable = DamageableManager.GetInstance().GetDamageable(other);
            if (damageable != null && damageable is not Champion) {
                OnHit?.Invoke(damageable);
                shouldMove = false;
                Destroy(gameObject);
            }
        }
    }
}