using System;
using System.Collections.Generic;
using UnityEngine;

public class KitegirlBullet : ABullet {
    private bool m_ShouldChain;
    private float chainCount = 0;

    public void SetShouldChain(bool shouldChain) {
        m_ShouldChain = shouldChain;
    }


    protected private override void OnTriggerEnter(Collider other) {
        // Debug.Log("KitegirlBullet OnTriggerEnter called");
        if (other.gameObject.CompareTag("Enemy")) {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            if (m_ShouldChain) {
                enemy.TakeDamage(this.m_Damage);
                TryReduceECooldown();
                Chain(other.gameObject.transform.position, new List<Enemy> { enemy });
                Destroy(gameObject);
            }
            else {
                enemy.TakeDamage(this.m_Damage);
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
        enemy.TakeDamage(this.m_Damage);

        alreadyHit.Add(enemy);

        if (shouldRecurse) {
            // bulletHitPoint = enemy.transform.position;
            // shouldRecurse = false;
            Chain(enemy.transform.position, alreadyHit, false);
        }
    }

    private void TryReduceECooldown() {
        (this.m_SourceEntity as Kitegirl)?.TryReduceECooldown();
    }
}