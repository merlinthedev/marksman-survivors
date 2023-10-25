using _Scripts.Enemies;
using _Scripts.Entities;
using System;
using UnityEngine;

namespace _Scripts.Champions.Kitegirl.Entities {
    public class KitegirlBullet : ABullet {
        private event Action<IDamageable> OnBulletHit;

        public void Init(Action<IDamageable> onBulletHit) {
            OnBulletHit += onBulletHit;
        }

        protected private override void OnTriggerEnter(Collider other) {
            if (this.isFake) return;
            // Debug.Log("KitegirlBullet OnTriggerEnter called");
            if (other.gameObject.CompareTag("Enemy")) {
                Enemy enemy = other.gameObject.GetComponent<Enemy>();

                // enemy.TakeFlatDamage(this.m_Damage);

                // enemy.TakeFlatDamage(damage);
                this.sourceEntity.DealDamage(enemy, this.damage, Champion.DamageType.BASIC);

                OnBulletHit?.Invoke(this.targetEntity);

                // TryReduceECooldown();
                if (!this.shouldPierce) {
                    Destroy(gameObject);
                }
            }

            if (other.gameObject.CompareTag("KitegirlGrenade")) {
                // do stuff
                // Debug.Log("Hit a grenade");
                KitegirlGrenade kitegirlGrenade = other.gameObject.GetComponent<KitegirlGrenade>();
                // kitegirlGrenade.TakeFlatDamage(1);
                this.sourceEntity.DealDamage(kitegirlGrenade, this.damage, Champion.DamageType.BASIC);
                this.shouldMove = this.shouldPierce;
                this.sourceEntity.ResetCurrentTarget();

                OnBulletHit?.Invoke(this.targetEntity);
            }
        }
    }
}