using System.Collections.Generic;
using UnityEngine;
using Util;

namespace Champions.Kitegirl.Entities {
    public class KitegirlGrenade : MonoBehaviour, IThrowable {
        private Vector3 m_TargetPoint;

        private Kitegirl m_SourceEntity;

        [SerializeField] private Collider m_Collider;
        [SerializeField] private Rigidbody m_Rigidbody;

        [SerializeField] private float m_DetonateTime = 2f;
        [SerializeField] private float m_DamageRadius = 5f;
        private float m_Damage = 0f;
        private float m_ThrownTime = 0f;

        private bool m_DestinationReached = false;
        private bool m_RegularDetonationCancelled = false;


        public void OnThrow(Vector3 targetPoint, IEntity sourceEntity) {
            m_TargetPoint = targetPoint;
            m_SourceEntity = (Kitegirl)sourceEntity;
            m_ThrownTime = Time.time;
            m_Rigidbody.useGravity = false;
            // Debug.Log("OnThrow()", this);

            m_Rigidbody.velocity =
                (m_TargetPoint - transform.position).normalized * 10f; // TODO: REFACTOR THIS, IT IS TERRIBLE


            Utilities.InvokeDelayed(() => Detonate(), m_DetonateTime, this);
        }


        private void Update() {
            if (!m_DestinationReached) {
                float distance = (m_TargetPoint - transform.position).magnitude;
                if (distance < 0.1f) {
                    m_DestinationReached = true;
                    m_Rigidbody.velocity = Vector3.zero;
                }
            }
        }

        private void Detonate(bool shot = false) {
            if (m_RegularDetonationCancelled) return;
            // Debug.Log("Detonating");

            List<Enemy> enemiesInRange =
                EnemyManager.GetInstance().GetEnemiesInArea(transform.position, m_DamageRadius);


            foreach (Enemy enemy in enemiesInRange) {
                float damage = enemy.GetMaxHealth() * 0.01f /* 1% of max health */ +
                               m_SourceEntity.GetChampionStatistics().AttackDamage *
                               0.01f; // 1% of AD
                enemy.TakeFlatDamage((shot ? 2f : 1f) * m_Damage);
                enemy.ApplyDebuff(Debuff.CreateDebuff(enemy, Debuff.DebuffType.Burn, 5f, damage, 1f));
            }

            Destroy(gameObject);
        }

        public void EarlyDetonate() {
            // Debug.Log("Early detonating");
            Detonate(true);

            m_RegularDetonationCancelled = true;
        }

        public void SetDamage(float damage) {
            m_Damage = damage;
        }
    }
}