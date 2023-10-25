using _Scripts.Champions;
using _Scripts.Entities;
using UnityEngine;

namespace _Scripts.Enemies {
    public class BasicEnemy : Enemy {
        protected override void Move() {
            Vector3 direction = currentTarget.GetTransform().position - transform.position;

            if (direction.magnitude < 0.1f) {
                currentTarget = null;
                return;
            }

            this.rigidbody.velocity = direction.normalized * this.movementSpeed;
        }

        public override void DealDamage(IDamageable damageable, float damage, Champion.DamageType damageType,
            bool shouldInvoke = true) {
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
            if (isDead || !this.canAttack) return;

            Champion champion = other.gameObject.GetComponent<Champion>();
            if (champion == null) {
                return;
            }

            if (Time.time > this.lastAttackTime + this.attackCooldown) {
                // champion.TakeFlatDamage(damage);
                // Logger.Log("Dealing damage", this);
                DealDamage(champion, this.damage, Champion.DamageType.BASIC);
                this.lastAttackTime = Time.time;
            }
        }
    }
}