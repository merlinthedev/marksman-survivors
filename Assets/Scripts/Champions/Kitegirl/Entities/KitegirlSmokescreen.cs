using System.Collections.Generic;
using BuffsDebuffs;
using BuffsDebuffs.Stacks;
using Enemy;
using UnityEngine;
using UnityEngine.Serialization;

namespace Champions.Kitegirl.Entities {
    public class KitegirlSmokescreen : MonoBehaviour {
        private Kitegirl kitegirl;


        [FormerlySerializedAs("m_SmokescreenDuration")] [SerializeField]
        private float smokescreenDuration = 5f;

        [FormerlySerializedAs("m_FragileStacks")] [SerializeField]
        private int fragileStacks = 10;

        [FormerlySerializedAs("m_SlowPercentage")] [SerializeField]
        private float slowPercentage = 0.33f; // Normalized! 0-1

        private float useTime = 0f;
        private Dictionary<Enemy.Enemy, Debuff> affectedEnemies = new();

        public void OnThrow(Kitegirl sourceEntity) {
            kitegirl = sourceEntity;
            useTime = Time.time;
            // Debug.Log("OnThrow()", this);
        }

        private void Update() {
            if (Time.time > useTime + smokescreenDuration) {
                foreach (KeyValuePair<Enemy.Enemy, Debuff> enemy in affectedEnemies) {
                    enemy.Key.RemoveDebuff(enemy.Value);
                }

                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.CompareTag("Enemy")) {
                Enemy.Enemy enemy = EnemyManager.GetInstance().GetEnemy(other);
                // enemy fragile stacks
                enemy.AddStacks(fragileStacks, Stack.StackType.FRAGILE);

                Debuff debuff = Debuff.CreateDebuff(enemy, kitegirl, Debuff.DebuffType.SLOW, -1,
                    slowPercentage);
                affectedEnemies.Add(enemy, debuff);
                enemy.ApplyDebuff(debuff); // -1 because we dont want the debuff to run out because of time, we want to
                //                              manually remove it from the entity
            }
        }

        private void OnTriggerExit(Collider other) {
            if (other.gameObject.CompareTag("Enemy")) {
                Enemy.Enemy enemy = EnemyManager.GetInstance().GetEnemy(other);
                if (affectedEnemies.ContainsKey(enemy)) {
                    enemy.RemoveDebuff(affectedEnemies[enemy]);
                    affectedEnemies.Remove(enemy);
                }
            }
        }
    }
}