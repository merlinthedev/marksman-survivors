using Core.Singleton;
using Entities;
using EventBus;
using Interactable;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core {
    public class DamageableManager : Singleton<DamageableManager> {

        private readonly List<IDamageable> damageables = new List<IDamageable>();

        private void OnEnable() {
            EventBus<EnemySpawnedEvent>.Subscribe(OnEnemySpawn);
            EventBus<EnemyKilledEvent>.Subscribe(OnEnemyKilled);
        }

        private void OnDisable() {
            EventBus<EnemySpawnedEvent>.Unsubscribe(OnEnemySpawn);
            EventBus<EnemyKilledEvent>.Unsubscribe(OnEnemyKilled);

        }


        public void AddDamageable(IDamageable damageable) {
            damageables.Add(damageable);
        }

        public void RemoveDamageable(IDamageable damageable) {
            damageables.Remove(damageable);
        }

        public IDamageable GetClosestDamageable(Vector3 position) {
            IDamageable closestDamageable = null;
            float closestDistance = float.MaxValue;

            foreach (IDamageable damageable in damageables) {
                float distance = Vector3.Distance(position, damageable.GetTransform().position);
                if (distance < closestDistance) {
                    closestDamageable = damageable;
                    closestDistance = distance;
                }
            }

            return closestDamageable;
        }

        private void OnEnemySpawn(EnemySpawnedEvent enemySpawnedEvent) {
            AddDamageable(enemySpawnedEvent.enemy);
        }

        private void OnEnemyKilled(EnemyKilledEvent enemyKilledEvent) {
            RemoveDamageable(enemyKilledEvent.enemy);
        }

        public List<IDamageable> GetDamageables() {
            return damageables;
        }


    }
}