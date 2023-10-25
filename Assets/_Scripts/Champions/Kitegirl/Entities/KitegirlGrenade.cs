using _Scripts.BuffsDebuffs;
using _Scripts.BuffsDebuffs.Stacks;
using _Scripts.Core;
using _Scripts.Enemies;
using _Scripts.Entities;
using _Scripts.Util;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Champions.Kitegirl.Entities {
    public class KitegirlGrenade : MonoBehaviour, IThrowable, IDamageable {
        private Vector3 targetPoint;

        private Kitegirl kitegirl;

        [SerializeField] private Collider m_Collider;
        [SerializeField] private Rigidbody m_Rigidbody;

        [SerializeField] private float detonateTime = 2f;
        [SerializeField] private float damageRadius = 5f;

        [SerializeField] private GameObject explosionPrefab;

        private float damage = 0f;
        private float thrownTime = 0f;

        private bool destinationReached = false;
        private bool regularDetonationCancelled = false;

        public List<IAttachable> attachables { get; } = new();


        public void OnThrow(Vector3 targetPoint, IEntity sourceEntity) {
            this.targetPoint = targetPoint;
            kitegirl = (Kitegirl)sourceEntity;
            thrownTime = Time.time;
            m_Rigidbody.useGravity = false;
            m_Collider.isTrigger = true;
            // Debug.Log("OnThrow()", this);

            m_Rigidbody.velocity =
                (this.targetPoint - transform.position).normalized * 10f; // TODO: REFACTOR THIS, IT IS TERRIBLE


            Utilities.InvokeDelayed(() => Detonate(), detonateTime, this);
        }


        private void Update() {
            if (!destinationReached) {
                float distance = (targetPoint - transform.position).magnitude;
                if (distance < 0.1f) {
                    destinationReached = true;
                    m_Rigidbody.velocity = Vector3.zero;
                }
            }
        }

        private void Detonate(bool shot = false) {
            if (regularDetonationCancelled) return;
            // Debug.Log("Detonating");

            List<Enemy> enemiesInRange =
                EnemyManager.GetInstance().GetEnemiesInArea(transform.position, damageRadius);


            foreach (Enemy enemy in enemiesInRange) {
                float damage = enemy.GetMaxHealth() * 0.01f /* 1% of max health */ +
                               kitegirl.GetAttackDamage() *
                               0.01f; // 1% of AD
                // enemy.TakeFlatDamage((shot ? 2f : 1f) * m_Damage);
                kitegirl.DealDamage(enemy, (shot ? 2f : 1f) * this.damage, Champion.DamageType.NON_BASIC);
                enemy.ApplyDebuff(Debuff.CreateDebuff(enemy, kitegirl, Debuff.DebuffType.BURN, 5f, damage, 1f));
            }

            DamageableManager.GetInstance().RemoveDamageable(m_Collider);

            Instantiate(explosionPrefab, new Vector3(transform.position.x, 0.1f, transform.position.z),
                Quaternion.Euler(90, 0, 0));

            Destroy(gameObject);
        }

        private void EarlyDetonate() {
            // Debug.Log("Early detonating");
            Detonate(true);

            regularDetonationCancelled = true;
        }

        public void SetDamage(float damage) {
            this.damage = damage;
        }

        public void TakeFlatDamage(float damage) {
            EarlyDetonate();
        }

        public float CalculateIncomingDamage(float damage) {
            return this.damage;
        }

        public Transform GetTransform() {
            return gameObject.transform;
        }

        public Vector3 GetTargetPoint() {
            return targetPoint;
        }
    }
}