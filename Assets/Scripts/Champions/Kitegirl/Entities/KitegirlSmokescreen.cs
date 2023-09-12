using System.Collections.Generic;
using BuffsDebuffs;
using BuffsDebuffs.Stacks;
using Enemy;
using UnityEngine;

namespace Champions.Kitegirl.Entities {
    public class KitegirlSmokescreen : MonoBehaviour {
        private Kitegirl m_Kitegirl;


        [SerializeField] private float m_SmokescreenDuration = 5f;
        [SerializeField] private int m_FragileStacks = 10;
        [SerializeField] private float m_SlowPercentage = 0.33f; // Normalized! 0-1
        private float m_UseTime = 0f;
        [SerializeReference] private Dictionary<Enemy.Enemy, Debuff> m_AffectedEnemies = new();

        public void OnThrow(Kitegirl sourceEntity) {
            m_Kitegirl = sourceEntity;
            m_UseTime = Time.time;
            // Debug.Log("OnThrow()", this);
        }

        private void Update() {
            if (Time.time > m_UseTime + m_SmokescreenDuration) {
                foreach (KeyValuePair<Enemy.Enemy, Debuff> enemy in m_AffectedEnemies) {
                    enemy.Key.RemoveDebuff(enemy.Value);
                }

                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.CompareTag("Enemy")) {
                Enemy.Enemy enemy = EnemyManager.GetInstance().GetEnemy(other);
                // enemy fragile stacks
                enemy.AddStacks(m_FragileStacks, Stack.StackType.FRAGILE);

                Debuff debuff = Debuff.CreateDebuff(m_Kitegirl, Debuff.DebuffType.Slow, -1,
                    m_SlowPercentage);
                m_AffectedEnemies.Add(enemy, debuff);
                enemy.ApplyDebuff(debuff); // -1 because we dont want the debuff to run out because of time, we want to
                //                              manually remove it from the entity
            }
        }

        private void OnTriggerExit(Collider other) {
            if (other.gameObject.CompareTag("Enemy")) {
                Enemy.Enemy enemy = EnemyManager.GetInstance().GetEnemy(other);
                if (m_AffectedEnemies.ContainsKey(enemy)) {
                    enemy.RemoveDebuff(m_AffectedEnemies[enemy]);
                    m_AffectedEnemies.Remove(enemy);
                }
            }
        }
    }
}