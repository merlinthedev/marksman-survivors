using _Scripts.Champions;
using _Scripts.Core;
using _Scripts.Util;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Entities {
    public class StickyBomb : MonoBehaviour, IAttachable {
        private IDamager source;
        private IDamageable damageable;
        private float timeToExplode;
        private float radius;
        private float damagePercentage;

        public void Init(float timeToExplode, float radius, float damagePercentage) {
            this.timeToExplode = timeToExplode;
            this.radius = radius;
            this.damagePercentage = damagePercentage;
        }

        public void OnAttach(IDamageable damageable, IDamager source) {
            this.damageable = damageable;
            this.source = source;
            this.damageable.attachables.Add(this);

            Utilities.InvokeDelayed(OnUse, timeToExplode, this);
        }

        public void OnUse() {
            StopAllCoroutines(); // cancel for if we call this function before the timeToExplode is up.

            List<IDamageable> damageables = DamageableManager.GetInstance()
                .GetDamageablesInArea(damageable.GetTransform().position, radius);
            foreach (var dam in damageables) {
                source.DealDamage(dam, (float)(source as Champion)?.GetAttackDamage() * damagePercentage,
                    Champion.DamageType.BASIC);
            }


            Destroy(gameObject);
        }
    }
}