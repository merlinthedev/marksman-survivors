﻿using System;
using UnityEngine;

public class KitegirlBullet : ABullet {
    private bool m_ShouldChain;


    public void SetShouldChain(bool shouldChain) {
        m_ShouldChain = shouldChain;
    }

    protected private override void OnTriggerEnter(Collider other) {
        // Debug.Log("KitegirlBullet OnTriggerEnter called");
        if (other.gameObject.CompareTag("Enemy")) {
            if (m_ShouldChain) {
                Enemy enemy = other.gameObject.GetComponent<Enemy>();
                enemy.TakeDamage(this.m_Damage);
                Chain(other.gameObject.transform.position, enemy);
            } else {
                base.OnTriggerEnter(other);
            }
        }

        if (other.gameObject.CompareTag("KitegirlGrenade")) {
            // do stuff
            Debug.Log("Hit a grenade");
            KitegirlGrenade kitegirlGrenade = other.gameObject.GetComponent<KitegirlGrenade>();
            kitegirlGrenade.EarlyDetonate();
        }

    }


    private void Chain(Vector3 bulletHitPoint, Enemy alreadyHit, bool shouldRecurse = true) {
        while (true) {
            Debug.Log("Chaining");

            Enemy enemy = EnemyManager.GetInstance().GetClosestEnemy(bulletHitPoint, alreadyHit);
            enemy.TakeDamage(this.m_Damage);

            if (shouldRecurse) {
                bulletHitPoint = enemy.transform.position;
                shouldRecurse = false;
                Destroy(gameObject);
                continue;
            }

            break;
        }
    }

}