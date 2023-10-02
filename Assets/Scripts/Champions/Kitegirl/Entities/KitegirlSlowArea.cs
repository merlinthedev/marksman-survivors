using BuffsDebuffs;
using UnityEngine;

namespace Champions.Kitegirl.Entities {
    public class KitegirlSlowArea : MonoBehaviour {
        private Kitegirl kitegirl;

        private float throwTime = 0f;

        private float lifespan;
        private float slowPercentage;
        private float slowDuration;
        private float ADDamageRatio;

        public void SetLifespan(float lifespan) {
            this.lifespan = lifespan;
        }

        public void SetSlowPercentage(float slowPercentage) {
            this.slowPercentage = slowPercentage;
        }

        public void SetSlowDuration(float slowDuration) {
            this.slowDuration = slowDuration;
        }

        public void SetADDamageRatio(float ADDamageRatio) {
            this.ADDamageRatio = ADDamageRatio;
        }

        public void OnThrow(Kitegirl sourceEntity) {
            kitegirl = sourceEntity;

            throwTime = Time.time;
        }

        private void Update() {
            if (Time.time > throwTime + lifespan) {
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
                kitegirl.DealDamage(debuffable, kitegirl.GetAttackDamage() * ADDamageRatio);
            }
        }
    }
}