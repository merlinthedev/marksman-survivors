using BuffsDebuffs;
using Enemy;
using UnityEngine;
using UnityEngine.Serialization;

namespace Champions.Kitegirl.Entities {
    public class KitegirlSlowArea : MonoBehaviour {
        private Kitegirl kitegirl;

        [SerializeField] private float slowPercentage = 0.33f; // 0-1 Normalized
        [SerializeField] private float slowDuration = 3f; // Seconds
        [SerializeField] private float adDamageRatio = 1.2f; // 0-1 Normalized
        [SerializeField] private float lifeSpan = 0.2f;

        private float throwTime = 0f;

        public void OnThrow(Kitegirl sourceEntity) {
            kitegirl = sourceEntity;

            throwTime = Time.time;
        }

        private void Update() {
            if (Time.time > throwTime + lifeSpan) {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.CompareTag("Enemy")) {
                // Enemy.Enemy enemy = EnemyManager.GetInstance().GetEnemy(other);
                //
                // enemy.ApplyDebuff(Debuff.CreateDebuff(kitegirl, Debuff.DebuffType.Slow,
                //     slowDuration, slowPercentage));
                // enemy.TakeFlatDamage(kitegirl.GetAttackDamage() * adDamageRatio);
                IDebuffable debuffable = other.gameObject.GetComponent<IDebuffable>();
                debuffable.ApplyDebuff(Debuff.CreateDebuff(debuffable, kitegirl, Debuff.DebuffType.SLOW,
                    slowDuration, slowPercentage));
                kitegirl.DealDamage(debuffable, kitegirl.GetAttackDamage() * adDamageRatio);
            }
        }
    }
}