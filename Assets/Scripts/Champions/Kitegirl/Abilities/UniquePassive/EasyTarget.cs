using Champions.Abilities;
using Enemies;
using Entities;
using UnityEngine;

namespace Champions.Kitegirl.Abilities.UniquePassive {
    public class EasyTarget : Ability {
        [SerializeField] private float maxHealthPercentageAsDamage = 0.05f;

        public override void Hook(Champion champion) {
            base.Hook(champion);
            champion.OnDamageDone += Use;
        }

        private void Use(IDamageable damageable) {
            if (damageable is Enemy enemy) {
                champion.DealDamage(enemy, enemy.GetMaxHealth() * maxHealthPercentageAsDamage, false);
            }
        }

        private void OnApplicationQuit() {
            champion.OnBulletHit -= Use;
        }
    }
}