using _Scripts.Champions;
using _Scripts.Entities;
using System.Collections;
using UnityEngine;

namespace _Scripts.Enemies {
    public class Mage : Enemy {
        private bool casting = false;
        [SerializeField] private float castTime = 1f;
        [SerializeField] private GameObject attackIndicator;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private float projectileSpeed = 10f;
        [SerializeField] private float attackRange = 10f;
        [SerializeField] private float attackSize = 2f;
        [SerializeField] private float attackCooldown = 5f;
        private Vector3 targetPos;

        protected override void Move() {
            if (casting) return;
            
            targetPos = currentTarget.GetTransform().position;
            Vector3 direction = targetPos - transform.position;
            if(Vector3.Distance(targetPos, transform.position) < attackRange && Time.time > attackCooldown + lastAttackTime){
                StartCoroutine(Attack());
                return;
            }

            if (direction.magnitude < 0.1f) {
                currentTarget = null;
                return;
            }

            rigidbody.velocity = direction.normalized * movementSpeed;
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
            if (isDead || !canAttack) return;

            Champion champion = other.gameObject.GetComponent<Champion>();
            if (champion == null) {
                return;
            }

            if (Time.time > lastAttackTime + attackCooldown) {
                DealDamage(champion, damage, Champion.DamageType.BASIC);
                lastAttackTime = Time.time;
            }
        }

        private IEnumerator Attack() {
            casting = true;

            gameObject.layer = 11;
            
            var attackArea = Instantiate(attackIndicator, new Vector3(targetPos.x, 0, targetPos.z), Quaternion.identity);
            attackArea.transform.localScale = new Vector3(attackSize, 0.01f, attackSize);
            attackArea.GetComponent<AttackIndicator>().castTime = castTime;
            Destroy(attackArea, castTime);
            yield return new WaitForSeconds(castTime);

            //attack
            lastAttackTime = Time.time;
            
            StopAttack();

        }

        private void StopAttack() {
            gameObject.layer = 10;
            casting = false;
        }
    }
}