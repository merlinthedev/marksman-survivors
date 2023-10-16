using System.Collections.Generic;
using Enemies;
using Entities;
using UnityEngine;

namespace Champions.Kitegirl.Entities {
    public class KitegirlBullet : ABullet {
        private bool shouldChain;
        private float chainCount = 0;

        public void SetShouldChain(bool shouldChain) {
            this.shouldChain = shouldChain;
        }


        private protected override void OnTriggerEnter(Collider other) {
            // Debug.Log("KitegirlBullet OnTriggerEnter called");
            if (other.gameObject.CompareTag("Enemy")) {
                Enemy enemy = other.gameObject.GetComponent<Enemy>();
                if (shouldChain) {
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

                    TryReduceECooldown();
                    Chain(other.gameObject.transform.position, new List<Enemy> { enemy });
                    Destroy(gameObject);
                } else {
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

                    TryReduceECooldown();
                    Destroy(gameObject);
                }
            }

            if (other.gameObject.CompareTag("KitegirlGrenade")) {
                // do stuff
                // Debug.Log("Hit a grenade");
                KitegirlGrenade kitegirlGrenade = other.gameObject.GetComponent<KitegirlGrenade>();
                // kitegirlGrenade.TakeFlatDamage(1);
                sourceEntity.DealDamage(kitegirlGrenade, damage);
                shouldMove = false;
                sourceEntity.ResetCurrentTarget();
            }
        }


        private void Chain(Vector3 bulletHitPoint, List<Enemy> alreadyHit, bool shouldRecurse = true) {
            Enemy enemy = EnemyManager.GetInstance().GetClosestEnemy(bulletHitPoint, alreadyHit);
            damage *= 0.5f;
            // enemy.TakeFlatDamage(damage);
            sourceEntity.DealDamage(enemy, damage);
            alreadyHit.Add(enemy);

            if (shouldRecurse) {
                // bulletHitPoint = enemy.transform.position;
                // shouldRecurse = false;
                Chain(enemy.transform.position, alreadyHit, false);
            }
        }

        private void TryReduceECooldown() {
            (sourceEntity as Kitegirl)?.TryReduceECooldown();
        }
    }
}