using BuffsDebuffs;
using Entities;
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
            IDamageable damageable = other.gameObject.GetComponent<IDamageable>();

            if (damageable != null) {
                IDebuffable debuffable = other.gameObject.GetComponent<IDebuffable>();
                if (debuffable != null) {
                    if (debuffable != kitegirl) {
                        debuffable.ApplyDebuff(Debuff.CreateDebuff(debuffable, kitegirl, Debuff.DebuffType.SLOW,
                            slowDuration, slowPercentage));
                    }
                }

                if (damageable != kitegirl) {
                    kitegirl.DealDamage(damageable, kitegirl.GetAttackDamage() * ADDamageRatio,
                        Champion.DamageType.NON_BASIC);
                }
            }
        }
    }
}