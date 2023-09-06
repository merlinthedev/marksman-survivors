using UnityEngine;

namespace Champions.Kitegirl.Entities {
    public class KitegirlSlowArea : MonoBehaviour {
        private Kitegirl m_Kitegirl;

        [SerializeField] private float m_SlowPercentage = 0.33f; // 0-1 Normalized
        [SerializeField] private float m_SlowDuration = 2f; // Seconds
        [SerializeField] private float m_ADDamageRatio = 0.5f; // 0-1 Normalized
        [SerializeField] private float m_LifeSpan = 0.5f;

        private float m_ThrowTime = 0f;

        public void OnThrow(Kitegirl sourceEntity) {
            m_Kitegirl = sourceEntity;

            m_ThrowTime = Time.time;
        }

        private void Update() {
            if (Time.time > m_ThrowTime + m_LifeSpan) {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.CompareTag("Enemy")) {
                Enemy enemy = EnemyManager.GetInstance().GetEnemy(other);

                enemy.ApplyDebuff(Debuff.CreateDebuff(m_Kitegirl, Debuff.DebuffType.Slow,
                    m_SlowDuration, m_SlowPercentage));
                enemy.TakeFlatDamage(m_Kitegirl.GetChampionStatistics().AttackDamage *
                                     m_ADDamageRatio);
            }
        }
    }
}