using UnityEngine;

namespace Champions.Kitegirl.Entities {
    public class KitegirlSlowArea : MonoBehaviour {
        private AEntity m_SourceEntity;

        [SerializeField] private float m_SlowPercentage = 0.33f; // 0-1 Normalized
        [SerializeField] private float m_SlowDuration = 2f; // Seconds
        [SerializeField] private float m_ADDamageRatio = 0.5f; // 0-1 Normalized
        [SerializeField] private float m_LifeSpan = 0.5f;

        private float m_ThrowTime = 0f;

        public void OnThrow(AEntity sourceEntity) {
            m_SourceEntity = sourceEntity;

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

                enemy.ApplyDebuff(Debuff.CreateDebuff(m_SourceEntity as Champion, Debuff.DebuffType.SLOW,
                    m_SlowDuration, m_SlowPercentage));
                enemy.TakeFlatDamage((float)(m_SourceEntity as Champion)?.GetChampionStatistics().AttackDamage *
                                     m_ADDamageRatio);
            }
        }
    }
}