using Champions;
using Core;
using Entities;
using Unity.VisualScripting;
using UnityEngine;

namespace Enemies {
    public class Dummy : Enemy {
        protected override void Move() { }

        public override void DealDamage(IDamageable damageable, float damage, Champion.DamageType damageType,
            bool shouldInvoke = true) {
        }

        public override void TakeFlatDamage(float damage) {
            Debug.Log("Dummy taking flat damage: " + damage);
            ShowDamageUI(damage);
        }
    }
}