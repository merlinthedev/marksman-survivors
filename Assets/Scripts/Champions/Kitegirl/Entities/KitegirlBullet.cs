using System;
using System.Collections.Generic;
using Core;
using Enemies;
using Entities;
using UnityEditor.Rendering;
using UnityEngine;

namespace Champions.Kitegirl.Entities {
    public class KitegirlBullet : ABullet {
        private event Action<IDamageable> OnBulletHit;

        public void Init(Action<IDamageable> onBulletHit) {
            OnBulletHit += onBulletHit;
        }

        private protected override void OnTriggerEnter(Collider other) {
            if (isFake) return;
            // Debug.Log("KitegirlBullet OnTriggerEnter called");
            if (other.gameObject.CompareTag("Enemy")) {
                Enemy enemy = other.gameObject.GetComponent<Enemy>();

                // enemy.TakeFlatDamage(this.m_Damage);
                if ((bool)(sourceEntity as Kitegirl)?.HasUltimateActive()) {
                    if (enemy.IsFragile) {
                        enemy.Die();
                    } else {
                        // enemy.TakeFlatDamage(damage);
                        sourceEntity.DealDamage(enemy, damage);
                    }
                } else {
                    // enemy.TakeFlatDamage(damage);
                    sourceEntity.DealDamage(enemy, damage);
                }

                OnBulletHit?.Invoke(targetEntity);

                // TryReduceECooldown();
                Destroy(gameObject);
            }

            if (other.gameObject.CompareTag("KitegirlGrenade")) {
                // do stuff
                // Debug.Log("Hit a grenade");
                KitegirlGrenade kitegirlGrenade = other.gameObject.GetComponent<KitegirlGrenade>();
                // kitegirlGrenade.TakeFlatDamage(1);
                sourceEntity.DealDamage(kitegirlGrenade, damage);
                shouldMove = false;
                sourceEntity.ResetCurrentTarget();

                OnBulletHit?.Invoke(targetEntity);
            }
        }
    }
}