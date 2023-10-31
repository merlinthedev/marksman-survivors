using _Scripts.Champions;
using _Scripts.Entities;
using System.Collections;
using UnityEngine;

namespace _Scripts.Enemies {
    public class Brute : Enemy {
        [SerializeField] bool casting = false;
        [SerializeField] private float castTime = 1f;
        [SerializeField] private float chargeDistance = 10f;
        [SerializeField] private float chargeSpeed = 10f;
        [SerializeField] private float chargeCooldown = 5f;
        [SerializeField] private GameObject attackIndicator;
        private Vector3 chargeDirection;
        private Vector3 chargePosition;
        private Vector3 chargeTarget;
        private float lastChargeTime = 0f;

        private void FixedUpdate() {
            if(casting){
                if(Vector3.Distance(chargePosition, transform.position) > chargeDistance){
                    StopCharge();
                }
            }
        }

        protected override void Move() {
            if (casting) return;
            
            Vector3 direction = currentTarget.GetTransform().position - transform.position;
            if(direction.magnitude < chargeDistance && Time.time > chargeCooldown + lastChargeTime){
                StartCoroutine(Charge());
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
            StopCharge();
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

        private IEnumerator Charge() {
            casting = true;
            animator.SetTrigger("Charge");
            animator.SetBool("Charging", true);
            //turn off collision between layer enemy and layer enemy
            //a: enemy
            gameObject.layer = 11;

            //Get angle
            Vector3 direction = currentTarget.GetTransform().position - transform.position;
            float attackAngle = Vector3.SignedAngle(Vector3.forward, direction, Vector3.up);
            
            //Instantiate attack indicator
            var attackArea = Instantiate(attackIndicator, transform.position, Quaternion.Euler(0, attackAngle + 90, 0));
            
            //Set attack indicator position and scale
            attackArea.transform.position += direction.normalized * chargeDistance / 2;
            attackArea.transform.position = new Vector3(attackArea.transform.position.x, 0.01f, attackArea.transform.position.z);
            attackArea.transform.localScale = new Vector3(chargeDistance, 0.01f, 2f);
            
            //Set attack indicator variables
            attackArea.GetComponent<AttackIndicator>().direction = direction.normalized;
            attackArea.GetComponent<AttackIndicator>().distance = chargeDistance;
            attackArea.GetComponent<AttackIndicator>().castTime = castTime;
            
            //Save own position and target position
            chargePosition = transform.position;
            chargeTarget = transform.position + direction.normalized * chargeDistance;
            
            yield return new WaitForSeconds(castTime);
            
            Debug.Log("Charging");
            Destroy(attackArea);

            rigidbody.velocity = direction.normalized * chargeSpeed;
            lastChargeTime = Time.time;
        }

        private void StopCharge() {
            rigidbody.velocity = Vector3.zero;
            animator.SetBool("Charging", false);
            casting = false;
            gameObject.layer = 10;
        }
    }
}