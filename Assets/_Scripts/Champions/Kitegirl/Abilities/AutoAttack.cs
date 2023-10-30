using System;
using _Scripts.Champions.Abilities;
using _Scripts.Entities;

namespace _Scripts.Champions.Kitegirl.Abilities {
    public class AutoAttack : Ability {
        public override void Hook(Champion champion) {
            base.Hook(champion);
            abilityCooldown = 1f / champion.GetAttackSpeed();
        }

        public void OnUse(IDamageable damageable) {
            if (IsOnCooldown()) return;
            champion.OnAutoAttack(damageable);
            base.OnUse();
        }
    }
}