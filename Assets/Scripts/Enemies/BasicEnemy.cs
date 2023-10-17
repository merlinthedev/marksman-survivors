using Champions;
using Entities;
using UnityEngine;

namespace Enemies {
    public class BasicEnemy : Enemy {
        protected override void Move() {
            Vector3 direction = currentTarget.GetTransform().position - transform.position;

            if (direction.magnitude < 0.1f) {
                currentTarget = null;
                return;
            }

            rigidbody.velocity = direction.normalized * movementSpeed;
        }

        public override void DealDamage(IDamageable damageable, float damage, bool shouldInvoke = true) {
            // Debug.Log("Enemy dealing damage");
            damageable.TakeFlatDamage(damage);
        }

        private void OnCollisionStay(Collision other) {
            if (other.gameObject.CompareTag("Player")) {
                Collision(other.collider);
            }
        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.CompareTag("Player")) {
                // Logger.Log("Collision with player", this);
                Collision(other);
            }
        }

        private void Collision(Collider other) {
            if (isDead || !canAttack) return;

            Champion champion = other.gameObject.GetComponent<Champion>();
            if (champion == null) {
                return;
            }

            if (Time.time > lastAttackTime + attackCooldown) {
                // champion.TakeFlatDamage(damage);
                // Logger.Log("Dealing damage", this);
                DealDamage(champion, damage);
                lastAttackTime = Time.time;
            }
        }
    }
}