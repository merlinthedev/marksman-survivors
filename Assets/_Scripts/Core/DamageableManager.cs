using _Scripts.Core.Singleton;
using _Scripts.Entities;
using _Scripts.EventBus;
using _Scripts.Util;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Scripts.Core {
    public class DamageableManager : Singleton<DamageableManager> {
        private readonly Dictionary<Collider, IDamageable> damageables = new();

        private void OnEnable() {
            EventBus<EnemySpawnedEvent>.Subscribe(OnEnemySpawn);
            EventBus<EnemyKilledEvent>.Subscribe(OnEnemyKilled);
        }

        private void OnDisable() {
            EventBus<EnemySpawnedEvent>.Unsubscribe(OnEnemySpawn);
            EventBus<EnemyKilledEvent>.Unsubscribe(OnEnemyKilled);
        }


        public void AddDamageable(Collider collider, IDamageable damageable) {
            damageables.Add(collider, damageable);
        }

        public void RemoveDamageable(Collider collider) {
            damageables.Remove(collider);
        }

        public IDamageable GetClosestDamageable(Vector3 position, float maxDistance,
            List<IDamageable> toIgnore = null) {
            float closestDistance = float.MaxValue;

            IDamageable closestDamageable = null;
            foreach (IDamageable damageable in damageables.Values) {
                if (toIgnore != null && toIgnore.Contains(damageable)) {
                    continue;
                }

                float distance = Vector3.Distance(position, damageable.GetTransform().position);
                if (!(distance < closestDistance)) continue;
                if (distance > maxDistance) continue;
                closestDamageable = damageable;
                closestDistance = distance;
            }

            return closestDamageable;
        }

        public List<IDamageable> GetDamageablesInArea(Vector3 position, float radius, IDamageable toExclude = null) {
            List<IDamageable> damageablesInArea = new List<IDamageable>();

            foreach (var damageable in damageables.Values) {
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

            foreach (var damageable in damageables.Values) {
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
            AddDamageable(enemySpawnedEvent.enemy.GetComponent<Collider>(), enemySpawnedEvent.enemy);
        }

        private void OnEnemyKilled(EnemyKilledEvent enemyKilledEvent) {
            RemoveDamageable(enemyKilledEvent.enemy.GetComponent<Collider>());
        }

        public List<IDamageable> GetDamageables() {
            return damageables.Values.ToList();
        }

        public IDamageable GetDamageable(Collider collider) {
            IDamageable damageable;
            damageables.TryGetValue(collider, out damageable);
            return damageable;
        }
    }
}