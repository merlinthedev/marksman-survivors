using System.Collections.Generic;
using UnityEngine;

namespace Champions.Kitegirl.Entities {
    public class KitegirlSmokescreen : MonoBehaviour {
        private AEntity m_SourceEntity;


        [SerializeField] private float m_SmokescreenDuration = 5f;
        [SerializeField] private float m_FragileStacks = 10f;
        [SerializeField] private float m_SlowPercentage = 0.33f; // Normalized! 0-1
        private float m_UseTime = 0f;
        [SerializeReference] private Dictionary<Enemy, Debuff> m_AffectedEnemies = new();

        public void OnThrow(AEntity sourceEntity) {
            m_SourceEntity = sourceEntity;
            m_UseTime = Time.time;
            // Debug.Log("OnThrow()", this);
        }

        private void Update() {
            if (Time.time > m_UseTime + m_SmokescreenDuration) {
                foreach (KeyValuePair<Enemy, Debuff> enemy in m_AffectedEnemies) {
                    enemy.Key.RemoveDebuff(enemy.Value);
                }

                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.CompareTag("Enemy")) {
                Enemy enemy = EnemyManager.GetInstance().GetEnemy(other);
                // enemy fragile stacks
                enemy.AddFragileStacks(m_FragileStacks);

                Debuff debuff = Debuff.CreateDebuff(m_SourceEntity as Champion, Debuff.DebuffType.SLOW, -1,
                    m_SlowPercentage);
                m_AffectedEnemies.Add(enemy, debuff);
                enemy.ApplyDebuff(debuff); // -1 because we dont want the debuff to run out because of time, we want to
                //                              manually remove it from the entity
            }
        }

        private void OnTriggerExit(Collider other) {
            if (other.gameObject.CompareTag("Enemy")) {
                Enemy enemy = EnemyManager.GetInstance().GetEnemy(other);
                enemy.RemoveFragileStacks(m_FragileStacks);
                if (m_AffectedEnemies.ContainsKey(enemy)) {
                    enemy.RemoveDebuff(m_AffectedEnemies[enemy]);
                    m_AffectedEnemies.Remove(enemy);
                }
            }
        }
    }
}