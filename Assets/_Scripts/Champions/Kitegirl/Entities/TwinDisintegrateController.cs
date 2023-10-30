using System;
using _Scripts.Entities;
using UnityEngine;

namespace _Scripts.Champions.Kitegirl.Entities {
    public class TwinDisintegrateController : MonoBehaviour {
        private float rotationSpeed;
        private float totalRotation;
        private float initialTotalRotation;
        private float damage;

        private IDamager source;

        public void Init(IDamager source, float damage, float rotationSpeed, float totalRotation) {
            this.source = source;
            this.damage = damage;
            this.rotationSpeed = rotationSpeed;
            this.totalRotation = totalRotation;

            initialTotalRotation = totalRotation;
        }

        private void FixedUpdate() {
            transform.parent.Rotate(Vector3.up, rotationSpeed);
            totalRotation -= rotationSpeed;

            if (totalRotation <= 0 || totalRotation >= initialTotalRotation * 2) {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject == source.gameObject) {
                Debug.Log("Hit the source aka the player");
                return;
            }

            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null) {
                source.DealDamage(damageable, damage, Champion.DamageType.NON_BASIC);
            }
        }
    }
}