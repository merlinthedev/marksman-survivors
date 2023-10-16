using Core.Singleton;
using Entities;
using EventBus;
using System.Collections.Generic;
using UnityEngine;

namespace Core {
    public class DamageableManager : Singleton<DamageableManager> {
        private readonly List<IDamageable> damageables = new();

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

        public IDamageable GetClosestDamageable(Vector3 position, List<IDamageable> toIgnore = null) {
            float closestDistance = float.MaxValue;

            IDamageable closestDamageable = null;
            foreach (IDamageable damageable in damageables) {
                if (toIgnore != null && toIgnore.Contains(damageable)) {
                    continue;
                }

                float distance = Vector3.Distance(position, damageable.GetTransform().position);
                if (!(distance < closestDistance)) continue;
                closestDamageable = damageable;
                closestDistance = distance;
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