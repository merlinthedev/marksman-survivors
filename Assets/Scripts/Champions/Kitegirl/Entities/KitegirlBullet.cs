using System.Collections.Generic;
using UnityEngine;

namespace Champions.Kitegirl.Entities {
    public class KitegirlBullet : ABullet {
        private bool m_ShouldChain;
        private float chainCount = 0;

        public void SetShouldChain(bool shouldChain) {
            m_ShouldChain = shouldChain;
        }


        private protected override void OnTriggerEnter(Collider other) {
            // Debug.Log("KitegirlBullet OnTriggerEnter called");
            if (other.gameObject.CompareTag("Enemy")) {
                Enemy enemy = other.gameObject.GetComponent<Enemy>();
                if (m_ShouldChain) {
                    // enemy.TakeFlatDamage(this.m_Damage);
                    if ((bool)(m_SourceEntity as global::Champions.Kitegirl.Kitegirl)?.HasUltimateActive()) {
                        if (enemy.IsFragile) {
                            enemy.Die();
                        }
                        else {
                            enemy.TakeFlatDamage(m_Damage);
                        }
                    }
                    else {
                        enemy.TakeFlatDamage(m_Damage);
                    }

                    TryReduceECooldown();
                    Chain(other.gameObject.transform.position, new List<Enemy> { enemy });
                    Destroy(gameObject);
                }
                else {
                    // enemy.TakeFlatDamage(this.m_Damage);
                    if ((bool)(m_SourceEntity as global::Champions.Kitegirl.Kitegirl)?.HasUltimateActive()) {
                        if (enemy.IsFragile) {
                            enemy.Die();
                        }
                        else {
                            enemy.TakeFlatDamage(m_Damage);
                        }
                    }
                    else {
                        enemy.TakeFlatDamage(m_Damage);
                    }

                    TryReduceECooldown();
                    Destroy(gameObject);
                }
            }

            if (other.gameObject.CompareTag("KitegirlGrenade")) {
                // do stuff
                // Debug.Log("Hit a grenade");
                KitegirlGrenade kitegirlGrenade = other.gameObject.GetComponent<KitegirlGrenade>();
                kitegirlGrenade.EarlyDetonate();
            }
        }


        private void Chain(Vector3 bulletHitPoint, List<Enemy> alreadyHit, bool shouldRecurse = true) {
            Enemy enemy = EnemyManager.GetInstance().GetClosestEnemy(bulletHitPoint, alreadyHit);
            this.m_Damage *= 0.8f;
            enemy.TakeFlatDamage(this.m_Damage);

            alreadyHit.Add(enemy);

            if (shouldRecurse) {
                // bulletHitPoint = enemy.transform.position;
                // shouldRecurse = false;
                Chain(enemy.transform.position, alreadyHit, false);
            }
        }

        private void TryReduceECooldown() {
            (this.m_SourceEntity as global::Champions.Kitegirl.Kitegirl)?.TryReduceECooldown();
        }
    }
}