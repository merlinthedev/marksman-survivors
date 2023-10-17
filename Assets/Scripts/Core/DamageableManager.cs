using Core.Singleton;
using Entities;
using EventBus;
using System.Collections.Generic;
using UnityEngine;
using Util;

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

        public List<IDamageable> GetDamageablesInArea(Vector3 position, float radius, IDamageable toExclude = null) {
            List<IDamageable> damageablesInArea = new List<IDamageable>();

            foreach (var damageable in damageables) {
                if (damageable == toExclude) {
                    continue;
                }

                if (Vector3.Distance(position, damageable.GetTransform().position) <= radius) {
                    damageablesInArea.Add(damageable);
                }
            }

            return damageablesInArea;
        }

        public List<IDamageable> GetDamageablesInCone(Vector3 position, Vector3 leftConePoint, Vector3 rightConePoint,
            IDamageable toExclude = null) {
            List<IDamageable> damageablesInCone = new List<IDamageable>();

            foreach (var damageable in damageables) {
                if (damageable == toExclude) {
                    continue;
                }

                if (Utilities.IsInsideTriangle(new Vector2(position.x, position.z),
                        new Vector2(leftConePoint.x, leftConePoint.z), new Vector2(rightConePoint.x, rightConePoint.z),
                        new Vector2(damageable.GetTransform().position.x, damageable.GetTransform().position.z))) {
                    damageablesInCone.Add(damageable);
                }
            }

            return damageablesInCone;
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