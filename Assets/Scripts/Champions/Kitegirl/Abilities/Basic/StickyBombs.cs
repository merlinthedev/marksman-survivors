using System.Collections.Generic;
using Champions.Abilities;
using Core;
using Entities;
using UnityEngine;
using Util;

namespace Champions.Kitegirl.Abilities {
    public class StickyBombs : Ability {
        [SerializeField] private float timeToExplode = 3f;
        [SerializeField] private float damagePercentage = 0.1f;
        [SerializeField] private float damageArea = 5f;

        public override void Hook(Champion champion) {
            base.Hook(champion);
            champion.OnBulletHit += Use;
        }

        private void Use(IDamageable damageable) {
            Utilities.InvokeDelayed(() => {
                List<IDamageable> damageables = DamageableManager.GetInstance()
                    .GetDamageablesInArea(damageable.GetTransform().position, damageArea);
                damageables.ForEach(d => { champion.DealDamage(d, champion.GetAttackDamage() * damagePercentage); });
            }, timeToExplode, champion);
        }

        private void OnApplicationQuit() {
            champion.OnBulletHit -= Use;
        }
    }
}